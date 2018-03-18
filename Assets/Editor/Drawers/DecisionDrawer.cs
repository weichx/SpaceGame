using System;
using Editor.GUIComponents;
using SpaceGame.AI;
using SpaceGame.EditorComponents;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(Decision))]
    public class DecisionDrawer : ReflectedPropertyDrawer {

        private const string NameField = nameof(Decision.name);
        private const string ActionField = nameof(Decision.action);
        private const string EvaluatorField = nameof(Decision.evaluator);
        private const string ContextCreatorField = nameof(Decision.contextCreator);
        private const string ContextTypeField = nameof(Decision.contextType);

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {

            if (property[EvaluatorField].Value == null) {
                // todo -- context should match the decisions context
                property[EvaluatorField].Value = new Evaluator<EntityContext>();    
            }
            
            GUIRect guiRect = new GUIRect(position);
            EditorGUIX.PropertyField(guiRect, property[NameField]);
            EditorGUIX.TypePopup<DecisionContext>(guiRect, EditorGUIX.TempLabel("Context Type"), property[ContextTypeField]);
            EditorGUIX.PropertyField(guiRect, property[ActionField]);
            EditorGUIX.PropertyField(guiRect, property[ContextCreatorField]);
            EditorGUIX.PropertyField(guiRect, property[EvaluatorField]);

            if (property[ContextTypeField].DidChange) {
                property.ApplyChanges();
                Type newContextType = (Type) property[ContextTypeField].Value;
                if (!AssertCompatible(property[ActionField], newContextType)) {
                    property[ActionField].Value = null;
                }
                if (!AssertCompatible(property[ContextCreatorField], newContextType)) {
                    property[ContextCreatorField].Value = null;
                }
                if (!AssertCompatible(property[EvaluatorField], newContextType)) {
                    property[EvaluatorField].SetValueAndCopyCompatibleProperties(
                        EditorReflector.CreateGenericInstance(typeof(Evaluator<>), newContextType)
                    );
                    // todo -- for each consideration, make sure its compatible with new context type
                }
            }

        }

        private static bool AssertCompatible(ReflectedProperty reflectedProperty, Type newContextType) {
            IContextAware aware = reflectedProperty.Value as IContextAware;
            if (aware != null) {
                if (!aware.GetContextType().IsAssignableFrom(newContextType)) {
                    Debug.Log($"Not compatible! {aware.GetContextType().Name} & {newContextType.Name}");
                    return false;
                }
            }
            return true;
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIX.singleLineHeight;
            height += base.GetPropertyHeight(property[ActionField]);
            height += base.GetPropertyHeight(property[ContextCreatorField]);
            height += base.GetPropertyHeight(property[EvaluatorField]);
            return height;
        }

    }

}