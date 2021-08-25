using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal unsafe partial class BlackboardFunction<TFunction>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DefaultLowLevelObjectProviderCollection<BlackboardFunction<TFunction>,
                LowLevelBlackboardFunction, LowLevelBlackboardFunction.PointerConverter>
            GetLowLevelCollectionProvider(BlackboardFunction<TFunction>[] functionCollection)
        {
            return new DefaultLowLevelObjectProviderCollection<
                BlackboardFunction<TFunction>, LowLevelBlackboardFunction, LowLevelBlackboardFunction.PointerConverter>(functionCollection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetAlignedMemorySize(BlackboardFunction<TFunction>[] functionCollection)
        {
            return GetLowLevelCollectionProvider(functionCollection).GetAlignedMemorySize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* WriteLowLevelObjectAndJumpPast(BlackboardFunction<TFunction>[] functionCollection, byte* stream)
        {
            return GetLowLevelCollectionProvider(functionCollection).WriteLowLevelObjectAndJumpPast(stream);
        }
    }

    internal unsafe partial class DecoratorBlackboardFunction
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetAlignedMemorySize(DecoratorBlackboardFunction[] functionCollection)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunction<DecoratorDelegate>.GetAlignedMemorySize(functionCollection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* WriteLowLevelObjectAndJumpPast(DecoratorBlackboardFunction[] functionCollection, byte* stream)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunction<DecoratorDelegate>.WriteLowLevelObjectAndJumpPast(functionCollection, stream);
        }
    }

    internal unsafe partial class EffectorBlackboardFunction
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetAlignedMemorySize(EffectorBlackboardFunction[] functionCollection)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunction<EffectorDelegate>.GetAlignedMemorySize(functionCollection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* WriteLowLevelObjectAndJumpPast(EffectorBlackboardFunction[] functionCollection, byte* stream)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunction<EffectorDelegate>.WriteLowLevelObjectAndJumpPast(functionCollection, stream);
        }
    }
}