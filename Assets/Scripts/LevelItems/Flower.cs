using AnimationView;
using UnityEngine;

namespace LevelItems
{
    public class Flower : MonoBehaviour, IInteractable
    {
        FlowerAnimationModule animationModule;

        [Header("Geometry")]
        [SerializeField] private float flowerHeight = 1.2f;
        [SerializeField] private float maxTiltAngle = 25f;
        [SerializeField] private float deadZoneWidth = 0.25f;
        [SerializeField] private Transform visual;
        [Header("Time")]
        [SerializeField] private float currentProgress = 0f;
        [SerializeField] private float smoothSpeed = 7f;
        [SerializeField] private float growDuration = 1.5f;
        [SerializeField] private float returnDuration = 0.55f;
        [SerializeField] private float returnOvershoot = 1.15f;
        [SerializeField] private float popOvershoot = 1.25f;
        [SerializeField] private float popDuration = 0.35f;

        private bool isGrowing = true;
        private bool isFullyGrown = false;
        public bool IsFullyGrown => isFullyGrown;

        // flower state 
        private Vector3 headPosition;
        private bool isBent;

        private void Awake()
        {
            // transform.localScale = Vector3.zero;
            animationModule = new(visual);
            visual.localScale = Vector3.zero;
            headPosition = transform.position + transform.up * flowerHeight;
        }

        public void StartReceiveWater(Vector3 pos)
        {
        }

        public void ContinueReceiveWater(Vector3 pos)
        {
            if (!isGrowing || isFullyGrown) return;
            currentProgress += Time.deltaTime / growDuration;
            float progressClamped = Mathf.Clamp01(currentProgress);
            animationModule.Grow(progressClamped);
            if (progressClamped >= 1f)
            {
                isGrowing = false;
                isFullyGrown = true;
                animationModule.PlayBloomPopEffect(popOvershoot, popDuration);
            }
        }
        public void StopReceiveWater(Vector3 pos)
        {
        }
        public void Push(Vector3 pos, float influenceRadius)
        {
            Vector2 dirFromMouse = headPosition - pos;
            float distance = dirFromMouse.magnitude;
            if (distance >= influenceRadius) return;

            isBent = true;

            float normalizedDist = distance / influenceRadius;
            float force = Mathf.SmoothStep(1f, 0f, normalizedDist);
            float side = Mathf.Clamp(dirFromMouse.x / deadZoneWidth, -1f, 1f);
            float targetAngle = -side * maxTiltAngle * force;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            animationModule.Push(targetRotation, smoothSpeed);
        }

        public void OnHover()
        {
            if (!isBent) return;
            isBent = false;
            animationModule.OnHover(returnOvershoot, returnDuration);
        }
    }
}