using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReeperCommon.Logging;
using UnityEngine;
using ReeperCommonTexture2DExtensions = ReeperCommon.Extensions.Texture2DExtensions;

namespace ReeperKSP.Extensions
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Saves texture into plugin dir with supplied name.
        /// Precondition: texture must be readable
        /// </summary>
        public static bool SaveToDisk(this Texture2D texture, string pathInGameData)
        {
            // texture format - needs to be ARGB32, RGBA32, RGB24 or Alpha8
            var validFormats = new List<TextureFormat>{ TextureFormat.Alpha8, 
                                                        TextureFormat.RGB24,
                                                        TextureFormat.RGBA32,
                                                        TextureFormat.ARGB32};

            if (!validFormats.Contains(texture.format))
                return ReeperCommonTexture2DExtensions.CreateReadable(texture).SaveToDisk(pathInGameData);


            if (pathInGameData.StartsWith("/"))
                pathInGameData = pathInGameData.Substring(1);

            pathInGameData = "GameData/" + pathInGameData;

            if (!pathInGameData.EndsWith(".png"))
                pathInGameData += ".png";

            try
            {
                var fullPath = Path.Combine(KSPUtil.ApplicationRootPath, pathInGameData);
                var directory = Path.GetDirectoryName(fullPath);

                if (directory == null)
                    throw new DirectoryNotFoundException("Couldn't get directory from " + fullPath);
                

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var file = new System.IO.FileStream(fullPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                var writer = new System.IO.BinaryWriter(file);
                writer.Write(texture.EncodeToPNG());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
