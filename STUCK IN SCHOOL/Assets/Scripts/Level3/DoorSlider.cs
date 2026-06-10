using System.Collections;
using UnityEngine;

namespace NavKeypad
{
    public class DoorSlider : MonoBehaviour
    {
        public Vector3 slideDirection = Vector3.back;
        public float slideDistance = 0.8f;
        public float slideSpeed = 2f;

        private Vector3 closedPos;
        private bool isOpen = false;

        private void Awake()
        {
            closedPos = transform.localPosition;
        }

        public void Configure(Vector3 direction, float distance, float speed)
        {
            slideDirection = direction;
            slideDistance = distance;
            slideSpeed = speed;
        }

        public void Open()
        {
            if (isOpen) return;
            StopAllCoroutines();
            StartCoroutine(SlideTo(closedPos + slideDirection.normalized * slideDistance));
            isOpen = true;
        }

        public void Close()
        {
            if (!isOpen) return;
            StopAllCoroutines();
            StartCoroutine(SlideTo(closedPos));
            isOpen = false;
        }

        private IEnumerator SlideTo(Vector3 target)
        {
            while (Vector3.Distance(transform.localPosition, target) > 0.001f)
            {
                transform.localPosition = Vector3.MoveTowards(
                    transform.localPosition, target, slideSpeed * Time.deltaTime);
                yield return null;
            }
            transform.localPosition = target;
        }
    }
}
