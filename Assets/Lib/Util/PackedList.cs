namespace Weichx.Util {

    struct Index {

        public uint id;
        public ushort index;
        public ushort next;

        public Index(uint id, ushort next) {
            this.id = id;
            this.index = 0;
            this.next = next;
        }

    }

    class PackedList<T> where T : struct {

        private const int NewObjectIdAdd = 0x10000;
        private const int IndexMask = 0xffff;

        private ushort count;
        private T[] objects;
        private Index[] indices;
        private ushort freeListEnqueue;
        private ushort freeListDequeue;

        public PackedList() {
            count = 0;
            for (ushort i = 0; i < 16; i++) {
                indices[i] = new Index(i, (ushort) (i + 1));
            }
        }

        public int Count => count;

        public T[] RawList => objects;

        public bool Contains(uint id) {
            Index index = indices[id & IndexMask];
            return index.id == id && index.index != ushort.MaxValue;
        }

        public uint Add(uint id, T item) {
            Index index = indices[freeListDequeue];
            freeListDequeue = index.next;
            index.id += NewObjectIdAdd;
            index.index = count++;
            objects[index.index] = item;
            return index.id;
        }

        public void Remove(int id) {
//            Index index = indices[id & IndexMask];
//            T t = objects[index.index];
//            t = objects[--count];
//            indices[t.id & IndexMask].index = index.index;
//            index.index = ushort.MaxValue;
//            indices[freeListEnqueue].next = (ushort) (id & IndexMask);
//            freeListEnqueue = (ushort) (id & IndexMask);
        }

    }

}