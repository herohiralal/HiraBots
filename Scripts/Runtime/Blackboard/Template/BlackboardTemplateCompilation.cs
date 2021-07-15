using System.Collections.Generic;
using System.Linq;

namespace HiraBots
{
    internal partial class BlackboardTemplate : ICompilable
    {
        internal BlackboardTemplateCompiledData CompiledData { get; private set; } = null;
        public bool IsCompiled => CompiledData != null;

        public void Compile()
        {
        }

        public void Free()
        {
            CompiledData = null;
        }

        public IEnumerable<ICompilable> GetHierarchyInOrder() =>
            (parent == null ? Enumerable.Empty<ICompilable>() : parent.GetHierarchyInOrder())
            .Append(this);
    }
}