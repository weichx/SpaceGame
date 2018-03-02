using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor {

    [CustomPropertyDrawer(typeof(FactionAttribute))]
    public class FactionDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            string[] factions = Faction.GetFactionNames();
            int index = property.intValue;

            int newIndex = EditorGUI.Popup(position, index, factions);
            property.intValue = newIndex;
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

    }

}