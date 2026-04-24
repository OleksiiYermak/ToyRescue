using System.Collections.Generic;
using UnityEngine;

namespace ToyRescue.Data
{
    [CreateAssetMenu(fileName = "ActivityDefinition", menuName = "Toy Rescue/Activity Definition")]
    public sealed class ActivityDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string activityId = "toy_rescue_default";
        [SerializeField, TextArea] private string tutorialText = "Can you help me clean the room? Find the toys and put them in the basket before time runs out!";

        [Header("Rules")]
        [SerializeField, Min(1)] private int targetCount = 3;
        [SerializeField, Min(1f)] private float durationSeconds = 60f;
        [SerializeField] private List<string> validToyCategories = new List<string>();

        [Header("Content")]
        [SerializeField] private string roomPrefabAddress;
        [SerializeField] private GameObject directRoomPrefab;

        [Header("Music Cues")]
        [SerializeField] private string menuMusicCueId = "menu";
        [SerializeField] private string preGameMusicCueId = "pregame";
        [SerializeField] private string playingMusicCueId = "playing";
        [SerializeField] private string resultMusicCueId = "result";

        public string ActivityId => activityId;
        public string TutorialText => tutorialText;
        public int TargetCount => targetCount;
        public float DurationSeconds => durationSeconds;
        public IReadOnlyList<string> ValidToyCategories => validToyCategories;
        public string RoomPrefabAddress => roomPrefabAddress;
        public GameObject DirectRoomPrefab => directRoomPrefab;
        public string MenuMusicCueId => menuMusicCueId;
        public string PreGameMusicCueId => preGameMusicCueId;
        public string PlayingMusicCueId => playingMusicCueId;
        public string ResultMusicCueId => resultMusicCueId;

        public bool AcceptsCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return false;
            }

            return validToyCategories.Contains(category);
        }
    }
}
