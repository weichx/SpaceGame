using JetBrains.Annotations;
using Lib.Util;
using SpaceGame.Assets;

namespace Src.Game.Assets {

    public class BehaviorDefinition : GameAsset {

        public int behaviorSetId;
        public ListX<ActionDefinition> actions;

        [UsedImplicitly]
        private BehaviorDefinition() { }

        [UsedImplicitly]
        private BehaviorDefinition(int id, string name) : base(id, name) {
            behaviorSetId = -1;
            this.actions = new ListX<ActionDefinition>();
        }

        [PublicAPI]
        public ActionDefinition AddActionDefinition(ActionDefinition actionDefinition, int index = -1) {
            if (index == -1) {
                this.actions.Add(actionDefinition);
            }
            else {
                this.actions.Insert(index, actionDefinition);
            }
            actionDefinition.behaviorId = id;
            return actionDefinition;
        }

    }

}