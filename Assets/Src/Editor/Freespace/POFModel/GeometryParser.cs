using System;
using UnityEngine;
using System.Collections.Generic;

namespace Freespace.POFModel.Geometry {

    internal class PolygonGroup {

        public readonly List<Polygon> polygons;
        public readonly int textureIndex;

        public PolygonGroup(int textureIndex) {
            this.textureIndex = textureIndex;
            polygons = new List<Polygon>();
        }

    }

    public class GeometryParser {

        private readonly ByteBufferReader reader;
        private readonly PointDefinitions points;
        private readonly List<PolygonGroup> polygonGroups;

        public GeometryParser(byte[] bspBytes) {
            reader = new ByteBufferReader(bspBytes);
            points = new PointDefinitions();
            polygonGroups = new List<PolygonGroup>();
            ParseBSPTree();
        }

        public KeyValuePair<Mesh, int[]> GetMeshAndTextureIndices() {
            List<PolygonGroup> groups = polygonGroups.FindAll((polygonGroup) => polygonGroup.polygons.Count > 0);
            Mesh mesh = BuildMesh2(groups);
            int[] textureIndices = new int[groups.Count];

            for (int i = 0; i < groups.Count; i++) {
                textureIndices[i] = groups[i].textureIndex;
            }

            return new KeyValuePair<Mesh, int[]>(mesh, textureIndices);
        }

        private static Mesh BuildMesh2(List<PolygonGroup> polygonGroups) {
            int triangleCount = 0;
            List<int[]> tris = new List<int[]>();
            Dictionary<Vector3, List<FinalVertex>> lookupPositions = new Dictionary<Vector3, List<FinalVertex>>();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvCoords = new List<Vector2>();

            for (int i = 0; i < polygonGroups.Count; i++) {
                PolygonGroup group = polygonGroups[i];
                List<Polygon> polygons = group.polygons;
                int[] triangles = new int[polygons.Count * 3];
                tris.Add(triangles);
                int count = 0;

                for (int j = 0; j < polygons.Count; j++) {
                    for (int k = 0; k < polygons[j].vertices.Length; k++) {
                        FinalVertex vertex = new FinalVertex(polygons[j].vertices[k], -1);

                        FinalVertex match = null;
                        List<FinalVertex> matchList;

                        if (lookupPositions.TryGetValue(vertex.position, out matchList)) {
                            match = FindMatchingVertex(vertex, matchList);
                        }
                        else {
                            matchList = new List<FinalVertex>();
                            matchList.Add(vertex);
                            lookupPositions.Add(vertex.position, matchList);
                        }

                        if (match != null) {
                            triangles[count] = match.triangleIndex;
                        }
                        else {
                            vertex.triangleIndex = triangleCount++;
                            triangles[count] = vertex.triangleIndex;
                            vertices.Add(vertex.position);
                            normals.Add(vertex.normal);
                            uvCoords.Add(vertex.uvCoord);
                        }

                        count++;
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.subMeshCount = polygonGroups.Count;

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvCoords);
            mesh.SetNormals(normals);

            for (int i = 0; i < polygonGroups.Count; i++) {
                mesh.SetTriangles(tris[i], i);
            }

            return mesh;
        }

        private static Mesh BuildMesh(List<Polygon> groupedPolygons) {
            if (groupedPolygons.Count == 0) return null;
            FinalVertex[] subObjectVertices = new FinalVertex[groupedPolygons.Count * 3];
            int[] triangles = new int[groupedPolygons.Count * 3];
            int triangleCount = 0;
            int vertIdx = 0;

            for (int i = 0; i < groupedPolygons.Count; i++) {
                subObjectVertices[vertIdx++] = new FinalVertex(groupedPolygons[i].vertices[0], -1);
                subObjectVertices[vertIdx++] = new FinalVertex(groupedPolygons[i].vertices[1], -1);
                subObjectVertices[vertIdx++] = new FinalVertex(groupedPolygons[i].vertices[2], -1);
            }

            Dictionary<Vector3, List<FinalVertex>> lookupPositions = new Dictionary<Vector3, List<FinalVertex>>();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvCoords = new List<Vector2>();

            for (int i = 0; i < subObjectVertices.Length; i++) {
                FinalVertex vertex = subObjectVertices[i];
                FinalVertex match = null;
                List<FinalVertex> matchList;

                if (lookupPositions.TryGetValue(vertex.position, out matchList)) {
                    match = FindMatchingVertex(vertex, matchList);
                }
                else {
                    matchList = new List<FinalVertex>();
                    matchList.Add(vertex);
                    lookupPositions.Add(vertex.position, matchList);
                }

                if (match != null) {
                    triangles[i] = match.triangleIndex;
                }
                else {
                    vertex.triangleIndex = triangleCount++;
                    triangles[i] = vertex.triangleIndex;
                    vertices.Add(subObjectVertices[i].position);
                    normals.Add(subObjectVertices[i].normal);
                    uvCoords.Add(subObjectVertices[i].uvCoord);
                }
            }

            Mesh mesh = new Mesh();

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvCoords);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0); // = triangles;
            return mesh;
        }

