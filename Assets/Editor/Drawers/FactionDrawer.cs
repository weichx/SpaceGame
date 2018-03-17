using SpaceGame;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(FactionReference))]
    public class FactionDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {

            string[] factions = Faction.GetFactionNames();
            int index = ((FactionReference) property.Value).factionId;
            int newIndex = EditorGUI.Popup(position, property.Label, index, factions);
            if (index != newIndex) {
                property.Value = new FactionReference(newIndex);
            }

        }

    }

}