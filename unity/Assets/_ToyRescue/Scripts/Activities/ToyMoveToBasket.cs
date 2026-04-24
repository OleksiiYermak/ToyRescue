using System.Collections;
using UnityEngine;

namespace ToyRescue.Activities
{
    [DisallowMultipleComponent]
    public sealed class ToyMoveToBasket : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float moveDuration = 0.35f;
        [SerializeField, Min(0f)] private float hopHeight = 0.25f;
        [SerializeField] private bool matchParentRotationOnArrival;

        private Transform initialParent;
        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;
        private Vector3 initialLocalScale;
        private Coroutine moveRoutine;

        private void Awake()
        {
            CaptureInitialPose();
        }

        public void MoveTo(Vector3 worldDestination, Transform parentAfterMove = null)
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }

            moveRoutine = StartCoroutine(MoveRoutine(worldDestination, parentAfterMove));
        }

        public void ResetToInitialPose()
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
                moveRoutine = null;
            }

            transform.SetParent(initialParent, false);
            transform.localPosition = initialLocalPosition;
            transform.localRotation = initialLocalRotation;
            transform.localScale = initialLocalScale;
            gameObject.SetActive(true);
        }

        private void CaptureInitialPose()
        {
            initialParent = transform.parent;
            initialLocalPosition = transform.localPosition;
            initialLocalRotation = transform.localRotation;
            initialLocalScale = transform.localScale;
        }

        private IEnumerator MoveRoutine(Vector3 worldDestination, Transform parentAfterMove)
        {
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = matchParentRotationOnArrival && parentAfterMove != null
                ? parentAfterMove.rotation
                : startRotation;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / moveDuration);
                Vector3 nextPosition = Vector3.Lerp(startPosition, worldDestination, progress);
                nextPosition.y += Mathf.Sin(progress * Mathf.PI) * hopHeight;

                transform.position = nextPosition;
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, progress);
                yield return null;
            }

            if (parentAfterMove != null)
            {
                transform.SetParent(parentAfterMove, true);
            }

            transform.position = worldDestination;
            transform.rotation = endRotation;
            moveRoutine = null;
        }
    }
}
