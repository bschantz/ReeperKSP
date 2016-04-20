using System;
using ReeperCommon.Containers;
using ReeperCommon.Logging;
using UnityEngine;

namespace ReeperKSP.Extensions
{
// ReSharper disable once UnusedMember.Global
    public static class CameraExtensions
    {
// ReSharper disable once UnusedMember.Global
        public static void CaptureSingleFrame(this Camera cam, string filename)
        {
            if (cam == null) throw new ArgumentNullException("cam");
            if (filename == null) throw new ArgumentNullException("filename");
            if (string.IsNullOrEmpty(filename)) throw new ArgumentException("filename cannot be empty");

            cam.gameObject.AddComponent<SingleFrameCapture>().Filename = filename;
        }


// ReSharper disable once UnusedMember.Global
        public static Texture2D Capture(this Camera cam)
        {
            if (cam == null) throw new ArgumentNullException("cam");

            var oldTarget = cam.targetTexture;
            var oldRt = RenderTexture.active;

            var temp = RenderTexture.GetTemporary(Screen.width, Screen.height);
            var texture = new Texture2D(temp.width, temp.height);

            cam.targetTexture = temp;
            cam.Render();
            cam.targetTexture = oldTarget;

            RenderTexture.active = temp;
            texture.ReadPixels(new Rect(0, 0, temp.width, temp.height), 0, 0);
            RenderTexture.active = oldRt;

            RenderTexture.ReleaseTemporary(temp);

            return texture;
        }


// ReSharper disable once UnusedMember.Global
        public static void RenderSingleFrame(this Camera cam, string filename)
        {
            if (cam == null) throw new ArgumentNullException("cam");
            if (filename == null) throw new ArgumentNullException("filename");
            if (string.IsNullOrEmpty(filename)) throw new ArgumentException("filename cannot be empty");

            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
            var rt = RenderTexture.GetTemporary(texture.width, texture.height);
            var old = RenderTexture.active;

            RenderTexture.active = rt;
            cam.Render();

            texture.ReadPixels(cam.pixelRect, 0, 0);
            texture.Apply();

            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = old;

            texture.SaveToDisk(filename);
        }


// ReSharper disable once ClassNeverInstantiated.Local
        private class SingleFrameCapture : MonoBehaviour
        {
            public string Filename;
            private Camera _camera;

            // ReSharper disable once UnusedMember.Local
            private void Awake()
            {
                _camera = GetComponent<Camera>();

                _camera.IfNull(() => Log.Error(typeof (SingleFrameCapture).Name + ": no Camera attached"));

            }
// ReSharper disable once UnusedMember.Local
            void OnPostRender()
            {
                if (!string.IsNullOrEmpty(Filename) && _camera != null)
                {
                    var tex = new Texture2D(Mathf.FloorToInt(_camera.pixelWidth), Mathf.FloorToInt(_camera.pixelHeight), TextureFormat.ARGB32, false);

                    tex.ReadPixels(_camera.pixelRect, 0, 0);
                    tex.SaveToDisk(Filename);

                    Filename = "";
                }

                Destroy(this);
            }
        }
    }


}
