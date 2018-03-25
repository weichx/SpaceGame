using Editor.GUIComponents;
using SpaceGame;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Editor.Drawers {

    [PropertyDrawerFor(typeof(ShipType))]
    public class ShipTypeDrawer : ReflectedPropertyDrawer {

        private static GUIRect guiRect = new GUIRect();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            guiRect.SetRect(position);
            EditorGUIX.PropertyField(guiRect.GetFieldRect(), property["name"]);
            EditorGUIX.DrawProperties(guiRect.GetFieldRect(8), property, new [] {"name"});
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            return property.ChildCount * EditorGUIUtility.singleLineHeight;
        }

    }

}