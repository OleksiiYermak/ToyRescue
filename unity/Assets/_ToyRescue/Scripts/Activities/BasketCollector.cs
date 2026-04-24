using UnityEngine;

namespace ToyRescue.Activities
{
    [DisallowMultipleComponent]
    public sealed class BasketCollector : MonoBehaviour
    {
        [SerializeField] private Transform collectionPoint;
        [SerializeField] private Vector3 stackOffset = new Vector3(0.08f, 0.04f, 0.08f);

        private int collectedVisualCount;

        public void CollectToy(ToyInteractable toy)
        {
            if (toy == null)
            {
                return;
            }

            Transform anchor = collectionPoint != null ? collectionPoint : transform;
            Vector3 destination = anchor.position + anchor.TransformVector(stackOffset * collectedVisualCount);
            collectedVisualCount++;

            ToyMoveToBasket toyMoveToBasket = toy.GetMoveToBasket();
            if (toyMoveToBasket != null)
            {
                toyMoveToBasket.MoveTo(destination, anchor);
                return;
            }

            toy.transform.SetParent(anchor, true);
            toy.transform.position = destination;
        }

        public void ResetCollector()
        {
            collectedVisualCount = 0;
        }
    }
}
