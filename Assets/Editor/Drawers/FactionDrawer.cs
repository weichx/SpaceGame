using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGame.Editor {

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