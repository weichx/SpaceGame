using System.Collections.Generic;
using JetBrains.Annotations;
using Weichx.ReflectionAttributes;

namespace SpaceGame.Assets {

    public class ShipTypeGroup : GameAsset {

        public ShipCategory shipCategory;
        
        [CreateOnReflect][UsedImplicitly] public List<ShipType> ships;

        [UsedImplicitly]
        private ShipTypeGroup() { }

        [UsedImplicitly]
        private ShipTypeGroup(int id, string name) : base(id, name) {
            this.ships = new List<ShipType>();
            this.shipCategory = ShipCategory.SmallFighter;
        }

        [PublicAPI]
        public ShipType AddShipDefinition(ShipType shipType, int index = -1) {
            if (index == -1) {
                this.ships.Add(shipType);
            }
            else {
                this.ships.Insert(index, shipType);
            }
            shipType.shipGroupId = id;
            return shipType;
        }

    }

}