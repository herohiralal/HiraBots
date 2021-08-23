namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        public string generatedCode
        {
            get
            {
                return CodeGenHelpers.ReadTemplate("Blackboard/Blackboard",
                    ("<BLACKBOARD-NAME>", name));
            }
        }
    }
}