using NUnit.Framework;
using ToyRescue.Core;

namespace ToyRescue.Tests
{
    public sealed class GameStateMachineTests
    {
        [Test]
        public void StartsInBootByDefault()
        {
            var stateMachine = new GameStateMachine();

            Assert.AreEqual(GameState.Boot, stateMachine.CurrentState);
        }

        [Test]
        public void TransitionToChangesCurrentState()
        {
            var stateMachine = new GameStateMachine();

            bool changed = stateMachine.TransitionTo(GameState.MainMenu);

            Assert.IsTrue(changed);
            Assert.AreEqual(GameState.MainMenu, stateMachine.CurrentState);
        }

        [Test]
        public void TransitionToSameStateReturnsFalse()
        {
            var stateMachine = new GameStateMachine();

            bool changed = stateMachine.TransitionTo(GameState.Boot);

            Assert.IsFalse(changed);
            Assert.AreEqual(GameState.Boot, stateMachine.CurrentState);
        }

        [Test]
        public void TransitionPublishesStateChangedEvent()
        {
            var eventBus = new EventBus();
            var stateMachine = new GameStateMachine(eventBus);
            GameStateChangedEvent receivedEvent = default;
            bool wasReceived = false;

            eventBus.Subscribe<GameStateChangedEvent>(eventData =>
            {
                receivedEvent = eventData;
                wasReceived = true;
            });

            stateMachine.TransitionTo(GameState.LoadingGame);

            Assert.IsTrue(wasReceived);
            Assert.AreEqual(GameState.Boot, receivedEvent.PreviousState);
            Assert.AreEqual(GameState.LoadingGame, receivedEvent.CurrentState);
        }
    }
}
