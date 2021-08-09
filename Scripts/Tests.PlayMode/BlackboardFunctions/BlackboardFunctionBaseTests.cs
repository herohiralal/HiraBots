using AOT;
using NUnit.Framework;
using Unity.Burst;
using UnityEngine;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal unsafe class BlackboardFunctionBaseTests
    {
        private delegate int FuncInt(in LowLevelBlackboard blackboard);

        private delegate void Action(in LowLevelBlackboard blackboard, byte* memory);

        [BurstCompile(CompileSynchronously = true)]
        private class SquareCalculator : BlackboardFunction<FuncInt>
        {
            private static bool s_FunctionCompiled = false;
            private static FunctionPointer<FuncInt> s_FunctionPointer;

            // compile and store the function pointer
            protected override void OnEnable()
            {
                base.OnEnable();
                if (!s_FunctionCompiled)
                {
                    s_FunctionPointer = BurstCompiler.CompileFunctionPointer<FuncInt>(Square);
                    s_FunctionCompiled = true;
                }
            }

            // override function pointer
            protected override FunctionPointer<FuncInt> function => s_FunctionPointer;

            // calculates and returns the square of a number
            [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(FuncInt))]
            private static int Square(in LowLevelBlackboard blackboard)
            {
                var value = blackboard.Access<int>(0);
                return value * value;
            }

            // build this function
            internal new static T Build<T>(string name, HideFlags hideFlags = HideFlags.None)
                where T : SquareCalculator
            {
                return BlackboardFunction<FuncInt>.Build<T>(name, hideFlags);
            }
        }

        [BurstCompile(CompileSynchronously = true)]
        private class CubeCalculator : BlackboardFunction<Action>
        {
            // the memory to be stored by this function
            private readonly struct Memory
            {
                internal Memory(int value)
                {
                    this.value = value;
                }

                internal int value { get; }
            }

            private static bool s_FunctionCompiled = false;
            private static FunctionPointer<Action> s_FunctionPointer;

            // compile and store the function pointer
            protected override void OnEnable()
            {
                base.OnEnable();
                if (!s_FunctionCompiled)
                {
                    s_FunctionPointer = BurstCompiler.CompileFunctionPointer<Action>(Cube);
                    s_FunctionCompiled = true;
                }
            }

            // the value to cube
            private int m_Value = 34;

            protected override int memorySize => base.memorySize + ByteStreamHelpers.CombinedSizes<Memory>(); // header includes the memory

            internal override byte* Compile(byte* stream)
            {
                stream = base.Compile(stream);

                // no offset
                ByteStreamHelpers.Write<Memory>(ref stream, new Memory(m_Value));

                // offset sizeof(Memory)
                return stream;
            }

            // override function pointer
            protected override FunctionPointer<Action> function => s_FunctionPointer;

            // calculates the cube of a number and saves it back in the blackboard
            [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(Action))]
            private static void Cube(in LowLevelBlackboard blackboard, byte* memory)
            {
                var actualMemory = (Memory*) memory;
                blackboard.Access<int>(0) = actualMemory->value * actualMemory->value * actualMemory->value;
            }

            // build this function
            internal static T Build<T>(string name, int value, HideFlags hideFlags = HideFlags.None)
                where T : CubeCalculator
            {
                var output = BlackboardFunction<Action>.Build<T>(name, hideFlags);
                output.m_Value = value;
                return output;
            }
        }

        /// <summary>
        /// Test a function which does not modify a blackboard.
        /// </summary>
        [Test]
        public void TestBlackboardPureFunction()
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
                calculator.Compile(functionAddress);
                var function = (LowLevelBlackboardFunction) functionAddress;

                // cross-check memory size
                Assert.AreEqual(memorySize, function.size, "Size mismatch between function and its low-level counterpart.");

                // calculate square
                var functionPointer = new FunctionPointer<FuncInt>(function.functionPtr);
                var square = functionPointer.Invoke(in blackboard);

                // cross check function return value
                Assert.AreEqual(value * value, square, "Function return mismatch.");
            }
            finally
            {
                Object.DestroyImmediate(calculator);
            }
        }

        /// <summary>
        /// Test a function which does modify the blackboard.
        /// </summary>
        [Test]
        public void TestBlackboardImpureFunction()
        {
            var value = Random.Range(-10, 10);

            var calculator = CubeCalculator.Build<CubeCalculator>("square calculator", value, HideFlags.HideAndDontSave);
            try
            {
                // create blackboard and assign value
                var blackboardAddress = stackalloc byte[sizeof(int)];
                var blackboard = new LowLevelBlackboard(blackboardAddress, sizeof(int));

                // compile function
                var memorySize = calculator.GetAlignedMemorySize();
                var functionAddress = stackalloc byte[memorySize];
                calculator.Compile(functionAddress);
                var function = (LowLevelBlackboardFunction) functionAddress;

                // cross-check memory size
                Assert.AreEqual(memorySize, function.size, "Size mismatch between function and its low-level counterpart.");

                // calculate cube
                var functionPointer = new FunctionPointer<Action>(function.functionPtr);
                functionPointer.Invoke(in blackboard, function.memory);
                
                // cross check modified function value
                var modifiedValue = blackboard.Access<int>(0);
                Assert.AreEqual(value * value * value, modifiedValue, "Function modification mismatch.");
            }
            finally
            {
                Object.DestroyImmediate(calculator);
            }
        }
    }
}