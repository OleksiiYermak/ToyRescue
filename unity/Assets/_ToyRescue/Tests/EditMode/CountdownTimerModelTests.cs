using NUnit.Framework;
using ToyRescue.Activities;

namespace ToyRescue.Tests
{
    public sealed class CountdownTimerModelTests
    {
        [Test]
        public void ResetRestoresFullDuration()
        {
            var model = new CountdownTimerModel(15f);
            model.Start();
            model.Tick(5f);

            model.Reset(30f);

            Assert.AreEqual(30f, model.DurationSeconds);
            Assert.AreEqual(30f, model.RemainingSeconds);
            Assert.IsFalse(model.IsRunning);
        }

        [Test]
        public void TickClampsRemainingTimeToZero()
        {
            var model = new CountdownTimerModel(2f);
            model.Start();

            bool changed = model.Tick(5f);

            Assert.IsTrue(changed);
            Assert.AreEqual(0f, model.RemainingSeconds);
            Assert.IsFalse(model.IsRunning);
            Assert.IsTrue(model.IsComplete);
        }

        [Test]
        public void DisplayedSecondsRoundUpUntilTimerCompletes()
        {
            var model = new CountdownTimerModel(2.2f);
            model.Start();

            model.Tick(0.3f);
            Assert.AreEqual(2, model.DisplayedSeconds);

            model.Tick(0.95f);
            Assert.AreEqual(1, model.DisplayedSeconds);

            model.Tick(5f);
            Assert.AreEqual(0, model.DisplayedSeconds);
        }
    }
}
