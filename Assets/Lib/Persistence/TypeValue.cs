namespace Weichx.Persistence {

    using System;


        [Flags]
        public enum TypeValue {

            Null = 0,
            Unknown = 1 << 0,
            Type = 1 << 1,
            String = 1 << 2,
            Boolean = 1 << 3,
            Float = 1 << 4,
            Double = 1 << 5,
            Decimal = 1 << 6,
            Integer = 1 << 7,
            UnsignedInteger = 1 << 8,
            SignedByte = 1 << 9,
            Byte = 1 << 10,
            Short = 1 << 11,
            UnsignedShort = 1 << 12,
            Long = 1 << 13,
            UnsignedLong = 1 << 14,
            Char = 1 << 15,
            Enum = 1 << 16,
            Array = 1 << 17,
            UnityObject = 1 << 18,
            Vector2 = 1 << 19,
            Vector3 = 1 << 20,
            Vector4 = 1 << 21,
            Quaternion = 1 << 22,
            Color = 1 << 23,
            List = 1 << 24,
            Stack = 1 << 25,
            Queue = 1 << 26,
            Dictionary = 1 << 27,
            HashSet = 1 << 28,
            Struct = 1 << 29,
            Class = 1 << 30,
        
            Primitive = Boolean | Float | Double | Decimal | Integer | UnsignedInteger | SignedByte | Byte | Short | UnsignedShort | Long | UnsignedLong | Char,
            PrimitiveLike = Primitive | String | Enum | Null,
            PrimitiveStruct =  Vector2 | Vector3 | Vector4 | Quaternion | Color,
            Collection = List | Array | Stack | Queue | HashSet | Dictionary,
            KnownType = PrimitiveLike | PrimitiveStruct | Collection,
            ReferenceType = Array | Collection | Class,
            SimpleKnownType = PrimitiveLike | PrimitiveStruct,
            ArrayLike = List | Array | Stack | Queue,
            StructLike = Vector2 | Vector3 | Vector4 | Quaternion | Color | Struct
        

        }

    

}