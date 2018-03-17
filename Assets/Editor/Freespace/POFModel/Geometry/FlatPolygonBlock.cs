using UnityEngine;

namespace Freespace.POFModel.Geometry {

    internal class FlatPolygonBlock {

        public readonly int size;
        public readonly Vector3 normal;
        public readonly Vector3 center;
        public readonly float radius;
        public readonly int vertexCount;
        public readonly byte red, green, blue, pad;
        public readonly PolygonVertex[] vertexMap;

        public FlatPolygonBlock(ByteBufferReader reader) {
            reader.FastForward(sizeof(int));
            size = reader.ReadInt();
            normal = reader.ReadVector3();
            center = reader.ReadVector3();
            radius = reader.ReadFloat();
            vertexCount = reader.ReadInt();
            red = reader.ReadByte();
            green = reader.ReadByte();
            blue = reader.ReadByte();
            pad = reader.ReadByte();
            vertexMap = new PolygonVertex[vertexCount];

            for (int i = 0; i < vertexCount; i++) {
                vertexMap[i].vertexIndex = reader.ReadUShort();
                vertexMap[i].normalIndex = reader.ReadUShort();
            }
        }

    }

}