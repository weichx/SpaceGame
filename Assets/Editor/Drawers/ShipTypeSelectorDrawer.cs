using System;
using System.Collections.Generic;
using System.Linq;
using Weichx.Util;
using SpaceGame;
using SpaceGame.Assets;
using SpaceGame.Engine;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using Weichx.ReflectionAttributes.Markers;

namespace Drawers {

    [PropertyDrawerFor(typeof(ShipTypeSelector))]
    public class ShipTypeSelectorDrawer : ReflectedPropertyDrawerX {

        private string[] shipTypeNames;
        private List<int> ids = new List<int>();

        private ListX<Chassis> chassis;
        
        public override void OnInitialize() {
            chassis = new ListX<Chassis>(Resources.FindObjectsOfTypeAll<Chassis>().Where(EditorUtility.IsPersistent));
            shipTypeNames = chassis.MapArray((c) => c.name);
        }
        
        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {

            string assetGuid = property.stringValue;
            GameDatabase db = GameDatabase.ActiveInstance;
            GameObject current = db.GetPrefabAsset(assetGuid);

//            IReadonlyListX<ShipTypeGroup> shipTypeGroups = db.GetAssetList<ShipTypeGroup>();
            GameObject newValue = EditorGUI.ObjectField(position, "Chassis Prefab", current, typeof(GameObject), false) as GameObject;

            if (newValue != current) {
                if (newValue == null) {
                    property.Value = string.Empty;
                }
                else {
                    property.Value = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newValue));
                }
            }
//            int idx = 0;
//            int currentId = property.intValue;
//            int currentIndex = -1;
//           // shipTypeNames.Clear();
//            ids.Clear();
//            
//            for (int i = 0; i < shipTypeGroups.Count; i++) {
//                ShipTypeGroup shipTypeGroup = shipTypeGroups[i];
//
//                string groupName = $"[{shipTypeGroup.name}]";
//
//                for (int j = 0; j < shipTypeGroup.ships.Count; j++) {
//                    ShipType shipType = shipTypeGroup.ships[j];
//              //      shipTypeNames.Add($"{groupName} {shipType.name}");
//                    ids.Add(shipType.id);
//                    if (shipType.id == currentId) {
//                        currentIndex = idx;
//                    }
//                    idx++;
//                }
//
//            }
//
//            int newIndex = EditorGUI.Popup(position, property.Label, currentIndex, shipTypeNames.ToArray());
//            if (newIndex != currentIndex) {
//                property.Value = db.FindAsset<ShipType>(ids[newIndex]).id;
//            }


        }

    }

}