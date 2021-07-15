using System.Collections.Generic;

namespace HiraBots
{
    internal interface ICompilable
    {
        bool IsCompiled { get; }
        void Compile();
        void Free();
        IEnumerable<ICompilable> GetHierarchyInOrder();
    }
}