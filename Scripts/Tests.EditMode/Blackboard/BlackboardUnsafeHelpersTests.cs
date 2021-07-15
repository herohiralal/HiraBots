using UnityEngine;
using NUnit.Framework;
using static HiraBots.BlackboardUnsafeHelpers;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal class BlackboardUnsafeHelpersTests
    {
        private enum GenericResult : ulong
        {
            Failure = 0,
            Success = 1
        }

        [Test]
        public unsafe void BooleanReadWriteTest([Random(0, 1, 1)] byte b)
        {
            var value = b;
            var stream = &value;

            var booleanValue = b != 0;

            Assert.IsTrue(ReadBooleanValue(stream, 0) == booleanValue, "Read test failed.");

            WriteBooleanValue(stream, 0, !booleanValue);

            Assert.IsTrue(ReadBooleanValue(stream, 0) != booleanValue, "Write test failed.");
        }

        [Test]
        public unsafe void EnumReadWriteTest([Random(0, 1, 1)] byte i)
        {
            var input = (GenericResult) i;

            ulong value = i;
            var stream = (byte*) &value;

            Assert.AreEqual(ReadEnumValue<GenericResult>(stream, 0), input, "Read test failed.");

            WriteEnumValue<GenericResult>(stream, 0, input == GenericResult.Failure ? GenericResult.Success : GenericResult.Failure);

            Assert.AreEqual(ReadEnumValue<GenericResult>(stream, 0),
                input == GenericResult.Failure ? GenericResult.Success : GenericResult.Failure, "Write test failed.");
        }

        [Test]
        public unsafe void FloatReadWriteTest([Random(-5f, 5f, 1)] float f)
        {
            var value = f;
            var stream = (byte*) &value;

            Assert.AreEqual(ReadFloatValue(stream, 0), f, "Read test failed.");

            WriteFloatValue(stream, 0, f + 50);

            Assert.AreEqual(ReadFloatValue(stream, 0), f + 50, "Write test failed.");
        }

        [Test]
        public unsafe void IntegerReadWriteTest([Random(-5, 5, 1)] int i)
        {
            var value = i;
            var stream = (byte*) &value;

            Assert.AreEqual(ReadIntegerValue(stream, 0), i, "Read test failed.");

            WriteIntegerValue(stream, 0, i + 50);

            Assert.AreEqual(ReadIntegerValue(stream, 0), i + 50, "Write test failed.");
        }

        [Test]
        public unsafe void ObjectReadWriteTest()
        {
            var obj1 = ScriptableObject.CreateInstance<ScriptableObject>();
            var obj2 = ScriptableObject.CreateInstance<ScriptableObject>();
            obj1.hideFlags = HideFlags.HideAndDontSave;
            obj2.hideFlags = HideFlags.HideAndDontSave;

            var inst1 = obj1.GetInstanceID();
            var inst2 = obj2.GetInstanceID();

            var i = 0;
            var stream = (byte*) &i;

            Assert.IsNull(ReadObjectValue(stream, 0), "Null read value failed.");
            WriteObjectValue(stream, 0, obj1);

            Assert.AreEqual(obj1, ReadObjectValue(stream, 0), "Write test failed.");
            WriteObjectValue(stream, 0, obj2);

            Assert.AreEqual(obj2, ReadObjectValue(stream, 0), "Overwrite test failed.");
            WriteObjectValue(stream, 0, null);

            Assert.IsNull(ReadObjectValue(stream, 0), "Null write test failed.");

            Object.DestroyImmediate(obj1);
            Object.DestroyImmediate(obj2);

            i = inst1;
            Assert.IsNull(ReadObjectValue(stream, 0), "Resource freeing test 1 failed.");

            i = inst2;
            Assert.IsNull(ReadObjectValue(stream, 0), "Resource freeing test 2 failed.");
        }

        [Test]
        public unsafe void QuaternionReadWriteTest(
            [Random(float.MinValue, float.MaxValue, 1)] float x,
            [Random(float.MinValue, float.MaxValue, 1)] float y,
            [Random(float.MinValue, float.MaxValue, 1)] float z,
            [Random(float.MinValue, float.MaxValue, 1)] float w)
        {
            var quaternion = new Quaternion(x, y, z, w);
            var stream = (byte*) &quaternion;

            Assert.AreEqual(quaternion, ReadQuaternionValue(stream, 0), "Read test failed.");

            var newQuaternion = Quaternion.Euler(Vector3.one) * quaternion;

            Assert.AreNotEqual(quaternion, newQuaternion, "New value test failed.");

            WriteQuaternionValue(stream, 0, newQuaternion);

            Assert.AreEqual(newQuaternion, quaternion, "Write test failed.");
        }

        [Test]
        public unsafe void VectorReadWriteTest(
            [Random(-5, 5, 1)] float x,
            [Random(-5, 5, 1)] float y,
            [Random(-5, 5, 1)] float z)
        {
            var vector = new Vector3(x, y, z);
            var stream = (byte*) &vector;

            Assert.AreEqual(vector, ReadVectorValue(stream, 0), "Read test failed.");

            var newVector = vector + (100 * Vector3.one);

            Assert.AreNotEqual(vector, newVector, "New value test failed.");

            WriteVectorValue(stream, 0, newVector);

            Assert.AreEqual(newVector, vector, "Write test failed.");
        }
    }
}