using JetBrains.Annotations;
using Weichx.ReflectionAttributes;
using Weichx.ReflectionAttributes.Markers;

namespace SpaceGame.AI {

    public class DestroyFlightGroupGoal {
        
        [UsePropertyDrawer(typeof(FlightGroupSelector))]
        public int flightGroupId;

        [UsedImplicitly]
        public DestroyFlightGroupGoal() {
            this.flightGroupId = -1;
        }


    }

}