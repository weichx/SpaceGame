using JetBrains.Annotations;
using SpaceGame.AI;
using Weichx.ReflectionAttributes;

namespace SpaceGame.Assets {

    public class ActionDefinition : GameAsset {

        public int behaviorId;

        [DefaultExpanded] [CreateOnReflect] public Consideration[] considerations;
        
        [UsedImplicitly]
        private ActionDefinition() { }

        [UsedImplicitly]
        private ActionDefinition(int id, string name) : base(id, name) {
            behaviorId = -1;
        }

        public BehaviorDefinition GetBehaviorDefinition() {
            return GameDatabase.ActiveInstance.FindAsset<BehaviorDefinition>(behaviorId);
        }
    }

}