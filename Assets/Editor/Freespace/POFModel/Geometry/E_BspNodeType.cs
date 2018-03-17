namespace Freespace.POFModel.Geometry {

    internal enum BspTreeNodeType {

        EOF = 0,
        DefinePoints = 1,
        UntexturedPolygon = 2,
        TexturedPolygon = 3,
        SortNormal = 4,
        BoundingBox = 5

    }

}