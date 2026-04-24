using UnityEngine;

namespace ToyRescue.Data
{
    [CreateAssetMenu(fileName = "ToyDefinition", menuName = "Toy Rescue/Toy Definition")]
    public sealed class ToyDefinition : ScriptableObject
    {
        [SerializeField] private string toyId = "toy";
        [SerializeField] private string displayName = "Toy";
        [SerializeField] private string category = "toy";
        [SerializeField] private bool countsAsTarget = true;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;

        public string ToyId => toyId;
        public string DisplayName => displayName;
        public string Category => category;
        public bool CountsAsTarget => countsAsTarget;
        public Sprite Icon => icon;
        public GameObject Prefab => prefab;
    }
}
