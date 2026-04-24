using NUnit.Framework;
using ToyRescue.Activities;

namespace ToyRescue.Tests
{
    public sealed class ActivityProgressModelTests
    {
        [Test]
        public void CollectingTargetToysReachesWinState()
        {
            var model = new ActivityProgressModel(3);

            Assert.IsTrue(model.TryCollectTarget());
            Assert.IsTrue(model.TryCollectTarget());
            Assert.IsTrue(model.TryCollectTarget());

            Assert.AreEqual(3, model.CollectedCount);
            Assert.AreEqual(ActivityOutcome.Win, model.EvaluateOutcome(10f));
        }

        [Test]
        public void TimerExpiryBeforeTargetCountReturnsFail()
        {
            var model = new ActivityProgressModel(3);
            model.TryCollectTarget();

            ActivityOutcome outcome = model.EvaluateOutcome(0f);

            Assert.AreEqual(ActivityOutcome.Fail, outcome);
        }

        [Test]
        public void WinStateTakesPriorityOverTimerExpiry()
        {
            var model = new ActivityProgressModel(1);
            model.TryCollectTarget();

            ActivityOutcome outcome = model.EvaluateOutcome(0f);

            Assert.AreEqual(ActivityOutcome.Win, outcome);
        }
    }
}
