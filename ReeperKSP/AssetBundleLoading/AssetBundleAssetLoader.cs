﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using ReeperCommon.Containers;
using ReeperCommon.Extensions;
using ReeperCommon.Logging;
using ReeperCommon.Utilities;
using ReeperKSP.FileSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReeperKSP.AssetBundleLoading
{
    // ReSharper disable once UnusedMember.Global
    public static class AssetBundleAssetLoader
    {
        static readonly Dictionary<string, AssetBundle> InternalLoadedBundles =
            new Dictionary<string, AssetBundle>();

        private static readonly List<string> PendingBundles = new List<string>(); 

        // ReSharper disable once MemberCanBePrivate.Global
        public static ReadOnlyCollection<KeyValuePair<string, AssetBundleHandle>> LoadedBundles
        {
            get
            {
                return
                    new ReadOnlyCollection<KeyValuePair<string, AssetBundleHandle>>(InternalLoadedBundles
                        .Select(kvp => new KeyValuePair<string, AssetBundleHandle>(kvp.Key, new AssetBundleHandle(kvp.Value)))
                        .ToList());
            }
        }


        /// <summary>
        /// Synchronously inject assets into target fields
        /// </summary>
        /// <param name="target"></param>
        public static void InjectAssets(object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            InjectAssets(target, target.GetType());
        }


        /// <summary>
        /// Synchronously inject assets into the target type's static fields
        /// </summary>
        /// <param name="type"></param>
        public static void InjectAssets([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            InjectAssets(null, type);
        }


        private static void InjectAssets([CanBeNull] object target, [NotNull] Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            SendPreInjectionCallback(target);

            var fieldValues = new Dictionary<FieldInfo, Object>();

            foreach (var f in GetFieldsOf(target, targetType))
            {
                var assetAttribute = GetAttribute(f);

                if (!assetAttribute.Any()) continue;

                var assetBundlePath = GetAssetBundleFullPath(targetType, assetAttribute.Value);

                var isLoaded = GetLoadedAssetBundle(assetBundlePath);
                var assetBundleToLoadFrom = isLoaded.Or(() => LoadAssetBundle(assetBundlePath));
                var asset = LoadAssetImmediate(assetBundleToLoadFrom, f.FieldType, assetAttribute.Value);

                // cache all the field values we're going to set. If something goes wrong,
                // we don't want to have the target in a half-injected mutated state
                fieldValues.Add(f, asset);
            }

            AssignFields(target, fieldValues);
            SendPostInjectionCallback(target);
        }


        /// <summary>
        /// Injects fields of the target. If the target is null, only static fields will be injected.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static Coroutine<ValuelessCoroutine> InjectAssetsAsync(
            [CanBeNull] object target,
            [NotNull] Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            return CoroutineHoster.Instance.StartCoroutineValueless(InjectAssetsAsync_Internal(target, targetType));
        }


        /// <summary>
        ///  Injects static fields asynchronously
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static Coroutine<ValuelessCoroutine> InjectAssetsAsync([NotNull] Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            return InjectAssetsAsync(null, targetType);
        }


        /// <summary>
        /// Injects all fields of the target instance asynchronously
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Coroutine<ValuelessCoroutine> InjectAssetsAsync([NotNull] object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            return InjectAssetsAsync(target, target.GetType());
        }


        public static Coroutine<ValuelessCoroutine> LoadAssetBundleAsync([NotNull] IFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            return CoroutineHoster.Instance.StartCoroutine<ValuelessCoroutine>(LoadAssetBundleAsync(file.FullPath));
        }


        public static AssetBundleHandle LoadAssetBundle([NotNull] IFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            return new AssetBundleHandle(LoadAssetBundle(file.FullPath));
        }


        private static List<string> GetPathsOfNecessaryBundles([NotNull] Type targetType, [NotNull] IEnumerable<FieldInfo> fieldsToInject)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");
            if (fieldsToInject == null) throw new ArgumentNullException("fieldsToInject");

            return fieldsToInject
                .Select(fi => GetAssetBundleFullPath(targetType, GetAttribute(fi).Value))
                    .OrderByDescending(f => f)
                    .Distinct()
                    .ToList();
        }


        private static bool IsBundlePending(string bundlePath)
        {
            return PendingBundles.Contains(bundlePath);
        }


        private static IEnumerator WaitForBundle(string bundlePath, float timeoutSeconds = 60f)
        {
            Log.Verbose("Waiting for pending bundle: " + bundlePath);

            var timeoutTime = Time.realtimeSinceStartup + timeoutSeconds;

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (IsBundlePending(bundlePath) && (Time.realtimeSinceStartup < timeoutTime))
                yield return null;

            if (Time.realtimeSinceStartup > timeoutTime)
                throw new AssetBundleNotFoundException("Pending AssetBundle " + bundlePath + " timed out");

            if (!InternalLoadedBundles.Keys.Contains(bundlePath))
                throw new AssetBundleNotFoundException("Pending AssetBundle " + bundlePath +
                                                       " was not loaded correctly.");
        }


        // Note to future: if this isn't run as a Coroutine<ValuelessCoroutine> exceptions will be silenced! That's why I've made it 
        // private and forced the user to do the right thing using the exposed methods
        //
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        private static IEnumerator InjectAssetsAsync_Internal(
            [CanBeNull] object target, 
            [NotNull] Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            Log.Debug("Injecting assets async into " + targetType.Name);

            SendPreInjectionCallback(target);

            var targetFields = GetFieldsOf(target, targetType).Where(fi => GetAttribute(fi).Any()).ToList();
            var pathsOfBundles = GetPathsOfNecessaryBundles(targetType, targetFields);

            foreach (var path in pathsOfBundles)
                Log.Debug("Bundle dependency: " + path);

            // make sure all of the needed AssetBundles are loaded
            foreach (var bundlePath in pathsOfBundles)
            {
                if (GetLoadedAssetBundle(bundlePath).Any()) continue; // already have this bundle ready

                var bundleLoader = CoroutineHoster.Instance.StartCoroutine<Exception>(IsBundlePending(bundlePath) 
                    ? WaitForBundle(bundlePath) : 
                    LoadAssetBundleAsync(bundlePath));

                yield return bundleLoader.YieldUntilComplete;

                if (!bundleLoader.Error.Any()) continue; // no errors, carry on

                // ReSharper disable once ThrowingSystemException
                throw bundleLoader.Error.Value;
            }

            var fieldAssetLoader =
                CoroutineHoster.Instance.StartCoroutine<Dictionary<FieldInfo, Object>>(LoadFieldAssets(targetType, targetFields));

            yield return fieldAssetLoader.YieldUntilComplete;

            var fieldValues = fieldAssetLoader.Value;

            AssignFields(target, fieldValues);
            SendPostInjectionCallback(target);
        }


        // Coroutine<Dictionary<FieldInfo, Object>>
        private static IEnumerator LoadFieldAssets([NotNull] Type targetType, [NotNull] List<FieldInfo> targetFields)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");
            if (targetFields == null) throw new ArgumentNullException("targetFields");

            var fieldValues = new Dictionary<FieldInfo, Object>();

            // load values that will be injected
            foreach (var field in targetFields)
            {
                var attr = GetAttribute(field).Value;
                var assetBundlePath = GetAssetBundleFullPath(targetType, attr);
                var bundle = GetLoadedAssetBundle(assetBundlePath);

                if (!bundle.Any())
                    throw new InvalidOperationException("Somehow the expected AssetBundle is not loaded for " +
                                                        field.Name + ":" + field.FieldType + " on " +
                                                        field.DeclaringType + ", " + attr);

                var fieldValueLoader = CoroutineHoster.Instance.StartCoroutine<Exception>(LoadFieldValueAsync(bundle.Value, field, attr, fieldValues));

                yield return fieldValueLoader.YieldUntilComplete;

                if (fieldValueLoader.Error.Any())
                    // ReSharper disable once ThrowingSystemException
                    throw fieldValueLoader.Error.Value;
            }

            yield return fieldValues;
        }


        private static IEnumerator LoadAssetBundleAsync(string assetBundlePath)
        {
            if (string.IsNullOrEmpty(assetBundlePath)) throw new ArgumentException("cannot be null or empty", "assetBundlePath");

            if (!File.Exists(assetBundlePath)) throw new FileNotFoundException("File not found!", assetBundlePath);


            if (InternalLoadedBundles.Keys.Any(k => k == assetBundlePath))
                throw new ArgumentException("AssetBundle '" + assetBundlePath + "' has already been loaded");

            if (PendingBundles.Contains(assetBundlePath))
                throw new ArgumentException("AssetBundle '" + assetBundlePath +
                                            "' is pending. Wait for it instead of attempting to load another (which will fail)");

            var wwwLoad = new WWW(Uri.EscapeUriString(Application.platform == RuntimePlatform.WindowsPlayer ? "file:///" + assetBundlePath : "file://" + assetBundlePath));

            PendingBundles.Add(assetBundlePath);

            yield return wwwLoad;

            PendingBundles.Remove(assetBundlePath);

            if (!string.IsNullOrEmpty(wwwLoad.error))
                throw new AsyncAssetBundleLoadException(wwwLoad.error);

            try
            {
                var bundle = wwwLoad.assetBundle;

                if (bundle == null)
                    throw new ArgumentException("Failed to create AssetBundle from '" + assetBundlePath + "'");

                Log.Warning("Bundle loaded successfully: " + assetBundlePath);
                InternalLoadedBundles.Add(assetBundlePath, bundle);
            }
            catch (Exception e)
            {
                Log.Error("Caught exception: " + e + " in LoadAssetBundleAsync");
            }

            yield return true;
        }


        private static void SendPreInjectionCallback(object target)
        {
            var owner = target as IAssetBundleAssetsInjectedCallbackReceiver;
            if (owner != null)
                owner.BeforeAssetInjection();
        }


        private static void SendPostInjectionCallback(object target)
        {
            var owner = target as IAssetBundleAssetsInjectedCallbackReceiver;
            if (owner != null)
                owner.AfterAssetInjection();
        }


        /// <summary>
        /// Loads an asset synchronously
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="assetType"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        private static Object LoadAssetImmediate(AssetBundle bundle, Type assetType, AssetBundleAssetAttribute asset)
        {
            if (!bundle.GetAllAssetNames().Any(assetName => string.Equals(assetName, asset.AssetPathInBundle)))
                throw new AssetNotFoundException(asset);

            var loadedAsset = bundle.LoadAsset(asset.AssetPathInBundle, GetAssetTypeToLoad(assetType));

            if (loadedAsset == null)
                throw new FailedToLoadAssetException(asset, assetType);

            return ApplyCreationOptions(ConvertLoadedAssetToCorrectType(loadedAsset, assetType, asset), assetType, asset);
        }



        private static IEnumerator LoadFieldValueAsync(
            AssetBundle bundle,
            FieldInfo field,
            AssetBundleAssetAttribute asset,
            Dictionary<FieldInfo, Object> fieldValues)
        {
            if (!bundle.GetAllAssetNames().Any(assetName => string.Equals(assetName, asset.AssetPathInBundle)))
                throw new AssetNotFoundException(asset);

            // todo: make sure this handles multiple simultaneous requests of the same asset correctly by not failing
            // if the asset is already loaded by the time it returns
            var assetRequest = bundle.LoadAssetAsync(asset.AssetPathInBundle, GetAssetTypeToLoad(field.FieldType));

            yield return assetRequest;

            var loadedAsset = assetRequest.asset;

            if (loadedAsset == null)
                throw new FailedToLoadAssetException(asset, field.FieldType);

            fieldValues.Add(field,
                ApplyCreationOptions(ConvertLoadedAssetToCorrectType(loadedAsset, field.FieldType, asset),
                    field.FieldType, asset));
        }


        /// <summary>
        /// If the asset was loaded as a GameObject and the field type is some kind of component, we must grab the actual component
        /// out of the GameObject as that's the value that's really going to be injected
        /// </summary>
        /// <param name="loadedAsset"></param>
        /// <param name="assetFieldType"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        static Object ConvertLoadedAssetToCorrectType(Object loadedAsset, Type assetFieldType, AssetBundleAssetAttribute asset)
        {
            if (!AssetShouldBeLoadedAsGameObject(assetFieldType)) return loadedAsset;

            var goAsset = loadedAsset as GameObject;
            if (goAsset == null)
                throw new FailedToLoadAssetException(asset, typeof (GameObject));

            return assetFieldType == typeof (GameObject) ? (Object) goAsset : goAsset.GetComponent(assetFieldType)
                .IfNull(() =>
                {
                    throw new FailedToLoadAssetException (asset, assetFieldType);
                });
        }


        /// <summary>
        /// Taking a loaded asset, this will create an instance or whatever creation options are desired
        /// </summary>
        /// <param name="loadedAsset"></param>
        /// <param name="assetFieldType"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        static Object ApplyCreationOptions(Object loadedAsset, Type assetFieldType, AssetBundleAssetAttribute asset)
        {
            switch (asset.CreationStyle)
            {
                case AssetBundleAssetAttribute.AssetCreationStyle.Prefab:
                    return loadedAsset;

                case AssetBundleAssetAttribute.AssetCreationStyle.Instance:
                    {
                        if (!typeof(Component).IsAssignableFrom(assetFieldType)) return Object.Instantiate(loadedAsset);

                        var c = (Component)loadedAsset;

                        return Object.Instantiate(loadedAsset, c.transform.position, c.transform.rotation);
                    }

                default:
                    throw new NotImplementedException(asset.CreationStyle.ToString());
            }
        }


        static Maybe<AssetBundle> GetLoadedAssetBundle(string assetBundlePath)
        {
            if (string.IsNullOrEmpty(assetBundlePath))
                throw new ArgumentException("cannot be null or empty", "assetBundlePath");

            if (!File.Exists(assetBundlePath)) throw new FileNotFoundException("File not found!", assetBundlePath);

            AssetBundle bundle;

            return InternalLoadedBundles.TryGetValue(assetBundlePath, out bundle) ? bundle.ToMaybe() : Maybe<AssetBundle>.None;
        }


        static AssetBundle LoadAssetBundle(string assetBundlePath)
        {
            if (string.IsNullOrEmpty(assetBundlePath))
                throw new ArgumentException("cannot be null or empty", "assetBundlePath");

            if (!File.Exists(assetBundlePath)) throw new FileNotFoundException("File not found!", assetBundlePath);

            if (InternalLoadedBundles.Keys.Any(k => k == assetBundlePath))
                throw new ArgumentException("AssetBundle '" + assetBundlePath + "' has already been loaded");

            if (PendingBundles.Contains(assetBundlePath)) // todo: cancel pending asset bundle, if necessary? just throw for now
                throw new ArgumentException("Bundle '" + assetBundlePath + "' is already being loaded asynchronously", "assetBundlePath");
            
            var bundle = AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(assetBundlePath));

            if (bundle == null)
                throw new ArgumentException("Failed to create AssetBundle from '" + assetBundlePath + "'");

            InternalLoadedBundles.Add(assetBundlePath, bundle);

            return bundle;
        }


        static void AssignFields(object fieldOwner, Dictionary<FieldInfo, Object> fieldValues)
        {
            foreach (var kvp in fieldValues)
                kvp.Key.SetValue(kvp.Key.IsStatic ? null : fieldOwner, kvp.Value);
        }


        // ReSharper disable once UnusedMember.Global
        public static void UnloadAllBundles(bool unloadAllLoadedObjects = false)
        {
            foreach (var bundle in InternalLoadedBundles.Values)
            {
                try
                {
                    bundle.Unload(unloadAllLoadedObjects);
                }
                catch (Exception e)
                {
                    Log.Warning("Exception while unloading an AssetBundle: " + e);
                }
            }

            InternalLoadedBundles.Clear();
        }


        public static void UnloadBundle([NotNull] AssetBundleHandle bundleHandle, bool unloadAllLoadedObjects = false)
        {
            if (bundleHandle == null) throw new ArgumentNullException("bundleHandle");

            var targetBundle =
                InternalLoadedBundles.SingleOrDefault(loadedBundle => bundleHandle.Equals(loadedBundle.Value)).ToMaybe();

            if (!targetBundle.Any())
                throw new AssetBundleNotFoundException(bundleHandle);

            targetBundle.Value.Value.Unload(unloadAllLoadedObjects);

            InternalLoadedBundles.Remove(targetBundle.Value.Key);
        }
 

        static string GetAssetBundleFullPath(
            [NotNull] Type fieldOwnerType,
            [NotNull] AssetBundleAssetAttribute attribute)
        {
            if (fieldOwnerType == null) throw new ArgumentNullException("fieldOwnerType");
            if (attribute == null) throw new ArgumentNullException("attribute");
            var assemblyIdentifier = GetGameDatabaseUrlOfAssembly(fieldOwnerType.Assembly);

            // combine relative url with ownerType assembly url to get url of AssetBundle (including its name and extension),
            // then combine it with GameData path to come up with a fully qualified path
            var bundleFullPath =
                Path.GetFullPath(
                    Path.Combine(
                        Path.Combine(KSPUtil.ApplicationRootPath, "GameData" + assemblyIdentifier.Url),
                        attribute.AssetBundleRelativeUrl).Replace('\\', Path.DirectorySeparatorChar)
                        .Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(bundleFullPath))
                throw new AssetBundleNotFoundException(bundleFullPath, attribute.AssetBundleRelativeUrl);

            return bundleFullPath.ToLowerInvariant();
        }


        private static IUrlIdentifier GetGameDatabaseUrlOfAssembly(Assembly assembly)
        {
            var loadedAssembly = AssemblyLoader.loadedAssemblies.FirstOrDefault(la => ReferenceEquals(la.assembly, assembly)).ToMaybe();

            if (!loadedAssembly.Any())
                throw new LoadedAssemblyNotFoundException(assembly);

            if (String.IsNullOrEmpty(loadedAssembly.Value.url))
                throw new LoadedAssemblyNotFoundException(loadedAssembly.Value);

            return new KSPUrlIdentifier(loadedAssembly.Value.url, UrlType.Assembly);
        }


        static Maybe<AssetBundleAssetAttribute> GetAttribute(FieldInfo fi)
        {
            return fi.GetCustomAttributes(false).OfType<AssetBundleAssetAttribute>().SingleOrDefault().ToMaybe();
        }


        static IEnumerable<FieldInfo> GetFieldsOf([CanBeNull] object target, Type targetType)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            if (target != null) flags |= BindingFlags.Instance;

            return targetType.GetFields(flags);
        }


        public static IEnumerator<AssetBundleHandle> GetEnumerator()
        {
            return LoadedBundles.Select(kvp => kvp.Value).GetEnumerator();
        }


        /// <summary>
        /// According to the Unity docs, as of Unity5+ Component-type prefabs should be loaded as GameObjects
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        private static bool AssetShouldBeLoadedAsGameObject(Type assetType)
        {
            return typeof(Component).IsAssignableFrom(assetType);
        }


        /// <summary>
        /// According to the Unity docs, as of Unity5+ Component-type prefabs should be loaded as GameObjects
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        private  static Type GetAssetTypeToLoad(Type assetType)
        {
            return AssetShouldBeLoadedAsGameObject(assetType) ? typeof(GameObject) : assetType;
        }
    }
}