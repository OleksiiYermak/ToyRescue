using System.Collections.Generic;
using UnityEngine;

namespace ToyRescue.Activities
{
    [DisallowMultipleComponent]
    public sealed class ToyRegistry : MonoBehaviour
    {
        [SerializeField] private List<ToyInteractable> toys = new List<ToyInteractable>();
        [SerializeField] private bool autoRefreshFromChildrenOnAwake = true;

        public IReadOnlyList<ToyInteractable> Toys => toys;

        private void Awake()
        {
            EnsureToysRegistered();
        }

        [ContextMenu("Refresh From Children")]
        public void RefreshFromChildren()
        {
            ToyInteractable[] foundToys = GetComponentsInChildren<ToyInteractable>(true);
            toys.Clear();

            for (int i = 0; i < foundToys.Length; i++)
            {
                if (foundToys[i] != null)
                {
                    toys.Add(foundToys[i]);
                }
            }
        }

        public void EnsureToysRegistered()
        {
            if (toys.Count == 0 && autoRefreshFromChildrenOnAwake)
            {
                RefreshFromChildren();
            }
        }

        public void ResetToys()
        {
            EnsureToysRegistered();

            for (int i = 0; i < toys.Count; i++)
            {
                toys[i]?.ResetToy();
            }
        }
    }
}
