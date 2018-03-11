using SpaceGame.Util.Texture2DExtensions;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor.Windows {

    public class TextureFromColorWindow : EditorWindow {

        [MenuItem("Window/Texture From Color")]
        private static void Init() {
            GetWindow<TextureFromColorWindow>("Texture From Color");
        }

        private Color color;
        private Texture2D textureAsset;
        
        private void OnGUI() {
            
            textureAsset = EditorGUILayout.ObjectField("Texture", textureAsset, typeof(Texture2D), false) as Texture2D;
            
            Color old = new Color(color.r, color.g, color.b, color.a);
              
            color = EditorGUILayout.ColorField(old);
            if (color != old && textureAsset != null) {
                textureAsset.SetColor(color);
            }

            if (GUILayout.Button("Create Texture")) {
                textureAsset = new Texture2D(32, 32, TextureFormat.RGBA32, true);
                textureAsset.alphaIsTransparency = true;
                textureAsset.SetColor(color);
                AssetDatabase.CreateAsset(textureAsset, $"Assets/Editor Default Resources/UI Textures/Texture_{Random.Range(0, 9999)}.asset");
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
          
        }

    }

}