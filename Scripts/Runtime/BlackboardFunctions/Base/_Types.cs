namespace HiraBots
{
    [System.Serializable]
    internal enum SampleBlackboardFunctionsIntegerComparisonType : byte
    {
        Equals,
        GreaterThan,
        GreaterThanEqualTo,
        LesserThan,
        LesserThanEqualTo
    }

    [System.Serializable]
    internal enum SampleBlackboardFunctionsFloatComparisonType : byte
    {
        AlmostEquals,
        GreaterThan,
        GreaterThanEqualTo,
        LesserThan,
        LesserThanEqualTo
    }

    [System.Serializable]
    internal enum SampleBlackboardFunctionsEnumOperationType
    {
        Set,
        AddFlags,
        RemoveFlags
    }

    [System.Serializable]
    internal enum SampleBlackboardFunctionsFloatOperationType
    {
        Set,
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    [System.Serializable]
    internal enum SampleBlackboardFunctionsIntegerOperationType
    {
        Set,
        Add,
        Subtract,
        Multiply,
        Divide,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
    }

    [System.Serializable]
    internal enum SampleBlackboardFunctionsSetOperationType
    {
        Set,
        Unset
    }
}