using System.Collections.Generic;
using Weichx.Util;
using Rotorz.ReorderableList;
using SpaceGame;
using SpaceGame.AI.Behaviors;
using SpaceGame.Assets;
using UnityEditor;
using Weichx.EditorReflection;

namespace Drawers {

    [PropertyDrawerFor(typeof(AIBehavior[]))]
    [PropertyDrawerFor(typeof(List<AIBehavior>))]
    [PropertyDrawerFor(typeof(ListX<AIBehavior>))]
    public class AIBehaviorListDrawer : ListDrawer {

        protected override ReorderableListFlags listFlags => ReorderableListFlags.DisableDuplicateCommand;
        protected override bool useCreateMenu => false;

        public  override void OnInitialize() {
            base.OnInitialize();
            adapter.onCreateElement = () => {
                propertyAsList.AddElement(new AIBehavior());
            };
        }
    }

}