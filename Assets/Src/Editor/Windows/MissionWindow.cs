using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using SpaceGame;
using SpaceGame.Util;
using SpaceGame.Util.Texture2DExtensions;
using UnityEditor;
using UnityEngine;

namespace Src.Editor {

    public class ClickableLabel<T> {

        public T data;
        public Action<T> onClick;
        public GUIContent content;

        public ClickableLabel(string labelText, T data, Action<T> callback) {
            this.content = new GUIContent(labelText);
            this.data = data;
            this.onClick = callback;
            content.image = EditorGUIUtility.FindTexture("tree_icon");
        }

        public void OnGUI(Rect rect, GUIStyle style) {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Rect toggleRect = new Rect(rect);
            toggleRect.width = 16;
            rect.x += 16;
            rect.width -= 16;
            EditorGUI.Toggle(toggleRect, GUIContent.none, true);
            EditorGUI.LabelField(rect, content, style);

            switch (Event.current.GetTypeForControl(controlID)) {
                case EventType.MouseDown:
                    if (rect.Contains(Event.current.mousePosition)) {
                        onClick(data);
                        Event.current.Use();
                    }

                    break;
            }
        }

        public void OnGUILayout(GUIStyle style) {
            OnGUI(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label), style);
        }

    }

    public class MissionWindow : EditorWindow {

        private List<Entity> entitites;
        private GUIStyle selectedStyle;
        private GUIStyle unselectedStyle;

        [MenuItem("Window/Mission Editor")]
        private static void Init() {
            GetWindow<MissionWindow>();
        }

        private void MakeStyles() {
            unselectedStyle = EditorStyles.label;
            selectedStyle = new GUIStyle(EditorStyles.label);
            Texture2D tex = new Texture2D(2, 2);
            tex.SetColor(Color.blue);
            selectedStyle.normal = new GUIStyleState() {
                background = tex,
                textColor = Color.white
            };
        }

        private readonly HorizontalPaneState paneConfiguration = new HorizontalPaneState() {
            initialLeftPaneWidth = 120,
            minPaneWidthLeft = 65,
            minPaneWidthRight = 100
        };

        private void OnEnable() {
            EditorApplication.hierarchyWindowChanged += CollectEntities;
            CollectEntities();
        }

        private void OnDisable() {
            EditorApplication.hierarchyWindowChanged -= CollectEntities;
        }

        private void CollectEntities() {
            entitites = Resources.FindObjectsOfTypeAll<Entity>().ToList().FindAll((entity) => {
                return !entity.gameObject.IsPrefab();
            });

            entityLabels = new List<ClickableLabel<Entity>>();
            for (int i = 0; i < entitites.Count; i++) {
                entityLabels.Add(new ClickableLabel<Entity>(entitites[i].name, entitites[i], OnSelected));
            }
        }

        private readonly string[] tabs = {"Entities", "Ships"};

        private List<ClickableLabel<Entity>> entityLabels;
        private ClickableLabel<Entity> selected;
        private Entity selectedEntity;

        public void Vertical(Action action) {
            EditorGUILayout.BeginVertical();
            action();
            EditorGUILayout.EndVertical();
        }

        public void Horizontal(Action action) {
            EditorGUILayout.BeginHorizontal();
            action();
            EditorGUILayout.EndHorizontal();
        }

        public void ButtonGroup() {
            EditorGUILayout.BeginHorizontal();
            GUIStyle s0 = new GUIStyle(EditorStyles.miniButtonLeft);
            GUIStyle s1 = new GUIStyle(EditorStyles.miniButton);
            GUIStyle s2 = new GUIStyle(EditorStyles.miniButtonRight);
            s0.margin = new RectOffset();
            s1.margin = new RectOffset();
            s2.margin = new RectOffset();

            if (GUILayout.Button("Sort By Name", s0)) {
                entitites.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
                entityLabels = new List<ClickableLabel<Entity>>();
                for (int i = 0; i < entitites.Count; i++) {
                    entityLabels.Add(new ClickableLabel<Entity>(entitites[i].name, entitites[i], OnSelected));
                }
            }

            if (GUILayout.Button("Group Factions", s1)) { }

            EditorGUILayout.EndHorizontal();
        }

        public void FlexibleSpace() {
            GUILayout.FlexibleSpace();
        }

        public void OnGUI() {
            MakeStyles();
            GUILayout.Toolbar(0, tabs);
            EditorGUILayoutHorizontalPanes.Begin(paneConfiguration);

            Vertical(() => {
                Vertical(() => {
                    Repeat(entityLabels, (item) => {
                        GUIStyle style = item.data == selectedEntity ? selectedStyle : unselectedStyle;
                        item.OnGUILayout(style);
                    });
                });
                FlexibleSpace();
                Horizontal(ButtonGroup);
            });
            EditorGUILayoutHorizontalPanes.Splitter();
            RenderDetails();
            EditorGUILayoutHorizontalPanes.End();
        }

        private void OnSelected(Entity entity) {
            selectedEntity = entity;
        }

        private void RenderDetails() {
            Vertical((() => {
                ;
                EditorGUILayout.TextField("Entity Name");
                ShipType type = ShipType.TieFighter;
                type = (ShipType) EditorGUILayout.EnumPopup("Ship Type", type);
            }));
//            EditorGUILayout.ObjectField(new GUIContent(""))
        }

        enum MissionWindowPage {

            Enity,
            Ship

        }

        class MissionWindowState {

            public int selectedEntityId;
            public MissionWindowPage currentPage;
            public List<ShipDefinition> shipDefinitions;

        }

        private void Repeat<T>(List<T> list, Action<T> render) {
            list.ForEach(render);
        }

        class ShipDefinition {

            public string name;
            public GameObject prefab;

            public float maxSpeed;
            public float turnRate;
            public float accelerationRate;

            public float hitpoints;
            public float shieldPoints;

        }

//            /*
//             * ToSerializedObject() {
//             *     SerializedObject obj = new SerializedObject();
//             *     obj.AddProperty(nameof(prefab), prefab);
//             *     name = obj.GetProperty(nameof(name), name);
//             */
//
//        }
//        
        enum ShipType {

            TieFighter,
            BulkFrigate,
            StationType1

        }

    }

}