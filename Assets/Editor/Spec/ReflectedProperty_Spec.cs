using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Weichx.EditorReflection;

namespace Src.Editor.Spec {

    [TestFixture]
    public class ReflectedProperty_Spec_Assignment {

        class TestThing {

            public float x = 1f;
            public float y = 2f;
            public float z = 3f;

        }

        [Test]
        public void ReadValues() {
            TestThing thing = new TestThing();
            ReflectedObject root = new ReflectedObject(thing);
            Assert.AreEqual(root["x"].Value, 1f);
            Assert.AreEqual(root["y"].Value, 2f);
            Assert.AreEqual(root["z"].Value, 3f);
        }

        [Test]
        public void WriteValues() {
            TestThing thing = new TestThing();
            ReflectedObject root = new ReflectedObject(thing);
            root["x"].Value = 4f;
            root["y"].Value = 5f;
            Assert.AreEqual(root["x"].Value, 4f);
            Assert.AreEqual(root["y"].Value, 5f);
            Assert.AreEqual(root["z"].Value, 3f);
        }

        class TestVector {

            public Vector3 vec = new Vector3(1f, 2f, 3f);

        }

        [Test]
        public void ReadStruct() {
            ReflectedObject root = new ReflectedObject(new TestVector());
            Assert.AreEqual(root["vec"]["x"].Value, 1f);
            Assert.AreEqual(root["vec"]["y"].Value, 2f);
            Assert.AreEqual(root["vec"]["z"].Value, 3f);
        }

        [Test]
        public void WriteStruct() {
            TestVector target = new TestVector();
            target.vec = new Vector3(4f, 5f, 6f);
            ReflectedObject root = new ReflectedObject(target);
            Assert.AreEqual(root["vec"]["x"].Value, 4f);
            Assert.AreEqual(root["vec"]["y"].Value, 5f);
            Assert.AreEqual(root["vec"]["z"].Value, 6f);
            root["vec"]["x"].Value = 7f;
            root["vec"]["y"].Value = 8f;
            root["vec"]["z"].Value = 9f;
            Assert.AreEqual(root["vec"]["x"].Value, 7f);
            Assert.AreEqual(root["vec"]["y"].Value, 8f);
            Assert.AreEqual(root["vec"]["z"].Value, 9f);
        }

        class TestNested {

            public TestVector testVec;
            public string str;

        }

        [Test]
        public void NestedObject() {
            TestNested target = new TestNested();
            target.str = "somevalue";
            target.testVec = new TestVector();
            ReflectedObject root = new ReflectedObject(target);
            Assert.AreEqual(root["testVec"]["vec"]["x"].Value, 1f);
            Assert.AreEqual(root["testVec"]["vec"]["y"].Value, 2f);
            Assert.AreEqual(root["testVec"]["vec"]["z"].Value, 3f);
            Assert.AreEqual(root["str"].Value, "somevalue");
        }

        [Test]
        public void ObjectReassignment() {
            TestNested target = new TestNested();
            target.str = "somevalue";
            target.testVec = new TestVector();
            ReflectedObject root = new ReflectedObject(target);
            TestVector testVec = new TestVector();
            testVec.vec = new Vector3(-1f, -2f, -3f);
            root["testVec"].Value = testVec;
            Assert.AreEqual(root["testVec"]["vec"]["x"].Value, -1f);
            Assert.AreEqual(root["testVec"]["vec"]["y"].Value, -2f);
            Assert.AreEqual(root["testVec"]["vec"]["z"].Value, -3f);
            Assert.AreNotEqual(root["testVec"].Value, target.testVec);
        }

        [Test]
        public void ApplyModifiedProperties_Object() {
            TestNested target = new TestNested();
            target.str = "somevalue";
            target.testVec = new TestVector();
            ReflectedObject root = new ReflectedObject(target);
            TestVector testVec = new TestVector();
            testVec.vec = new Vector3(-1f, -2f, -3f);
            root["testVec"].Value = testVec;
            Assert.AreNotEqual(root["testVec"].Value, target.testVec);
            root.ApplyModifiedProperties();
            Assert.AreEqual(root["testVec"].Value, target.testVec);
            root["testVec"]["vec"]["x"].Value = 99f;
            Assert.AreNotEqual(99f, target.testVec.vec.x);
            root.ApplyModifiedProperties();
            Assert.AreEqual(99f, target.testVec.vec.x);
        }

        [Test]
        public void ApplyModifiedProperties_Struct() {
            Vector3 target = new Vector3(1f, 2f, 3f);
            ReflectedObject root = new ReflectedObject(target);
            root["x"].Value = 4f;
            root["y"].Value = 5f;
            Assert.AreEqual(1f, target.x);
            Assert.AreEqual(2f, target.y);
            root.ApplyModifiedProperties();
            target = (Vector3) root.Value;
            Assert.AreEqual(4f, target.x);
            Assert.AreEqual(5f, target.y);
        }

        [Test]
        public void BasicArray() {
            float[] target = new float[3];
            target[0] = 4f;
            target[1] = 5f;
            target[2] = 6f;
            ReflectedObject root = new ReflectedObject(target);
            Assert.AreEqual(root["0"].Value, 4f);
            Assert.AreEqual(root["1"].Value, 5f);
            Assert.AreEqual(root["2"].Value, 6f);
            root["0"].Value = 7f;
            root["1"].Value = 8f;
            root["2"].Value = 9f;
            Assert.AreEqual(root["0"].Value, 7f);
            Assert.AreEqual(root["1"].Value, 8f);
            Assert.AreEqual(root["2"].Value, 9f);
        }

        [Test]
        public void ApplyModifedProperties_Array() {
            float[] target = new float[3];
            target[0] = 4f;
            target[1] = 5f;
            target[2] = 6f;

            ReflectedObject root = new ReflectedObject(target);
            root["0"].Value = 1f;
            Assert.AreNotEqual(root["0"].Value, target[0]);
            root.ApplyModifiedProperties();
            Assert.AreEqual(root["0"].Value, target[0]);

        }

        [Test]
        public void ApplyModifiedProperties_List() {
            List<float> target = new List<float>();
           target.Add(4f);
           target.Add(5f);
           target.Add(6f);

            ReflectedObject root = new ReflectedObject(target);
            root["0"].Value = 1f;
            Assert.AreNotEqual(root["0"].Value, target[0]);
            root.ApplyModifiedProperties();
            Assert.AreEqual(root["0"].Value, target[0]);
        }

        [Test]
        public void ArrayOfStructs() {
            Vector3[] target = new Vector3[3];
            target[0] = new Vector3(1, 1, 1);
            target[1] = new Vector3(2, 2, 2);
            target[2] = new Vector3(3, 3, 3);
            ReflectedObject root = new ReflectedObject(target);
            root["0"]["x"].Value = -1f;
            Assert.AreNotEqual(root["0"]["x"].Value, target[0].x);
            root.ApplyModifiedProperties();
            Assert.AreEqual(-1f, target[0].x);
            Assert.AreEqual(root["0"]["x"].Value, target[0].x);
        }

    }

    [TestFixture]
    public class ReflectedProperty_Spec_Changed {

        class Thing {

            public float x = 1f;

        }
        
        [Test]
        public void SetsChangedProperty() {
            ReflectedObject obj = new ReflectedObject(new Thing());
            Assert.IsFalse(obj.HasModifiedProperties);
            obj["x"].Value = 2f;
            Assert.IsTrue(obj.HasModifiedProperties);
        }
        
        

    }

}