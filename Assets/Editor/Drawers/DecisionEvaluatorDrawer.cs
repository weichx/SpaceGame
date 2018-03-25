using Editor.GUIComponents;
using Rotorz.ReorderableList;
using SpaceGame.AI;
using SpaceGame.EditorComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(Evaluator), PropertyDrawerForOption.IncludeSubclasses)]
    public class DecisionEvaluatorDrawer : ReflectedPropertyDrawer {

        private const string NameField = nameof(Evaluator.name);
        private const string WeightField = nameof(Evaluator.weight);
        private const string ConsiderationsField = nameof(Evaluator.considerations);
        private const string BonusCalculatorField = nameof(Evaluator.bonusCalculator);

        private bool initialized;
        private string[] subclassNames;
        private ReflectedPropertyAdapter adapter;
        private GUIRect guiRect = new GUIRect();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {

            if (property == null) return;
            guiRect.SetRect(position);
            Initialize(property);

            EditorGUIX.Foldout(guiRect, property);

            property.IsExpanded = true; //temp
            if (property.IsExpanded) {
                EditorGUI.indentLevel++;
                EditorGUIX.PropertyField(guiRect, property[NameField]);
                EditorGUIX.PropertyField(guiRect, property[WeightField]);
                EditorGUIX.PropertyField(guiRect, property[BonusCalculatorField]);

                ReorderableListGUI.ListFieldAbsolute(EditorGUI.IndentedRect(guiRect.GetRect()), adapter);

                EditorGUI.indentLevel--;
            }
        }

        public void Initialize(ReflectedProperty source) {
            if (!initialized && source != null) {
                initialized = true;
                Debug.Assert(source[ConsiderationsField] != null, "source[ConsiderationsField] != null");
                adapter = new ReflectedPropertyAdapter((ReflectedListProperty) source[ConsiderationsField]);
            }
        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIUtility.singleLineHeight;
            if (property?.Value != null && property.IsExpanded) {
                Initialize(property);
                height += EditorGUIX.singleLineHeight * 3f;
                height += ReorderableListGUI.CalculateListFieldHeight(adapter);
            }
            return height;
        }

    }

}