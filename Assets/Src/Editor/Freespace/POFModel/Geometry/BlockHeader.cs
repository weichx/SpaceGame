namespace Freespace.POFModel.Geometry {

    internal struct BlockHeader {

        public readonly int id;
        public readonly int size;

        public BlockHeader(ByteBufferReader reader) {
            id = reader.ReadInt();
            size = reader.ReadInt();
        }

        public static int Size {
            get { return sizeof(int) + sizeof(int); }
        }

    }

}