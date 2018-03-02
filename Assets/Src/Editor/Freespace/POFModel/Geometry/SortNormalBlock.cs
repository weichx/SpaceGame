using UnityEngine;

namespace Freespace.POFModel.Geometry {

    public class SortNormalBlock {

        public readonly int size;
        public readonly int reserved;
        public readonly int frontOffset;
        public readonly int backOffset;
        public readonly int preListOffset;
        public readonly int postListOffset;
        public readonly int onlineOffset;
        public readonly Vector3 planeNormal;
        public readonly Vector3 planePoint;
        public readonly Vector3 boundingBoxMin;
        public readonly Vector3 boundingBoxMax;

        public SortNormalBlock(ByteBufferReader reader) {
            reader.FastForward(sizeof(int));
            size = reader.ReadInt();
            planeNormal = reader.ReadVector3();
            planePoint = reader.ReadVector3();
            reserved = reader.ReadInt();
            frontOffset = reader.ReadInt();
            backOffset = reader.ReadInt();
            preListOffset = reader.ReadInt();
            postListOffset = reader.ReadInt();
            onlineOffset = reader.ReadInt();
            boundingBoxMin = reader.ReadVector3();
            boundingBoxMax = reader.ReadVector3();
        }

    }

}