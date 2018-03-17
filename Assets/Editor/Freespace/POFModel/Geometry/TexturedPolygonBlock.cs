using UnityEngine;

namespace Freespace.POFModel.Geometry {

    public class TexturedPolygonBlock {

        public readonly int size;
        public readonly Vector3 normal;
        public readonly Vector3 center;
        public readonly float radius;
        public readonly int vertexCount;
        public readonly int textureIndex;
        public PolygonVertex[] vertexMap;

        public TexturedPolygonBlock(ByteBufferReader reader) {
            reader.FastForward(sizeof(int));
            size = reader.ReadInt();
            normal = reader.ReadVector3();
            center = reader.ReadVector3();
            radius = reader.ReadFloat();
            vertexCount = reader.ReadInt();
            textureIndex = reader.ReadInt();
            
            vertexMap = new PolygonVertex[vertexCount];

            for (int i = 0; i < vertexCount; i++) {
                vertexMap[i].vertexIndex = reader.ReadUShort();
                vertexMap[i].normalIndex = reader.ReadUShort();
                vertexMap[i].u = reader.ReadFloat();
                vertexMap[i].v = reader.ReadFloat();
            }
        }

    }

}