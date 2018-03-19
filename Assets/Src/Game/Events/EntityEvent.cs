using SpaceGame.Engine;

namespace SpaceGame.Events {

    public class EntityEvent : GameEvent {

        public readonly int entityId;

        public EntityEvent(int entityId) {
            this.entityId = entityId;
        }

        public Entity Entity => GameData.Instance.entityMap[entityId];

    }

}