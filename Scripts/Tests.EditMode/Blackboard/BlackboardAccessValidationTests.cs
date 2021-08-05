namespace HiraBots.Editor.Tests.ImportedFromPlayMode
{
    internal class BlackboardAccessValidationTests : Tests.BlackboardAccessValidationTests
    {
        public override void SetUp()
        {
            SetUp(true);
        }

        public override void TearDown()
        {
            TearDown(true);
        }
    }
}