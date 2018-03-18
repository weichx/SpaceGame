using SpaceGame;
using SpaceGame.EditorComponents;
using UnityEngine;
using Weichx.EditorReflection;

namespace Editor.Drawers {

    [PropertyDrawerFor(typeof(ShipDefinition))]
    public class ShipDefinitionDrawer : ReflectedPropertyDrawer {

        public override void OnGUI(Rect position, ReflectedProperty property, GUIContent label = null) {
            EditorGUIX.DrawProperties(position, property);
        }

    }

}