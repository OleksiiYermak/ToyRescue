using System;
using System.Collections.Generic;

namespace ToyRescue.Core
{
    public sealed class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> handlersByType = new();

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Type eventType = typeof(TEvent);
            if (!handlersByType.TryGetValue(eventType, out List<Delegate> handlers))
            {
                handlers = new List<Delegate>();
                handlersByType.Add(eventType, handlers);
            }

            handlers.Add(handler);
            return new Subscription<TEvent>(this, handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null)
            {
                return;
            }

            Type eventType = typeof(TEvent);
            if (!handlersByType.TryGetValue(eventType, out List<Delegate> handlers))
            {
                return;
            }

            handlers.Remove(handler);
            if (handlers.Count == 0)
            {
                handlersByType.Remove(eventType);
            }
        }

        public void Publish<TEvent>(TEvent eventData)
        {
            Type eventType = typeof(TEvent);
            if (!handlersByType.TryGetValue(eventType, out List<Delegate> handlers))
            {
                return;
            }

            Delegate[] snapshot = handlers.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                ((Action<TEvent>)snapshot[i]).Invoke(eventData);
            }
        }

        public void Clear()
        {
            handlersByType.Clear();
        }

        private sealed class Subscription<TEvent> : IDisposable
        {
            private EventBus eventBus;
            private Action<TEvent> handler;

            public Subscription(EventBus eventBus, Action<TEvent> handler)
            {
                this.eventBus = eventBus;
                this.handler = handler;
            }

            public void Dispose()
            {
                if (eventBus == null || handler == null)
                {
                    return;
                }

                eventBus.Unsubscribe(handler);
                eventBus = null;
                handler = null;
            }
        }
    }

    public readonly struct GameStateChangedEvent
    {
        public GameStateChangedEvent(GameState previousState, GameState currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }

        public GameState PreviousState { get; }
        public GameState CurrentState { get; }
    }

    public readonly struct SettingsChangedEvent
    {
        public SettingsChangedEvent(bool musicEnabled, bool sfxEnabled)
        {
            MusicEnabled = musicEnabled;
            SfxEnabled = sfxEnabled;
        }

        public bool MusicEnabled { get; }
        public bool SfxEnabled { get; }
    }

    public readonly struct ActivityStartedEvent
    {
        public ActivityStartedEvent(string activityId)
        {
            ActivityId = activityId;
        }

        public string ActivityId { get; }
    }

    public readonly struct ActivityConfiguredEvent
    {
        public ActivityConfiguredEvent(
            string activityId,
            string objectiveText,
            int targetCount,
            float durationSeconds,
            int collectedCount)
        {
            ActivityId = activityId;
            ObjectiveText = objectiveText;
            TargetCount = targetCount;
            DurationSeconds = durationSeconds;
            CollectedCount = collectedCount;
        }

        public string ActivityId { get; }
        public string ObjectiveText { get; }
        public int TargetCount { get; }
        public float DurationSeconds { get; }
        public int CollectedCount { get; }
    }

    public readonly struct ToySelectedEvent
    {
        public ToySelectedEvent(string toyId, string category)
        {
            ToyId = toyId;
            Category = category;
        }

        public string ToyId { get; }
        public string Category { get; }
    }

    public readonly struct ToyAcceptedEvent
    {
        public ToyAcceptedEvent(string toyId)
        {
            ToyId = toyId;
        }

        public string ToyId { get; }
    }

    public readonly struct ToyRejectedEvent
    {
        public ToyRejectedEvent(string toyId)
        {
            ToyId = toyId;
        }

        public string ToyId { get; }
    }

    public readonly struct ToyCollectedEvent
    {
        public ToyCollectedEvent(string toyId, int collectedCount, int targetCount)
        {
            ToyId = toyId;
            CollectedCount = collectedCount;
            TargetCount = targetCount;
        }

        public string ToyId { get; }
        public int CollectedCount { get; }
        public int TargetCount { get; }
    }

    public readonly struct TimerTickEvent
    {
        public TimerTickEvent(float remainingSeconds)
        {
            RemainingSeconds = remainingSeconds;
        }

        public float RemainingSeconds { get; }
    }

    public readonly struct ActivityWonEvent
    {
        public ActivityWonEvent(string activityId, float remainingSeconds)
        {
            ActivityId = activityId;
            RemainingSeconds = remainingSeconds;
        }

        public string ActivityId { get; }
        public float RemainingSeconds { get; }
    }

    public readonly struct ActivityFailedEvent
    {
        public ActivityFailedEvent(string activityId)
        {
            ActivityId = activityId;
        }

        public string ActivityId { get; }
    }
}
