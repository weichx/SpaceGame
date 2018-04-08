using System;
using SpaceGame;
using SpaceGameEditor.Drawers;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;
using SceneEntitySelector = Weichx.ReflectionAttributes.Markers.SceneEntitySelector;

namespace Drawers {

    [PropertyDrawerFor(typeof(SceneEntitySelector))]
    public class SceneEntitySelectorDrawer : ReflectedPropertyDrawerX {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            guiRect.SetRect(position);

            int entityId = property.intValue;
            GameDatabase db = GameDatabase.ActiveInstance;

            Entity sceneEntity = db.FindSceneEntityById(entityId);
            GameObject current = sceneEntity?.gameObject;

            GameObject newValue = EditorGUI.ObjectField(position, "Scene Entity", current, typeof(GameObject), true) as GameObject;

            if (newValue != current) {
                if (newValue != null) {
                    if (EditorUtility.IsPersistent(newValue)) {
                        Debug.Log("Can only set a scene entity");
                        return;
                    }
                    Entity newComponent = newValue.GetComponent<Entity>();
                    if (newComponent == null) {
                        Debug.Log("Need to set a gameobject with an entity component");
                        return;
                    }

                    EntityDefinition entityDefintion = db.GetEntityDefinitionForSceneEntity(newComponent.id);
                    if (entityDefintion != null) {
                        entityDefintion.sceneEntityId = -1;
                        Debug.Log($"Unassigned Scene Entity for {entityDefintion.name}");
                    }

                    property.Value = newComponent.id;

                }
                else {
                    property.Value = -1;
                }
            }

        }

    }

}