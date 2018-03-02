using System.IO;
using UnityEngine;

namespace Freespace.POFModel {

    public class POFModelParser {

        private ByteBufferReader reader;
        private readonly POFModel model;

        private POFModelParser() {
            model = new POFModel();
        }
        
        private POFModel ParseFile(string filePath) {
            byte[] bytes = File.ReadAllBytes(filePath);

            reader = new ByteBufferReader(bytes);
            string signature = reader.ReadString(4);
            int version = reader.ReadInt();

            if (signature != "PSPO") {
                Debug.LogError("Invalid POF file signature: " + signature);
                return null;
            }

            if (version < 2117) {
                Debug.LogError("Invalid POF file version: " + version);
                return null;
            }

            while (!reader.ReachedEOF) {
                string blockType = reader.ReadString(4);
                int blockSize = reader.ReadInt();
                int startPtr = reader.GetPtr();

                switch (blockType) {
                    case "TXTR":
                        ParseTextureSection();
                        break;
                    case "HDR2":
                        ParseHeaderSection();
                        break;
                    case "OBJ2":
                        ParseSubObjectSection();
                        break;
                    case "SPCL":
                        ParseSpecialPointSection();
                        break;
                    case "GPNT":
                        ParseGunPointSection();
                        break;
                    case "MPNT":
                        ParseMissilePointSection();
                        break;
                    case "TGUN":
                        ParseTurretGunSection();
                        break;
                    case "TMIS":
                        ParseTurretMissileSection();
                        break;
                    case "DOCK":
                        ParseDockPointSection();
                        break;
                    case "FUEL":
                        ParseFuelSection();
                        break;
                    case "SHLD":
                        ParseShieldSection();
                        break;
                    case "EYE ":
                        ParseEyeSection();
                        break;
                    case "ACEN":
                        ParseAutoCenterSection();
                        break;
                    case "INSG":
                        ParseInsigniaSection();
                        break;
                    case "PATH":
                        ParsePathSection();
                        break;
                    case "GLOW":
                        ParseGlowSection();
                        break;
                    case "SLDC":
                        ParseShieldCollisionBSPSection();
                        break;
                    case "PINF":
                        ParsePOFInfoSection(blockSize);
                        break;

                    default:
                        Debug.LogError("UNKNOWN BLOCK TYPE " + blockType);
                        return null;
                }

                AssertSectionFullyRead(blockType, startPtr, blockSize);
            }

            return model;
        }

           private void ParseTextureSection() {
            int textureCount = reader.ReadInt();
            model.textureList = new string[textureCount];

            for (int i = 0; i < textureCount; i++) {
                model.textureList[i] = reader.ReadString();
            }
        }

        private void ParseHeaderSection() {
            Header header = model.header;
            header.maxRadius = reader.ReadFloat();
            header.objectFlags = reader.ReadInt();
            header.subObjectCount = reader.ReadInt();
            header.minBounding = reader.ReadVector3();
            header.maxBounding = reader.ReadVector3();
            int detailLevels = reader.ReadInt();

            header.detailLevelIndices = new int[detailLevels];

            for (int i = 0; i < detailLevels; i++) {
                header.detailLevelIndices[i] = reader.ReadInt();
            }

            int debrisCount = reader.ReadInt();
            header.debrisCountIndices = new int[debrisCount];

            for (int i = 0; i < debrisCount; i++) {
                header.debrisCountIndices[i] = reader.ReadInt();
            }

            header.mass = reader.ReadFloat();
            header.centerOfMass = reader.ReadVector3();
            header.momentOfInertia = reader.ReadFloatArray(9);
            int crossSectionCount = reader.ReadInt();
            if (crossSectionCount < 0) crossSectionCount = 0;
            header.crossSections = new CrossSection[crossSectionCount];

            for (int i = 0; i < header.crossSections.Length; i++) {
                header.crossSections[i].depth = reader.ReadFloat();
                header.crossSections[i].radius = reader.ReadFloat();
            }

            header.muzzleLights = new MuzzleLight[reader.ReadInt()];

            for (int i = 0; i < header.muzzleLights.Length; i++) {
                header.muzzleLights[i].location = reader.ReadVector3();
                header.muzzleLights[i].lightType = reader.ReadInt();
            }
        }

