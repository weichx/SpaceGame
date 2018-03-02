using UnityEngine;

namespace Freespace.POFModel {

    public enum TurretType {

        Gun, Missile

    }

    public class TurretInfo {

        public TurretType type;
        public int parentSubObjectIndex;
        public int rotationBaseSubObjectIndex;
        public Vector3 turretNormal;
        public Vector3[] firingPoints;

        public TurretInfo(TurretType type) {
            this.type = type;
        }

    }
    
//    public class GunTurret {
//        //For multipart turrets, parentSubObjectIndex is the "barrel" of a turret,
//        //and the firing points will be in this sobj's axial frame.
//        //rotationBaseSubObjectIndex is the "base" of a turret, which the barrel will rotate with.
//        //For single-part turrets, parentSubObjectIndex == rotationBaseSubObjectIndex.
//        public int parentSubObjectIndex;
//        public int rotationBaseSubObjectIndex;
//        public Vector3 turretNormal;
//        public Vector3[] firingPoints;
//
//    }
//
//    public class MissileTurret {
//
//        //For multipart turrets, parentSubObjectIndex is the "barrel" of a turret,
//        //and the firing points will be in this sobj's axial frame.
//        //rotationBaseSubObjectIndex is the "base" of a turret, which the barrel will rotate with.
//        //For single-part turrets, parentSubObjectIndex == rotationBaseSubObjectIndex.
//        public int parentSubObjectIndex;
//        public int rotationBaseSubObjectIndex;
//        public Vector3 turretNormal;
//        public Vector3[] firingPoints;
//
//    }

}