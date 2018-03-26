using System.Collections.Generic;
using SpaceGame;
using SpaceGame.Assets;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.ReflectionAttributes.Markers;

namespace Drawers {

    [PropertyDrawerFor(typeof(ShipTypeSelector))]
    public class ShipTypeSelectorDrawer : ReflectedPropertyDrawerX{

        private List<string> shipTypeNames = new List<string>();
        private List<int> ids = new List<int>();
        
        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            
            GameDatabase db = GameDatabase.ActiveInstance;

            List<ShipTypeGroup> shipTypeGroups = db.shipTypeGroups;
            int idx = 0;
            int currentId = property.intValue;
            int currentIndex = -1;
            shipTypeNames.Clear();
            ids.Clear();
            
            for (int i = 0; i < shipTypeGroups.Count; i++) {
                ShipTypeGroup shipTypeGroup = shipTypeGroups[i];

                string groupName = $"[{shipTypeGroup.name}]";

                for (int j = 0; j < shipTypeGroup.ships.Count; j++) {
                    ShipType shipType = shipTypeGroup.ships[j];
                    shipTypeNames.Add($"{groupName} {shipType.name}");
                    ids.Add(shipType.id);
                    if (shipType.id == currentId) {
                        currentIndex = idx;
                    }
                    idx++;
                }

            }

            int newIndex = EditorGUI.Popup(position, property.Label, currentIndex, shipTypeNames.ToArray());
            if (newIndex != currentIndex) {
                property.Value = db.FindAsset<ShipType>(ids[newIndex]).id;
            }


        }

    }

}