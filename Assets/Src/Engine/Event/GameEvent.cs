using Weichx.Util;

namespace SpaceGame {

    public class GameEvent {

        public readonly float timestamp;
        
        protected GameEvent() {
            timestamp = GameTimer.Instance.GetRealTimestamp();
        }

    }

}