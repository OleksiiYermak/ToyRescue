using System;

namespace ToyRescue.Core
{
    public sealed class GameStateMachine
    {
        private readonly EventBus eventBus;

        public GameStateMachine(EventBus eventBus = null, GameState initialState = GameState.Boot)
        {
            this.eventBus = eventBus;
            CurrentState = initialState;
        }

        public event Action<GameStateChangedEvent> StateChanged;

        public GameState CurrentState { get; private set; }

        public bool CanTransitionTo(GameState nextState)
        {
            return CurrentState != nextState;
        }

        public bool TransitionTo(GameState nextState)
        {
            if (!CanTransitionTo(nextState))
            {
                return false;
            }

            GameState previousState = CurrentState;
            CurrentState = nextState;

            var eventData = new GameStateChangedEvent(previousState, nextState);
            StateChanged?.Invoke(eventData);
            eventBus?.Publish(eventData);
            return true;
        }
    }
}
