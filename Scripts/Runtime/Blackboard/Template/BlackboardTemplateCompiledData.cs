using Unity.Collections;

namespace HiraBots
{
    internal class BlackboardTemplateCompiledData
    {
        private NativeArray<byte> _template = default;
        
        ~BlackboardTemplateCompiledData()
        {
            if (_template.IsCreated)
                _template.Dispose();
        }
    }
}