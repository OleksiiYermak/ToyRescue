using NUnit.Framework;
using ToyRescue.Activities;

namespace ToyRescue.Tests
{
    public sealed class TargetToyMatcherTests
    {
        [Test]
        public void MatchesCategoryIgnoringCase()
        {
            var matcher = new TargetToyMatcher();

            bool isTargetToy = matcher.IsTargetToy(new[] { "blocks", "plush" }, "BLOCKS", true);

            Assert.IsTrue(isTargetToy);
        }

        [Test]
        public void RejectsToyWhenCountsAsTargetIsFalse()
        {
            var matcher = new TargetToyMatcher();

            bool isTargetToy = matcher.IsTargetToy(new[] { "blocks" }, "blocks", false);

            Assert.IsFalse(isTargetToy);
        }

        [Test]
        public void AcceptsTargetToyWhenActivityHasNoCategoryFilter()
        {
            var matcher = new TargetToyMatcher();

            bool isTargetToy = matcher.IsTargetToy(new string[0], "train", true);

            Assert.IsTrue(isTargetToy);
        }
    }
}
