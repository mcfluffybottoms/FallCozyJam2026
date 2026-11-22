using System;
using UnityEngine;
using DG.Tweening;

namespace AnimationView
{
    public class FlowerAnimationModule
    {
        private Vector3 initialScale;
        private Transform visualTransform;
        private Tween returnTween;
        private Quaternion initialLocalRot;

        public FlowerAnimationModule(Transform visual, float flowerHeight = 1.2f)
        {
            visualTransform = visual;
            initialScale = visualTransform.localScale;
            initialLocalRot = visualTransform.localRotation;
        }

        public void Grow(float progressClamped)
        {
            visualTransform.localScale = Vector3.Lerp(Vector3.zero, initialScale, progressClamped);
        }

        public void PlayBloomPopEffect(float popOvershoot, float popDuration)
        {
            visualTransform.DOKill();
            visualTransform.localScale = initialScale;
            visualTransform
                .DOScale(initialScale * popOvershoot, popDuration * 0.4f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    visualTransform
                        .DOScale(initialScale, popDuration * 0.6f)
                        .SetEase(Ease.OutBack, 2f);
                });
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayPlantSound(visualTransform.position);
        }

        public void Push(Quaternion targetRotation, float smoothSpeed)
        {
            returnTween?.Kill();

            visualTransform.localRotation = Quaternion.Slerp(
                visualTransform.localRotation,
                targetRotation,
                smoothSpeed * Time.deltaTime
            );
        }

        public void OnHover(float returnOvershoot, float returnDuration)
        {
            returnTween?.Kill();

            returnTween = visualTransform
                .DOLocalRotateQuaternion(initialLocalRot, returnDuration)
                .SetEase(Ease.OutBack, returnOvershoot);
        }
    }
}