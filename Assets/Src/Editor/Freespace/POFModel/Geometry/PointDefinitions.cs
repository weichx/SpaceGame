using System.Collections.Generic;
using UnityEngine;

namespace Freespace.POFModel.Geometry {

    public class PointDefinitions {

        private int vertexCount;
        private int[] normalCounts;
        public VertexData[] vertexData;
        public List<Vector3> normals;

        public int Read(ByteBufferReader reader) {
            BlockHeader header = new BlockHeader(reader);
            vertexCount = reader.ReadInt();
            reader.ReadInt(); //normal count
            reader.ReadInt(); //offset
            vertexData = new VertexData[vertexCount];
            normalCounts = new int[vertexCount];
            normals = new List<Vector3>();

            for (int i = 0; i < normalCounts.Length; i++) {
                normalCounts[i] = reader.ReadByte();
            }

            for (int i = 0; i < vertexCount; i++) {
                int normalCountForVertex = normalCounts[i];
                vertexData[i].vertex = reader.ReadVector3();
                vertexData[i].normals = new Vector3[normalCountForVertex];

                for (int j = 0; j < normalCountForVertex; j++) {
                    Vector3 normal = reader.ReadVector3();
                    vertexData[i].normals[j] = normal;
                    normals.Add(normal);
                }
            }

            return header.size;
        }

    }

}