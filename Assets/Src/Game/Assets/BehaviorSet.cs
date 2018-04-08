using JetBrains.Annotations;
using Weichx.Util;
using SpaceGame.AI.Behaviors;

namespace SpaceGame.Assets {

    public class BehaviorSet : GameAsset {

        public ListX<AIBehavior> behaviors;

        [UsedImplicitly]
        private BehaviorSet() { }

        [UsedImplicitly]
        private BehaviorSet(int id, string name) : base(id, name) {
            this.behaviors = new ListX<AIBehavior>(4);
        }

        public AIBehavior GetDefaultBehavior() {
//            if (behaviors.Count == 0) {
//                AddAIBehavior(GameDatabase.ActiveInstance.CreateAsset<AIBehavior>());
//            }
            return behaviors[0];
        }

        [PublicAPI]
        public AIBehavior AddAIBehavior(AIBehavior behaviorDefinition, int index = -1) {
//            if (behaviorDefinition.behaviorSetId != id) {
//                behaviorDefinition.GetBehaviorSet()?.behaviors.Remove(behaviorDefinition);
//                behaviors.AddOrInsert(behaviorDefinition, index);
//            }
//            else {
//                behaviors.MoveToIndex(behaviorDefinition, index);
//            }
//            behaviorDefinition.behaviorSetId = id;
            return behaviorDefinition;
        }

    }

}