using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Freespace {

    internal static class ImportUtil {

        private static readonly string PathSeperator;

        static ImportUtil() {
            PathSeperator = GetPathSeperator();
        }

        public static string GetShipClassFromPath(string path) {
            if (path.Contains("Very Large Ships")) return "Very Large Ships";
            if (path.Contains("Small Ships")) return "Small Ships";
            if (path.Contains("Medium Ships")) return "Medium Ships";
            if (path.Contains("Large Ships")) return "Large Ships";
            if (path.Contains("Shadow")) return "Shadow";
            if (path.Contains("Misc")) return "Misc";
            if (path.Contains("Transport")) return "Transport";
            if (path.Contains("Stationary Objects")) return "Stationary Objects";
            return path.Contains("Transports") ? "Transports" : "Unclassifed";
        }

        public static readonly CultureInfo CultureInfo = new CultureInfo("en-US", false);

        private static string GetPathSeperator() {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;

            switch (pid) {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                    return "\\";
                default:
                    return "/";
            }
        }

        public static string GetTexturePathFromImportLocation(string path) {
            string dirPath = Path.GetDirectoryName(path);
            string[] segments = dirPath.Split('/');

            string dirName = segments[segments.Length - 1];
            string[] dirStructureToTry = {"model", "models", "Model", "Models"};

            bool valid = false;

            for (int i = 0; i < dirStructureToTry.Length; i++) {
                if (dirName != dirStructureToTry[i]) continue;
                valid = true;
                break;
            }

            if (valid) {
                string[] namesToTry = {"maps", "Maps", "textures"};

                for (int i = 0; i < namesToTry.Length; i++) {
                    segments[segments.Length - 1] = namesToTry[i];
                    string texturePath = string.Join(PathSeperator, segments);

                    if (Directory.Exists(texturePath)) {
                        return texturePath;
                    }
                }
            }

            return dirPath.Replace("/", "\\");
        }

        public static string MakePath(string[] segments) {
            StringBuilder builder = new StringBuilder();

            int lastIdx = segments.Length - 1;

            if (segments[lastIdx].StartsWith(".")) {
                for (int i = 0; i < segments.Length - 2; i++) {
                    builder.Append(segments[i]);
                    builder.Append(PathSeperator);
                }

                builder.Append(segments[segments.Length - 2]);
                builder.Append(segments[segments.Length - 1]);
            }

            else {
                for (int i = 0; i < segments.Length - 1; i++) {
                    builder.Append(segments[i]);
                    builder.Append(PathSeperator);
                }

                builder.Append(segments[segments.Length - 1]);
            }

            return builder.ToString();
        }

        private static readonly string[] TextureExtensions = {".dds", ".tga", ".png", ".jpg"};

        private static string GetImportedTexturePath(string assetRoot, string textureName) {
            return MakePath(new[] {
                assetRoot, "Textures", textureName
            });
        }

        public static bool TryToImportTexture(string importPath, string assetPath) {
            for (int i = 0; i < TextureExtensions.Length; i++) {
                string filePath = MakePath(new[] {importPath, TextureExtensions[i]});

                if (File.Exists(filePath)) {
                    assetPath += TextureExtensions[i];
                    string checkPath = MakePath(new[] {"Assets", assetPath.Substring(Application.dataPath.Length + 1)});
                    //todo -- could also check the common texture directory and not import if file exists there
                    if (AssetDatabase.LoadAssetAtPath<Texture2D>(checkPath) == null) {
                        File.Copy(filePath, assetPath, true);
                    }
                    return true;
                }
            }

            return false;
        }

        public static Texture2D GetTexture(string assetPath, string textureName, string type) {
            string textureName2 = textureName.Substring(0, textureName.Length - 1);

            string[] pathsToTry = {
                GetImportedTexturePath(assetPath, textureName),
                GetImportedTexturePath(assetPath, textureName + "-01"),
                GetImportedTexturePath(assetPath, textureName + "-02"),
                "Assets/Freespace Assets/Common/Textures/" + textureName + type,
                "Assets/Freespace Assets/Common/Textures/" + textureName + "a" + type,
                "Assets/Freespace Assets/Common/Textures/" + textureName2 + type
            };

            for (int i = 0; i < pathsToTry.Length; i++) {
                string path = pathsToTry[i];

                for (int j = 0; j < TextureExtensions.Length; j++) {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path + TextureExtensions[j]);
                    if (texture != null) return texture;
                }
            }

            if (type != "-glow" && type != "-shine") {
                Debug.LogWarning("Unable to load texture asset for: " + textureName + type);
            }

            return null;
        }

    }

}