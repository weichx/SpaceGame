using SpaceGame.Editor.Reflection;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor {

//    [CustomPropertyDrawer(typeof(FactionAttribute))]
//    public class FactionDrawer : UnityEditor.PropertyDrawer {
//
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//            EditorGUI.BeginProperty(position, label, property);
//
//            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//
//            string[] factions = Faction.GetFactionNames();
//            int index = property.intValue;
//
//            int newIndex = EditorGUI.Popup(position, index, factions);
//            property.intValue = newIndex;
//            EditorGUI.EndProperty();
//            property.serializedObject.ApplyModifiedProperties();
//        }
//
//    }


    [PropertyDrawerFor(typeof(FactionReference))]
    public class FactionDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(ReflectedProperty property, GUIContent label = null) {
            EditorGUILayout.BeginHorizontal();
            string[] factions = Faction.GetFactionNames();
            int index = ((FactionReference)property.Value).factionId;
            EditorGUILayout.PrefixLabel("Faction");
            int newIndex = EditorGUILayout.Popup(index, factions);
            if (index != newIndex) {
                property.Value = new FactionReference(newIndex);
            }
            EditorGUILayout.EndHorizontal();
            
        }

    }
}