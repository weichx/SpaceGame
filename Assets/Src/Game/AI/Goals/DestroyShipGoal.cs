using JetBrains.Annotations;
using Weichx.ReflectionAttributes;
using Weichx.ReflectionAttributes.Markers;

namespace SpaceGame.AI {

    public class DestroyShipGoal {

        [UsePropertyDrawer(typeof(EntitySelector))]
        public int entityId;

        [UsedImplicitly]
        public DestroyShipGoal() {
            this.entityId = -1;
        }
    }

}