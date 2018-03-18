using System.Collections.Generic;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SpaceGame.Editor.MissionWindow {
    
    public class MissionPage  : MissionWindowPage {

        [SerializeField] private HorizontalPaneState splitterState;
        private TreeViewState treeState;

        private List<FactionDescription> factionDescriptions;
        private new MissionTreeView treeView;
        
        public MissionPage(MissionWindowState state) : base(state) {
            factionDescriptions = new List<FactionDescription>();
            Texture2D faction0Icon = EditorGUIUtility.Load("icon_faction_1.png") as Texture2D;
            Texture2D faction1Icon = EditorGUIUtility.Load("icon_faction_2.png") as Texture2D;
            Texture2D faction2Icon = EditorGUIUtility.Load("icon_faction_3.png") as Texture2D;
            factionDescriptions.Add(new FactionDescription(faction0Icon, "Faction 1", 1));        
            factionDescriptions.Add(new FactionDescription(faction1Icon, "Faction 2", 2));        
            factionDescriptions.Add(new FactionDescription(faction2Icon, "Faction 3", 3));  
            
        }

        public override void OnEnable() {
            treeView = new MissionTreeView(factionDescriptions);
        }
        
        public override void OnGUI() {
            InfamyGUI.HorizontalSplitPane(splitterState, RenderList, RenderDetails);
        }

        private void RenderList() {
            EditorGUILayout.BeginVertical();
            treeView.OnGUILayout();
            GUILayout.FlexibleSpace();
            InfamyGUI.Button("Create Faction", OnCreateFaction);
            EditorGUILayout.EndVertical();
        }

        
        private void OnCreateFaction() {
            
        }

        private void RenderDetails() {
            
        }

    }

}