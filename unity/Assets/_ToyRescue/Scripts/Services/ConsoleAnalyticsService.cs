using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ToyRescue.Services
{
    public sealed class ConsoleAnalyticsService : IAnalyticsService
    {
        public void Track(string eventName, IReadOnlyDictionary<string, object> properties = null)
        {
            if (properties == null || properties.Count == 0)
            {
                Debug.Log($"[Analytics] {eventName}");
                return;
            }

            var builder = new StringBuilder();
            builder.Append("[Analytics] ");
            builder.Append(eventName);

            foreach (KeyValuePair<string, object> property in properties)
            {
                builder.Append(" | ");
                builder.Append(property.Key);
                builder.Append('=');
                builder.Append(property.Value);
            }

            Debug.Log(builder.ToString());
        }

        public void TrackActivityStarted(string activityId)
        {
            Track("activity_start", new Dictionary<string, object> { { "activity_id", activityId } });
        }

        public void TrackToySelected(string toyId)
        {
            Track("toy_selected", new Dictionary<string, object> { { "toy_id", toyId } });
        }

        public void TrackToyCollected(string toyId, int collectedCount, int targetCount)
        {
            Track("toy_collected", new Dictionary<string, object>
            {
                { "toy_id", toyId },
                { "collected_count", collectedCount },
                { "target_count", targetCount }
            });
        }

        public void TrackWrongToy(string toyId)
        {
            Track("wrong_toy", new Dictionary<string, object> { { "toy_id", toyId } });
        }

        public void TrackActivityWon(string activityId)
        {
            Track("activity_win", new Dictionary<string, object> { { "activity_id", activityId } });
        }

        public void TrackActivityFailed(string activityId)
        {
            Track("activity_fail", new Dictionary<string, object> { { "activity_id", activityId } });
        }
    }
}
