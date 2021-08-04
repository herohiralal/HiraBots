using UnityEngine;
using NUnit.Framework;
using Unity.Mathematics;
using static HiraBots.BlackboardUnsafeHelpers;
using Random = UnityEngine.Random;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Validate the functionalities of blackboard read/write helper functions.
    /// </summary>
    [TestFixture]
    internal class BlackboardUnsafeHelpersTests
    {
        /// <summary>
        /// A generic unsigned 64-bit enum to be used for testing purposes.
        /// </summary>
        private enum GenericResult : ulong
        {
            Failure = 0,
            Success = 1
        }

        /// <summary>
        /// Validate boolean read/write.
        /// </summary>
        [Test]
        public unsafe void BooleanReadWriteTest()
        {
            var b = Random.Range(0, 2) != 0;

            var value = b;
            var stream = (byte*) &value;

            Assert.IsTrue(ReadBooleanValue(stream, 0) == b, "Read test failed.");

            WriteBooleanValue(stream, 0, !b);

            Assert.IsTrue(ReadBooleanValue(stream, 0) != b, "Write test failed.");
        }

        /// <summary>
        /// Validate enum read/write.
        /// </summary>
        [Test]
        public unsafe void EnumReadWriteTest()
        {
            var i = (byte) Random.Range(0, 2);

            var input = (GenericResult) i;

            ulong value = i;
            var stream = (byte*) &value;

            Assert.AreEqual(ReadEnumValue<GenericResult>(stream, 0), input, "Read test failed.");

            WriteEnumValue<GenericResult>(stream, 0, input == GenericResult.Failure ? GenericResult.Success : GenericResult.Failure);

            Assert.AreEqual(ReadEnumValue<GenericResult>(stream, 0),
                input == GenericResult.Failure ? GenericResult.Success : GenericResult.Failure, "Write test failed.");
        }

        /// <summary>
        /// Validate float read/write.
        /// </summary>
        [Test]
        public unsafe void FloatReadWriteTest()
        {
            var f = Random.Range(-5f, 5f);

            var value = f;
            var stream = (byte*) &value;

            Assert.AreEqual(ReadFloatValue(stream, 0), f, "Read test failed.");

            WriteFloatValue(stream, 0, f + 50);

            Assert.AreEqual(ReadFloatValue(stream, 0), f + 50, "Write test failed.");
        }

        /// <summary>
        /// Validate integer read/write.
        /// </summary>
        [Test]
        public unsafe void IntegerReadWriteTest()
        {
            var i = Random.Range(-5, 5);

            var value = i;
            var stream = (byte*) &value;

            Assert.AreEqual(ReadIntegerValue(stream, 0), i, "Read test failed.");

            WriteIntegerValue(stream, 0, i + 50);

            Assert.AreEqual(ReadIntegerValue(stream, 0), i + 50, "Write test failed.");
        }

        /// <summary>
        /// Validate Object read/write.
        /// </summary>
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

        /// <summary>
        /// Validate quaternion read/write.
        /// </summary>
        [Test]
        public unsafe void QuaternionReadWriteTest()
        {
            var q = (quaternion) Random.rotation;
            var stream = (byte*) &q;

            Assert.AreEqual(q, ReadQuaternionValue(stream, 0), "Read test failed.");

            var newQuaternion = math.mul(quaternion.Euler(new float3(1, 1, 1)), q);

            Assert.AreNotEqual(q, newQuaternion, "New value test failed.");

            WriteQuaternionValue(stream, 0, newQuaternion);

            Assert.AreEqual(newQuaternion, q, "Write test failed.");
        }

        /// <summary>
        /// Validate vector read/write.
        /// </summary>
        [Test]
        public unsafe void VectorReadWriteTest()
        {
            var vector = new float3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            var stream = (byte*) &vector;

            Assert.AreEqual(vector, ReadVectorValue(stream, 0), "Read test failed.");

            var newVector = vector + new float3(100, 100, 100);

            Assert.AreNotEqual(vector, newVector, "New value test failed.");

            WriteVectorValue(stream, 0, newVector);

            Assert.AreEqual(newVector, vector, "Write test failed.");
        }
    }
}