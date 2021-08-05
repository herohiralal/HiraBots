using NUnit.Framework;
using Unity.Burst;
using UnityEngine;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal unsafe class BlackboardFunctionBaseTests
    {
        private delegate int FuncInt(void* blackboard);

        [BurstCompile(CompileSynchronously = true)]
        private class SquareCalculator : BlackboardFunction<FuncInt>
        {
            private static readonly FunctionPointer<FuncInt> s_FunctionPointer;

            static SquareCalculator()
            {
                s_FunctionPointer = BurstCompiler.CompileFunctionPointer<FuncInt>(Square);
            }

            protected override int memorySize => base.memorySize + fakeHeaderSize;

            private static int fakeHeaderSize => ByteStreamHelpers.CombinedSizes<int>(); // fake int header

            protected override FunctionPointer<FuncInt> function => s_FunctionPointer;

            [BurstCompile(DisableDirectCall = true)]
            private static int Square(void* blackboard)
            {
                var value = (*(LowLevelBlackboard*) blackboard).Access<int>(0);
                return value * value;
            }

            internal new static T Build<T>(string name, HideFlags hideFlags = HideFlags.None)
                where T : SquareCalculator
            {
                return BlackboardFunction<FuncInt>.Build<T>(name, hideFlags);
            }
        }

        [Test]
        public void TestSquareCalculator()
        {
            var value = Random.Range(-10, 10);

            var calculator = SquareCalculator.Build<SquareCalculator>("square calculator", HideFlags.HideAndDontSave);
            try
            {
                // create blackboard and assign value
                var blackboardAddress = stackalloc byte[sizeof(int)];
                var blackboard = new LowLevelBlackboard(blackboardAddress, sizeof(int));
                blackboard.Access<int>(0) = value;

                // compile function
                var memorySize = calculator.GetAlignedMemorySize();
                var functionAddress = stackalloc byte[memorySize];
                calculator.AppendMemory(functionAddress);
                var function = (LowLevelBlackboardFunction) functionAddress;

                // cross-check memory size
                Assert.AreEqual(memorySize, function.size, "Size mismatch between function and its low-level counterpart.");

                // calculate square
                var functionPointer = new FunctionPointer<FuncInt>(function.functionPtr);
                var square = functionPointer.Invoke(&blackboard);

                // cross check function return value
                Assert.AreEqual(value * value, square, "Function return mismatch.");
            }
            finally
            {
                Object.DestroyImmediate(calculator);
            }
        }
    }
}