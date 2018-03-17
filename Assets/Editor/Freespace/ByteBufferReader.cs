using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Freespace {

    public class ByteBufferReader {

        private int ptr;
        private readonly byte[] bytes;

        public ByteBufferReader(byte[] bytes) {
            ptr = 0;
            this.bytes = bytes;
        }

        public bool ReachedEOF {
            get { return ptr >= bytes.Length; }
        }

        public void SetPtr(int ptr) {
            this.ptr = Mathf.Clamp(ptr, 0, bytes.Length);
        }

        public int GetPtr() {
            return ptr;
        }

        public void FastForward(int size) {
            ptr = Mathf.Clamp(ptr + size, 0, bytes.Length);
        }

        public void Rewind(int size) {
            ptr = Mathf.Clamp(ptr - size, 0, bytes.Length);
        }

        public int PeekInt() {
            return BitConverter.ToInt32(bytes, ptr);
        }

        public int ReadInt() {
            int retn = BitConverter.ToInt32(bytes, ptr);
            ptr += sizeof(int);
            return retn;
        }

        public short ReadShort() {
            short retn = BitConverter.ToInt16(bytes, ptr);
            ptr += sizeof(short);
            return retn;
        }

        public ushort ReadUShort() {
            ushort retn = BitConverter.ToUInt16(bytes, ptr);
            ptr += sizeof(ushort);
            return retn;
        }

        public float ReadFloat() {
            float retn = BitConverter.ToSingle(bytes, ptr);
            ptr += sizeof(float);
            return retn;
        }

        public byte ReadByte() {
            byte retn = bytes[ptr];
            ptr += sizeof(byte);
            return retn;
        }

        public int[] ReadIntArray(int size) {
            int[] retn = new int[size];

            for (int i = 0; i < size; i++) {
                retn[i] = ReadInt();
            }

            return retn;
        }

        public float[] ReadFloatArray(int size) {
            float[] retn = new float[size];

            for (int i = 0; i < size; i++) {
                retn[i] = ReadFloat();
            }

            return retn;
        }

        public byte[] ReadByteArray(int size) {
            byte[] retn = new byte[size];

            for (int i = 0; i < size; i++) {
                retn[i] = bytes[ptr];
                ptr++;
            }

            return retn;
        }

        public Vector3[] ReadVector3Array(int size) {
            Vector3[] retn = new Vector3[size];

            for (int i = 0; i < size; i++) {
                retn[i] = ReadVector3();
            }

            return retn;
        }

        public Vector3 ReadVector3() {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public string ReadString() {
            int size = ReadInt();
            string retn = Encoding.UTF8.GetString(bytes, ptr, size);
            ptr += size;
            return retn;
        }

        public string ReadString(int size) {
            string retn = Encoding.UTF8.GetString(bytes, ptr, size);
            ptr += size;
            return retn;
        }

        public string[] ReadStringArray() {
            string[] retn = new string[ReadInt()];

            for (int i = 0; i < retn.Length; i++) {
                retn[i] = ReadString();
            }

            return retn;
        }

    }

}