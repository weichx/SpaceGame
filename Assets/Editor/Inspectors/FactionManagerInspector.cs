using System;
using UnityEditor;
using UnityEngine;

namespace SpaceGame.Editor {

    [CustomEditor(typeof(FactionManager))]
    public class FactionManagerInspector : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            Faction[] factions = Faction.GetFactions();

            for (int i = 0; i < factions.Length; i++) {
                Faction faction = factions[i];
                EditorGUILayout.LabelField(faction.Name);
                EditorGUI.indentLevel++;
                DrawDispositions(faction, factions);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Save Changes To JsonFile")) {
                Faction.SaveToJson();
            }
        }

        private static void DrawDispositions(Faction faction, Faction[] factionList) {            
            for (int i = 0; i < factionList.Length; i++) {
                Faction other = factionList[i];
                if(other == faction) continue;
                Disposition disposition = faction.GetDisposition(other);
                Disposition newDisposition = (Disposition)EditorGUILayout.EnumPopup(other.Name, disposition);
                faction.SetDisposition(other, newDisposition);
            }
        }

    }

}