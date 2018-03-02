namespace SpaceGame.Events {

  
    public class Evt_EntityArriving : EntityEvent {

        public readonly ArrivalType arrivalType;

        public Evt_EntityArriving(int entityId, ArrivalType arrivalType) : base(entityId) {
            this.arrivalType = arrivalType;
        }

    }

}