using System.Text;

namespace HiraBots.Editor
{
    /// <summary>
    /// Validator to abstract out any context required by the blackboard templates.
    /// </summary>
    internal class BlackboardValidator
    {
        private readonly StringBuilder m_ErrorStringBuilder;
        private readonly BlackboardTemplateValidatorContext m_Context;

        internal BlackboardValidator()
        {
            m_ErrorStringBuilder = new StringBuilder(2000);
            m_Context = new BlackboardTemplateValidatorContext();
        }

        /// <summary>
        /// Validate a Blackboard Template.
        /// </summary>
        /// <param name="target">The target template to validate.</param>
        /// <param name="errorText">The error text.</param>
        /// <returns>Whether the template is validated.</returns>
        internal bool Execute(BlackboardTemplate target, out string errorText)
        {
            var success = true;

            target.Validate(m_Context);

            if (!m_Context.validated)
            {
                success = false;

                m_ErrorStringBuilder.AppendLine($"Failed to validate blackboard template {target.name}.\n\n");

                if (m_Context.recursionPoint != null)
                {
                    m_ErrorStringBuilder.AppendLine($"Contains cyclical hierarchy. Recursion Point - {m_Context.recursionPoint}.");
                }

                foreach (var index in m_Context.emptyIndices)
                {
                    m_ErrorStringBuilder.AppendLine($"The key at index {index} is empty.");
                }

                foreach (var (keyName, template) in m_Context.duplicateKeys)
                {
                    m_ErrorStringBuilder.AppendLine($"Contains duplicate keys named {keyName}." +
                                                    $" {(template == target ? "" : $" Inherited from {template.name}.")}");
                }

                foreach (var key in m_Context.badKeys)
                {
                    m_ErrorStringBuilder.AppendLine($"Contains invalid data for the key {key.name}.");
                }
            }

            m_Context.Reset();

            errorText = m_ErrorStringBuilder.ToString();
            m_ErrorStringBuilder.Clear();

            return success;
        }
    }
}