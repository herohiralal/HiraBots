using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent
    {
        private BlackboardTemplateCompiledData _template;
        private NativeArray<byte> _data;

        private byte* DataPtr => (byte*) _data.GetUnsafePtr();
        private byte* DataReadOnlyPtr => (byte*) _data.GetUnsafeReadOnlyPtr();

        internal static bool TryCreate(BlackboardTemplate template, out BlackboardComponent component)
        {
            if (template == null)
            {
                component = null;
                return false;
            }

            if (!template.IsCompiled)
            {
                component = null;
                return false;
            }

            component = new BlackboardComponent(template.CompiledData);
            return true;
        }

        private BlackboardComponent(BlackboardTemplateCompiledData template)
        {
            _template = template;
            _data = new NativeArray<byte>(_template.TemplateSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _template.CopyTemplateTo(_data);
            _unexpectedChanges = new System.Collections.Generic.List<ushort>(template.KeyCount);
            _template.AddInstanceSyncListener(this);
        }

        ~BlackboardComponent()
        {
            _template.RemoveInstanceSyncListener(this);

            if (_data.IsCreated)
                _data.Dispose();

            _template = null;
        }
    }
}