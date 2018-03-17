using System;
using Editor.GUIComponents;
using SpaceGame.AI;
using SpaceGame.Editor.GUIComponents;
using UnityEditor;
using UnityEngine;
using Weichx.EditorReflection;

namespace Editor.Drawers {

    [PropertyDrawerFor(typeof(Consideration), PropertyDrawerForOption.IncludeSubclasses)]
    public class ConsiderationDrawer : ReflectedPropertyDrawer {

        private static GUIRect guiRect = new GUIRect();

        private Type considerationType;

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {

            guiRect.SetRect(position);

            Type newConsiderationType;

            if (property.Value != null) {
                considerationType = property.Value.GetType();
                newConsiderationType = EditorGUIX.ConstructableTypePopup<Consideration>(guiRect, considerationType);
            }
            else {
                considerationType = null;
                newConsiderationType = EditorGUIX.ConstructableTypePopup<Consideration>(guiRect, considerationType);
            }
            
            if (newConsiderationType != considerationType) {
                Debug.Log(newConsiderationType.Name);
                considerationType = newConsiderationType;
                Consideration instance = Activator.CreateInstance(considerationType) as Consideration;
                property.SetValueAndCopyCompatibleProperties(instance);
            }
            
            EditorGUIX.DrawProperties(guiRect.GetRect(), property);

        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            float height = EditorGUIX.singleLineHeight;
            if (property.Value != null) {
                height += GetChildHeights(property);
            }
            return height;
        }

    }
    
    

}