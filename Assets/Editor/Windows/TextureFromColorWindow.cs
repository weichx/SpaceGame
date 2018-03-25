using Weichx.Util.Texture2DExtensions;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Windows {

    public class TextureFromColorWindow : EditorWindow {

        private Color color;
        private Texture2D textureAsset;
        private Color borderColor;
        private Color borderColorTop;
        private Color borderColorBottom;
        private Color borderColorLeft;
        private Color borderColorRight;
        private bool useBorder;

        private bool useSeperateBorders;
        private bool useBorderTop;
        private bool useBorderBottom;
        private bool useBorderLeft;
        private bool useBorderRight;

        private void OnGUI() {

            textureAsset = EditorGUILayout.ObjectField("Texture", textureAsset, typeof(Texture2D), false) as Texture2D;

            Color old = new Color(color.r, color.g, color.b, color.a);

            color = EditorGUILayout.ColorField(old);

            if (color != old && textureAsset != null) {
                textureAsset.SetColor(color);
            }

            useBorder = EditorGUILayout.Toggle("Border", useBorder);

            if (useBorder) {
                useSeperateBorders = EditorGUILayout.Toggle("Seperate Borders", useSeperateBorders);

                if (useSeperateBorders) {
                    EditorGUILayout.BeginHorizontal();
                    useBorderTop = EditorGUILayout.Toggle("Top", useBorderTop);
                    if (useBorderTop) {
                        borderColorTop = EditorGUILayout.ColorField(borderColorTop);
                    }
                    else {
                        borderColorTop = color;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    useBorderBottom = EditorGUILayout.Toggle("Bottom", useBorderBottom);
                    if (useBorderBottom) {
                        borderColorBottom = EditorGUILayout.ColorField(borderColorBottom);
                    }
                    else {
                        borderColorBottom = color;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    useBorderLeft = EditorGUILayout.Toggle("Left", useBorderLeft);
                    if (useBorderLeft) {
                        borderColorLeft = EditorGUILayout.ColorField(borderColorLeft);
                    }
                    else {
                        borderColorLeft = color;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    useBorderRight = EditorGUILayout.Toggle("Right", useBorderRight);
                    if (useBorderRight) {
                        borderColorRight = EditorGUILayout.ColorField(borderColorRight);
                    }
                    else {
                        borderColorRight = color;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else {
                    borderColor = EditorGUILayout.ColorField(new GUIContent("Border Color"), borderColor);
                    borderColorTop = borderColor;
                    borderColorBottom = borderColor;
                    borderColorLeft = borderColor;
                    borderColorRight = borderColor;
                }

            }

            if (GUILayout.Button("Create Texture")) {
                if (!useBorder) {
                    textureAsset = new Texture2D(32, 32, TextureFormat.RGBA32, true);
                    textureAsset.alphaIsTransparency = true;
                    textureAsset.SetColor(color);
                    AssetDatabase.CreateAsset(textureAsset, $"Assets/Editor Default Resources/UI Textures/Texture_{Random.Range(0, 9999)}.asset");
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
                else {
                    int borderWidth = 1;
                    textureAsset = new Texture2D(9, 9, TextureFormat.RGBA32, true);
                    textureAsset.alphaIsTransparency = true;
                    for (int x = 0; x < textureAsset.width; x++) {
                        for (int y = 0; y < textureAsset.height; y++) {
                            if (x < borderWidth) {
                                textureAsset.SetPixel(x, y, borderColorLeft);
                            }
                            else if (x > textureAsset.width - 1 - borderWidth) {
                                textureAsset.SetPixel(x, y, borderColorRight);
                            }
                            else if (y < borderWidth) {
                                textureAsset.SetPixel(x, y, borderColorTop);
                            }
                            else if (y > textureAsset.height - 1 - borderWidth) {
                                textureAsset.SetPixel(x, y, borderColorBottom);
                            }
                            else {
                                textureAsset.SetPixel(x, y, color);
                            }
                        }
                    }

                    textureAsset.Apply();
                    AssetDatabase.CreateAsset(textureAsset, $"Assets/Editor Default Resources/UI Textures/Texture_{Random.Range(0, 9999)}.asset");
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }

        }

    }

}