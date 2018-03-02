using System.Collections.Generic;
using UnityEngine;

namespace Freespace.POFModel {

    public class POFModel {

        public Header header;
        public ShieldData shieldData;
        public string[] textureList;
        public List<SubObject> subObjects;
        public List<SpecialPoint> specialPoints;
        public List<GunSlot> gunSlots;
        public List<MissileSlot> missileSlots;
        public List<TurretInfo> turrets;
        public List<DockPoint> dockPoints;
        public List<Thruster> thrusters;
        public List<PathInfo> aiPaths;
        public List<HullLight> hullLights;
        public List<EyePosition> eyePositions;

        public POFModel() {
            header = new Header();
            shieldData = new ShieldData();
            subObjects = new List<SubObject>();
            specialPoints = new List<SpecialPoint>();
            gunSlots = new List<GunSlot>();
            missileSlots = new List<MissileSlot>();
            turrets = new List<TurretInfo>();
            dockPoints = new List<DockPoint>();
            thrusters = new List<Thruster>();
            aiPaths = new List<PathInfo>();
            hullLights = new List<HullLight>();
            eyePositions = new List<EyePosition>();
        }

        public int TextureCount {
            get { return textureList.Length; }
        }

        public bool HasThrusters {
            get { return thrusters.Count > 0; }
        }
        
        public bool HasGunSlots {
            get { return gunSlots.Count > 0; }
        }

        public bool HasMissileSlots {
            get { return missileSlots.Count > 0; }
        }

        public bool HasShieldData {
            get { return shieldData != null && shieldData.faces != null && shieldData.vertices != null; }
        }

        public bool HasDebris {
            get { return header.debrisCountIndices.Length > 0; }
        }

        public int DetailLevels {
            get { return header.detailLevelIndices.Length;  }
        }
        
        public int DebrisCount {
            get { return header.debrisCountIndices.Length; }
        }

        public Vector3[] ShieldVertices {
            get { return shieldData.vertices; }
        }

        public Face[] ShieldFaces {
            get { return shieldData.faces; }
        }

        public SubObject GetDebrisPiece(int index) {
            return GetSubObjectByIndex(header.debrisCountIndices[index]);
        }

        public SubObject GetDetailLevel(int index) {
            return GetSubObjectByIndex(header.detailLevelIndices[index]);
        }

        public SubObject GetSubObjectByIndex(int index) {
            if (index < 0 || index >= subObjects.Count) {
                return null;
            }

            return subObjects[index];
        }

        public string GetTextureNameByIndex(int textureIndex) {
            if (textureIndex < 0 || textureIndex >= textureList.Length) return null;
            return textureList[textureIndex];
        }

        public Vector3 GetBoundingBoxSize() {
            return header.maxBounding - header.minBounding;
        }

    }

}