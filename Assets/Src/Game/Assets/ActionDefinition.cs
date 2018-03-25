using JetBrains.Annotations;
using SpaceGame.Assets;

namespace Src.Game.Assets {

    public class ActionDefinition : GameAsset {

        public int behaviorId;

        [UsedImplicitly]
        private ActionDefinition() { }

        [UsedImplicitly]
        private ActionDefinition(int id, string name) : base(id, name) {
            behaviorId = -1;
        }

    }

}