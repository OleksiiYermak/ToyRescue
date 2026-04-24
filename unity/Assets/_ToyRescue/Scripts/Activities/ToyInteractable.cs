using System;
using ToyRescue.Data;
using UnityEngine;

namespace ToyRescue.Activities
{
    [DisallowMultipleComponent]
    public sealed class ToyInteractable : MonoBehaviour
    {
        [Header("Toy Data")]
        [SerializeField] private ToyDefinition toyDefinition;
        [SerializeField] private string toyIdOverride;
        [SerializeField] private string categoryOverride;
        [SerializeField] private bool useCountsAsTargetOverride;
        [SerializeField] private bool countsAsTargetOverride = true;

        private Collider[] cachedColliders;
        private ToyMoveToBasket toyMoveToBasket;

        public event Action<ToyInteractable> Selected;

        public ToyDefinition ToyDefinition => toyDefinition;
        public bool IsCollected { get; private set; }
        public string ToyId => toyDefinition != null && !string.IsNullOrWhiteSpace(toyDefinition.ToyId)
            ? toyDefinition.ToyId
            : !string.IsNullOrWhiteSpace(toyIdOverride)
                ? toyIdOverride
                : name;
        public string DisplayName => toyDefinition != null && !string.IsNullOrWhiteSpace(toyDefinition.DisplayName)
            ? toyDefinition.DisplayName
            : name;
        public string Category => !string.IsNullOrWhiteSpace(categoryOverride)
            ? categoryOverride
            : toyDefinition != null
                ? toyDefinition.Category
                : string.Empty;
        public bool CountsAsTarget => useCountsAsTargetOverride || toyDefinition == null
            ? countsAsTargetOverride
            : toyDefinition.CountsAsTarget;

        private void Awake()
        {
            cachedColliders = GetComponentsInChildren<Collider>(true);
            toyMoveToBasket = GetComponent<ToyMoveToBasket>();
        }

        private void OnMouseUpAsButton()
        {
            RequestSelection();
        }

        public void RequestSelection()
        {
            if (IsCollected || !isActiveAndEnabled)
            {
                return;
            }

            Selected?.Invoke(this);
        }

        public void MarkCollected()
        {
            IsCollected = true;
            SetCollidersEnabled(false);
        }

        public void ResetToy()
        {
            IsCollected = false;
            gameObject.SetActive(true);
            toyMoveToBasket?.ResetToInitialPose();
            SetCollidersEnabled(true);
        }

        public ToyMoveToBasket GetMoveToBasket()
        {
            return toyMoveToBasket;
        }

        private void SetCollidersEnabled(bool isEnabled)
        {
            if (cachedColliders == null || cachedColliders.Length == 0)
            {
                cachedColliders = GetComponentsInChildren<Collider>(true);
            }

            for (int i = 0; i < cachedColliders.Length; i++)
            {
                if (cachedColliders[i] != null)
                {
                    cachedColliders[i].enabled = isEnabled;
                }
            }
        }
    }
}
