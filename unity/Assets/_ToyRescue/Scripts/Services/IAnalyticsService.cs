using System.Collections.Generic;

namespace ToyRescue.Services
{
    public interface IAnalyticsService
    {
        void Track(string eventName, IReadOnlyDictionary<string, object> properties = null);
        void TrackActivityStarted(string activityId);
        void TrackToySelected(string toyId);
        void TrackToyCollected(string toyId, int collectedCount, int targetCount);
        void TrackWrongToy(string toyId);
        void TrackActivityWon(string activityId);
        void TrackActivityFailed(string activityId);
    }
}