        private void ParseSubObjectSection() {
            SubObject subObject = new SubObject();
            subObject.subModelNumber = reader.ReadInt();
            subObject.radius = reader.ReadFloat();
            subObject.submodelParent = reader.ReadInt();
            subObject.offset = reader.ReadVector3();
            subObject.geometricCenter = reader.ReadVector3();
            subObject.boundingBoxMin = reader.ReadVector3();
            subObject.boundingBoxMax = reader.ReadVector3();
            subObject.submodelName = reader.ReadString();
            subObject.properties = reader.ReadString();
            subObject.movementType = reader.ReadInt();
            subObject.movementAxis = reader.ReadInt();
            subObject.reserved = reader.ReadInt();
            subObject.bspData = reader.ReadByteArray(reader.ReadInt());
            subObject.reserved = 0;

            if (subObject.submodelName == string.Empty) {
                subObject.submodelName = "SubObject " + subObject.subModelNumber;
            }
            
            model.subObjects.Add(subObject);
        }

        private void ParseSpecialPointSection() {
            int specialPointCount = reader.ReadInt();

            for (int i = 0; i < specialPointCount; i++) {
                SpecialPoint specialPoint = new SpecialPoint();
                specialPoint.name = reader.ReadString();
                specialPoint.properties = reader.ReadString();
                specialPoint.point = reader.ReadVector3();
                specialPoint.radius = reader.ReadFloat();
                model.specialPoints.Add(specialPoint);
            }
        }

        // A "slot" is what you see in the loadout screen. Primaries have a max of 2
        // and secondaries of 3 for player-flyable ships.
        // "Guns" are the actual number of barrels and hence projectiles you'll get when you press the trigger.
        // There is likely no practical max.
        private void ParseGunPointSection() {
            int slots = reader.ReadInt();

            for (int i = 0; i < slots; i++) {
                GunSlot slot = new GunSlot();
                model.gunSlots.Add(slot);
                slot.gunPoints = new PositionNormal[reader.ReadInt()];

                for (int j = 0; j < slot.gunPoints.Length; j++) {
                    slot.gunPoints[j] = new PositionNormal(reader.ReadVector3(), reader.ReadVector3());
                }
            }
        }

        private void ParseMissilePointSection() {
            int slots = reader.ReadInt();

            for (int i = 0; i < slots; i++) {
                MissileSlot slot = new MissileSlot();
                model.missileSlots.Add(slot);
                slot.missilePoints = new PositionNormal[reader.ReadInt()];

                for (int j = 0; j < slot.missilePoints.Length; j++) {
                    slot.missilePoints[j] = new PositionNormal(reader.ReadVector3(), reader.ReadVector3());
                }
            }
        }

        private void ParseTurretGunSection() {
            int bankCount = reader.ReadInt();

            for (int i = 0; i < bankCount; i++) {
                TurretInfo turret = new TurretInfo(TurretType.Gun);
                turret.parentSubObjectIndex = reader.ReadInt();
                turret.rotationBaseSubObjectIndex = reader.ReadInt();
                turret.turretNormal = reader.ReadVector3();
                turret.firingPoints = reader.ReadVector3Array(reader.ReadInt());
                model.turrets.Add(turret);
            }
        }

        private void ParseTurretMissileSection() {
            int bankCount = reader.ReadInt();

            for (int i = 0; i < bankCount; i++) {
                TurretInfo turret = new TurretInfo(TurretType.Missile);
                turret.parentSubObjectIndex = reader.ReadInt();
                turret.rotationBaseSubObjectIndex = reader.ReadInt();
                turret.turretNormal = reader.ReadVector3();
                turret.firingPoints = reader.ReadVector3Array(reader.ReadInt());
                model.turrets.Add(turret);
            }
        }

        // Note: Properties… if $name= found, then this is name.
        // If name is cargo then this is a cargo bay.
        private void ParseDockPointSection() {
            int dockPointCount = reader.ReadInt();

            for (int i = 0; i < dockPointCount; i++) {
                DockPoint dockPoint = new DockPoint();
                dockPoint.properties = reader.ReadString();
                dockPoint.pathNumber = reader.ReadIntArray(reader.ReadInt());

                int pointCount = reader.ReadInt();
                dockPoint.points = new PositionNormal[pointCount];

                for (int j = 0; j < pointCount; j++) {
                    dockPoint.points[j].point = reader.ReadVector3();
                    dockPoint.points[j].normal = reader.ReadVector3();
                }

                model.dockPoints.Add(dockPoint);
            }
        }

        private void ParseFuelSection() {
            int thrusterCount = reader.ReadInt();

            for (int i = 0; i < thrusterCount; i++) {
                Thruster thruster = new Thruster();
                thruster.glows = new ThrusterGlow[reader.ReadInt()];
                thruster.properties = reader.ReadString();
                model.thrusters.Add(thruster);

                for (int j = 0; j < thruster.glows.Length; j++) {
                    thruster.glows[j].position = reader.ReadVector3();
                    thruster.glows[j].normal = reader.ReadVector3();
                    thruster.glows[j].radius = reader.ReadFloat();
                }
            }
        }

