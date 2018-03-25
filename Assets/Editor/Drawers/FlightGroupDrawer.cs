using Editor.GUIComponents;
using SpaceGame;
using SpaceGame.EditorComponents;
using UnityEngine;
using Weichx.EditorReflection;

namespace SpaceGameEditor.Drawers {

    [PropertyDrawerFor(typeof(FlightGroupDefinition))]
    public class FlightGroupDrawer : ReflectedPropertyDrawer {

        private static readonly GUIRect s_guiRect = new GUIRect();

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label) {
            s_guiRect.SetRect(position);

            EditorGUIX.PropertyField(s_guiRect, property[nameof(FlightGroupDefinition.name)]);
            EditorGUIX.PropertyField(s_guiRect, property[nameof(FlightGroupDefinition.goals)]);

        }

        public override float GetPropertyHeight(ReflectedProperty property) {
            return property["goals"].GetPropertyHeight() +
                   property["name"].GetPropertyHeight();
        }

    }

}