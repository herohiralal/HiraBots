using NUnit.Framework;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Tests to validate the data-cooking functionality.
    /// </summary>
    [TestFixture]
    internal class CookedDataTests
    {
        /// <summary>
        /// Confirm the existence of template collection instance, be it in play mode or in the build.
        /// Also validate the sorting order in the template collection.
        /// </summary>
        [Test]
        public void BlackboardTemplateCollectionIsCooked()
        {
            var btc = BlackboardTemplateCollection.instance;
            Assert.IsTrue(btc != null, "BlackboardTemplateCollection must be cooked into play mode or build.");

            for (var i = 1; i < btc.count; i++)
            {
                Assert.IsTrue(btc[i].hierarchyIndex >= btc[i - 1].hierarchyIndex, "Blackboard template collection built without proper sorting.");
            }
        }
    }
}