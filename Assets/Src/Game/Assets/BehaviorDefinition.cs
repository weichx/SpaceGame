using JetBrains.Annotations;
using Lib.Util;
using SpaceGame.AI;
using Weichx.ReflectionAttributes;

namespace SpaceGame.Assets {

    public class BehaviorDefinition : GameAsset {

        public int behaviorSetId;
        public ListX<ActionDefinition> actions;

        [DefaultExpanded][CreateOnReflect]
        public Consideration[] considerations;
        
        [UsedImplicitly]
        private BehaviorDefinition() { }

        [UsedImplicitly]
        private BehaviorDefinition(int id, string name) : base(id, name) {
            behaviorSetId = -1;
            this.actions = new ListX<ActionDefinition>();
        }

        [PublicAPI]
        public ActionDefinition AddActionDefinition(ActionDefinition actionDefinition, int index = -1) {
            if (actionDefinition.behaviorId != id) {
                actionDefinition.GetBehaviorDefinition()?.actions.Remove(actionDefinition);
                this.actions.AddOrInsert(actionDefinition, index);
            }
            else {
                this.actions.MoveToIndex(actionDefinition, index);
            }
            actionDefinition.behaviorId = id;
            return actionDefinition;
        }

        public BehaviorSet GetBehaviorSet() {
            return GameDatabase.ActiveInstance.FindAsset<BehaviorSet>(behaviorSetId);
        }
        
    }

}