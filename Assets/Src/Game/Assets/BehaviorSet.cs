using JetBrains.Annotations;
using Lib.Util;

namespace SpaceGame.Assets {

    public class BehaviorSet : GameAsset {

        public ListX<BehaviorDefinition> behaviors;

        [UsedImplicitly]
        private BehaviorSet() { }

        [UsedImplicitly]
        private BehaviorSet(int id, string name) : base(id, name) {
            this.behaviors = new ListX<BehaviorDefinition>(4);
        }

        public BehaviorDefinition GetDefaultBehavior() {
            if (behaviors.Count == 0) {
                AddBehaviorDefinition(GameDatabase.ActiveInstance.CreateAsset<BehaviorDefinition>());
            }
            return behaviors[0];
        }

        [PublicAPI]
        public BehaviorDefinition AddBehaviorDefinition(BehaviorDefinition behaviorDefinition, int index = -1) {
            if (behaviorDefinition.behaviorSetId != id) {
                behaviorDefinition.GetBehaviorSet()?.behaviors.Remove(behaviorDefinition);
                behaviors.AddOrInsert(behaviorDefinition, index);
            }
            else {
                behaviors.MoveToIndex(behaviorDefinition, index);
            }
            behaviorDefinition.behaviorSetId = id;
            return behaviorDefinition;
        }

    }

}