using Editor.GUIComponents;
using SpaceGame;
using SpaceGame.EditorComponents;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(EntityDefinition))]
    public class EntityDefinitionDrawer : ReflectedPropertyDrawer {

        private const string NameField = nameof(EntityDefinition.name);
        private const string ShipTypeField = nameof(EntityDefinition.shipTypeId);
        private const string GoalsField = nameof(EntityDefinition.goals);
        private const string SceneEntityIdField = nameof(EntityDefinition.sceneEntityId);
        
        private static readonly GUIRect s_guiRect = new GUIRect();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            s_guiRect.SetRect(position);
            EditorGUIX.PropertyField(s_guiRect, property[NameField]);
            EditorGUIX.PropertyField(s_guiRect, property[ShipTypeField]);
            EditorGUIX.PropertyField(s_guiRect, property[SceneEntityIdField]);
            EditorGUIX.PropertyField(s_guiRect, property[GoalsField]);
            
            if (!property.HasModifiedProperties) return;
            
            GameDatabase db = GameDatabase.ActiveInstance;
            int sceneEntityId = property[SceneEntityIdField].intValue;
            Entity sceneEntity = db.FindSceneEntityById(sceneEntityId);

            bool sceneEntityChanged = property[SceneEntityIdField].HasModifiedProperties;
            
            property.ApplyChanges();
            
            if (sceneEntityChanged) {
                db.UpdateDefinitionFromSceneEntity(property.GetValue<EntityDefinition>(), sceneEntity);
            }

            if (property.HasModifiedProperties) {
                Debug.Log("Changed!");
                property.ApplyChanges();
                if (sceneEntity != null) {
                    db.UpdateSceneEntityFromDefinition(sceneEntity, property.GetValue<EntityDefinition>());
                }
            }
            
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = 0f;
            height += EditorGUIX.GetChildHeights(property[NameField]);
            height += EditorGUIX.GetChildHeights(property[ShipTypeField]);
            height += EditorGUIX.GetChildHeights(property[SceneEntityIdField]);
            height += EditorGUIX.GetChildHeights(property[GoalsField]);
            return height;
        }

    }

}