        private void ParseShieldSection() {
            ShieldData shieldData = model.shieldData;
            shieldData.vertices = reader.ReadVector3Array(reader.ReadInt());
            shieldData.faces = new Face[reader.ReadInt()];

            for (int i = 0; i < shieldData.faces.Length; i++) {
                Face face = new Face();
                face.normal = reader.ReadVector3();
                face.vertexIndices = reader.ReadIntArray(3);
                face.neighborIndices = reader.ReadIntArray(3);
                shieldData.faces[i] = face;
            }
        }

        private void ParseEyeSection() {
            int eyePositionCount = reader.ReadInt();

            for (int i = 0; i < eyePositionCount; i++) {
                EyePosition eyePosition = new EyePosition();
                eyePosition.parentSubObjectIndex = reader.ReadInt();
                eyePosition.position = reader.ReadVector3();
                eyePosition.normal = reader.ReadVector3();
                model.eyePositions.Add(eyePosition);
            }
        }

        private void ParseAutoCenterSection() {
            reader.ReadVector3();
        }

        private void ParseInsigniaSection() {
           
            int insigniaCount = reader.ReadInt();

            for (int i = 0; i < insigniaCount; i++) {
                Insignia insignia = new Insignia();
                insignia.detailLevel = reader.ReadInt();
                insignia.faces = new InsigniaFace[reader.ReadInt()];
                insignia.vertices = new Vector3[reader.ReadInt()];

                for (int j = 0; j < insignia.vertices.Length; j++) {
                    insignia.vertices[j] = reader.ReadVector3();
                }

                insignia.offset = reader.ReadVector3();

                for (int j = 0; j < insignia.faces.Length; j++) {
                    InsigniaFace face = new InsigniaFace();
                    face.vertexIndex0 = reader.ReadInt();
                    face.u0 = reader.ReadFloat();
                    face.v0 = reader.ReadFloat();
                    face.vertexIndex1 = reader.ReadInt();
                    face.u1 = reader.ReadFloat();
                    face.v1 = reader.ReadFloat();
                    face.vertexIndex2 = reader.ReadInt();
                    face.u2 = reader.ReadFloat();
                    face.v2 = reader.ReadFloat();
                    insignia.faces[j] = face;
                }
            }
        }

        private void ParsePathSection() {
            int pathCount = reader.ReadInt();

            for (int i = 0; i < pathCount; i++) {
                PathInfo pathInfo = new PathInfo();
                pathInfo.name = reader.ReadString();
                pathInfo.parentName = reader.ReadString();
                PathVertex[] points = new PathVertex[reader.ReadInt()];

                for (int j = 0; j < points.Length; j++) {
                    PathVertex point = new PathVertex();
                    point.position = reader.ReadVector3();
                    point.radius = reader.ReadFloat();
                    point.subObjectIndices = reader.ReadIntArray(reader.ReadInt());
                    points[j] = point;
                }

                pathInfo.points = points;
                model.aiPaths.Add(pathInfo);
            }
        }

        private void ParseGlowSection() {
            int glowBankCount = reader.ReadInt();

            for (int i = 0; i < glowBankCount; i++) {
                HullLight light = new HullLight();
                light.displayTime = reader.ReadInt();
                light.onTime = reader.ReadInt();
                light.offTime = reader.ReadInt();
                light.parentIndex = reader.ReadInt();
                light.lod = reader.ReadInt();
                light.type = reader.ReadInt();
                light.lights = new HullLightPoint[reader.ReadInt()];
                light.properties = reader.ReadString();

                for (int j = 0; j < light.lights.Length; j++) {
                    light.lights[j].point = reader.ReadVector3();
                    light.lights[j].normal = reader.ReadVector3();
                    light.lights[j].radius = reader.ReadFloat();
                }

                model.hullLights.Add(light);
            }
        }

        private void ParseShieldCollisionBSPSection() {
            model.shieldData.collisionBSP = reader.ReadByteArray(reader.ReadInt());
        }

        private void ParsePOFInfoSection(int size) {
            reader.ReadString(size);
        }

        private void AssertSectionFullyRead(string sectionName, int startPtr, int size) {
            if (reader.GetPtr() - startPtr != size) {
                Debug.Log("Failed to fully read section: " + sectionName);
            }
        }
        public static POFModel Parse(string filePath) {
            return new POFModelParser().ParseFile(filePath);
        }
    }

}