        private static FinalVertex FindMatchingVertex(FinalVertex input, List<FinalVertex> vertices) {
            for (int i = 0; i < vertices.Count; i++) {
                FinalVertex compare = vertices[i];

                if (compare.position == input.position && compare.normal == input.normal) {
                    return compare;
                }
            }

            return null;
        }

        private void ParseBSPTree(int offset = 0) {
            while (true) {
                int blockSize;
                reader.SetPtr(offset);
                BspTreeNodeType blockType = (BspTreeNodeType) reader.PeekInt();

                switch (blockType) {
                    case BspTreeNodeType.EOF:
                        break;
                    case BspTreeNodeType.DefinePoints:
                        blockSize = points.Read(reader);
                        offset = offset + blockSize;
                        continue;
                    case BspTreeNodeType.UntexturedPolygon:
                        blockSize = TranslateFlatPolygon();
                        offset = offset + blockSize;
                        continue;
                    case BspTreeNodeType.TexturedPolygon:
                        blockSize = TranslateTexturedPolygon();
                        offset = offset + blockSize;
                        continue;
                    case BspTreeNodeType.SortNormal:
                        InterpretSortNormalBlock(offset);
                        break;
                    case BspTreeNodeType.BoundingBox:
                        blockSize = new BlockHeader(reader).size;
                        offset = offset + blockSize;
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            }
        }

        private int TranslateFlatPolygon() {
            FlatPolygonBlock polygonBlock = new FlatPolygonBlock(reader);
            Polygon polygon = new Polygon();
            
            Vertex[] vertices = new Vertex[polygonBlock.vertexCount];

            for (int i = 0; i < polygonBlock.vertexCount; i++) {
                int vertexIndex = polygonBlock.vertexMap[i].vertexIndex;
                int normalIndex = polygonBlock.vertexMap[i].normalIndex;
                vertices[i].point = points.vertexData[vertexIndex].vertex;
                vertices[i].normal = points.normals[normalIndex];
                vertices[i].uvCoords = new Vector2(0, 0);
            }

            polygon.vertices = vertices;
            polygon.normal = polygonBlock.normal;
            polygon.centroid = polygonBlock.center;
            AddPolygonToGroup(polygon, -1);

            return polygonBlock.size;
        }

        private int TranslateTexturedPolygon() {
            TexturedPolygonBlock polygonBlock = new TexturedPolygonBlock(reader);

            Polygon polygon = new Polygon();
            Vertex[] vertices = new Vertex[polygonBlock.vertexCount];

            for (int i = 0; i < polygonBlock.vertexCount; i++) {
                int vertexIndex = polygonBlock.vertexMap[i].vertexIndex;
                int normalIndex = polygonBlock.vertexMap[i].normalIndex;
                vertices[i].point = points.vertexData[vertexIndex].vertex;
                vertices[i].normal = points.normals[normalIndex];
                vertices[i].uvCoords = new Vector2(polygonBlock.vertexMap[i].u, 1f - polygonBlock.vertexMap[i].v);
            }

            polygon.vertices = vertices;
            polygon.normal = polygonBlock.normal;
            polygon.centroid = polygonBlock.center;
            AddPolygonToGroup(polygon, polygonBlock.textureIndex);

            return polygonBlock.size;
        }

        private void InterpretSortNormalBlock(int offset) {
            SortNormalBlock sortNormalBlock = new SortNormalBlock(reader);

            if (sortNormalBlock.frontOffset != 0) {
                ParseBSPTree(offset + sortNormalBlock.frontOffset);
            }

            if (sortNormalBlock.backOffset != 0) {
                ParseBSPTree(offset + sortNormalBlock.backOffset);
            }

            if (sortNormalBlock.preListOffset != 0) {
                ParseBSPTree(offset + sortNormalBlock.preListOffset);
            }

            if (sortNormalBlock.postListOffset != 0) {
                ParseBSPTree(offset + sortNormalBlock.postListOffset);
            }

            if (sortNormalBlock.onlineOffset != 0) {
                ParseBSPTree(offset + sortNormalBlock.onlineOffset);
            }
        }

        private void AddPolygonToGroup(Polygon polygon, int textureIndex) {
            PolygonGroup group = GetGroupForTextureIndex(textureIndex);

            // quad 1 2 3 4
            //    ==>
            //triangle 1 2 3
            //triangle 3 4 1
            if (polygon.vertices.Length != 3) {
                if (polygon.vertices.Length == 4) {
                    Polygon p1 = new Polygon();
                    Polygon p2 = new Polygon();
                    p1.vertices = new Vertex[3];
                    p2.vertices = new Vertex[3];
                    p1.vertices[0] = polygon.vertices[0];
                    p1.vertices[1] = polygon.vertices[1];
                    p1.vertices[2] = polygon.vertices[2];
                    p2.vertices[0] = polygon.vertices[2];
                    p2.vertices[1] = polygon.vertices[3];
                    p2.vertices[2] = polygon.vertices[0];
                    p1.textureIndex = polygon.textureIndex;
                    p2.textureIndex = polygon.textureIndex;
                    //todo -- recalculate normal if needed
                    group.polygons.Add(p1);
                    group.polygons.Add(p2);
                }
                else {

                    Vector3 averageNormal = Vector3.zero;
                    Vector2 averageUVs = Vector2.zero;
                    
                    for (int i = 0; i < polygon.vertices.Length; i++) {
                        averageUVs += polygon.vertices[i].uvCoords;
                        averageNormal += polygon.vertices[i].normal;
                    }

                    averageUVs = averageUVs / polygon.vertices.Length;
                    averageNormal = averageNormal / polygon.vertices.Length;
                    
                    for (int i = 0; i < polygon.vertices.Length; i++) {
                        Polygon p = new Polygon();
                        p.normal = polygon.normal;
                        p.textureIndex = polygon.textureIndex;
                        p.vertices = new Vertex[3];
                        p.vertices[2] = polygon.vertices[i];
                        p.vertices[1] = new Vertex();
                        p.vertices[1].point = polygon.centroid;
                        p.vertices[1].uvCoords = averageUVs;
                        p.vertices[1].normal = averageNormal;
                        p.vertices[0] = polygon.vertices[(i + 1) % polygon.vertices.Length];
                        group.polygons.Add(p);
                    }

                }
            }
            else {
                group.polygons.Add(polygon);
            }
        }

        private PolygonGroup GetGroupForTextureIndex(int textureIndex) {
            for (int i = 0; i < polygonGroups.Count; i++) {
                if (polygonGroups[i].textureIndex == textureIndex) return polygonGroups[i];
            }

            PolygonGroup group = new PolygonGroup(textureIndex);
            polygonGroups.Add(group);
            return group;
        }

    }

}