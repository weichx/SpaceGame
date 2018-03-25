using Editor.GUIComponents;
using SpaceGame;
using SpaceGame.Assets;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(ShipTypeGroup))]
    public class ShipGroupDrawer : ReflectedPropertyDrawer {

        private const string NameField = nameof(ShipTypeGroup.name);
        private const string ShipCategoryField = nameof(ShipTypeGroup.shipCategory);

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            GUIRect guiRect = new GUIRect();
            guiRect.SetRect(position);

            EditorGUIX.PropertyField(guiRect.GetFieldRect(), property[NameField]);
            EditorGUIX.PropertyField(guiRect.GetFieldRect(), property[ShipCategoryField]);

        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            return 2f * EditorGUIUtility.singleLineHeight;
        }

    }

}