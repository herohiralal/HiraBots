using System.Text;

namespace HiraBots.Editor
{
    internal class BlackboardValidator
    {
        private readonly StringBuilder _sb;
        private readonly BlackboardTemplateValidatorContext _context;

        internal BlackboardValidator()
        {
            _sb = new StringBuilder(2000);
            _context = new BlackboardTemplateValidatorContext();
        }

        internal bool Execute(BlackboardTemplate target, out string errorText)
        {
            var success = true;

            target.Validate(_context);

            if (!_context.Validated)
            {
                success = false;

                _sb.AppendLine($"Failed to validate blackboard template {target.name}.\n\n");

                if (_context.RecursionPoint != null)
                    _sb.AppendLine($"Contains cyclical hierarchy. Recursion Point - {_context.RecursionPoint}.");

                foreach (var index in _context.EmptyIndices)
                    _sb.AppendLine($"The key at index {index} is empty.");

                foreach (var (keyName, template) in _context.DuplicateKeys)
                    _sb.AppendLine($"Contains duplicate keys named {keyName}." +
                                   $" {(template == target ? "" : $" Inherited from {template.name}.")}");
            }

            _context.Reset();

            errorText = _sb.ToString();
            _sb.Clear();

            return success;
        }
    }
}