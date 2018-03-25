using JetBrains.Annotations;
using Lib.Util;
using SpaceGame.Assets;

namespace Src.Game.Assets {

    public class BehaviorSet : GameAsset {

        public ListX<BehaviorDefinition> behaviors;

        [UsedImplicitly]
        private BehaviorSet(int id, string name) : base(id, name) {
            this.behaviors = new ListX<BehaviorDefinition>(4);
        }

        public BehaviorDefinition GetDefaultBehavior() {
            return behaviors[0];
        }

        [PublicAPI]
        public BehaviorDefinition AddBehaviorDefinition(BehaviorDefinition behaviorDefinition, int index = -1) {
            if (index == -1) {
                this.behaviors.Add(behaviorDefinition);
            }
            else {
                this.behaviors.Insert(index, behaviorDefinition);
            }
            behaviorDefinition.behaviorSetId = id;
            return behaviorDefinition;
        }

    }

}