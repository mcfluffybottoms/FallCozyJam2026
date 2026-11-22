
using UnityEngine;
using DG.Tweening;
using System.ComponentModel.Design;

namespace AnimationView
{
    public class WeedAnimationModule
    {
        // properties recieved to manipulate object
        private Transform visualTransform;
        private Rigidbody2D rb;
        private Collider2D collider;

        // positions (from transform initial)
        private Vector3 initialVisualPos;
        private Vector3 initialVisualScale;

        // positions (from transform initial)
        private Vector3 handOffset;

        public WeedAnimationModule(MonoBehaviour owner)
        {
            visualTransform = owner.transform;
            rb = owner.GetComponent<Rigidbody2D>();
            collider = owner.GetComponent<Collider2D>();

            initialVisualPos = visualTransform.localPosition;
            initialVisualScale = visualTransform.localScale;

            if (rb != null) rb.bodyType = RigidbodyType2D.Static;
        }

        public void InitGrab(Vector3 mouseStart)
        {
            visualTransform.DOKill();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayPullingSound(visualTransform.position);
        }

        public bool TryPluck(float progress, float maxVisualLift)
        {
            float visualY = Mathf.Sin(progress * Mathf.PI * 0.5f) * maxVisualLift;
            float jitter = progress > 0.4f ? Random.Range(-0.02f, 0.02f) * progress : 0f;
            visualTransform.localPosition = initialVisualPos + new Vector3(jitter, visualY, 0);

            visualTransform.localScale = new Vector3(
                initialVisualScale.x * (1f - progress * 0.2f),
                initialVisualScale.y * (1f + progress * 0.35f),
                initialVisualScale.z
            );

            return progress >= 1f;
        }

        public void RipAndAttachToHand(float radius, float pluckPower)
        {
            visualTransform.DOKill();

            Vector2 randomOffset = Random.insideUnitCircle * radius;
            handOffset = (Vector3)randomOffset;

            Vector3 popUpOffset = initialVisualPos + new Vector3(Random.Range(-pluckPower, pluckPower), 0.6f, 0f);
            Sequence snapSeq = DOTween.Sequence();
            snapSeq.Append(visualTransform.DOScale(initialVisualScale * 1.15f, 0.1f));
            snapSeq.Append(visualTransform.DOLocalMove(popUpOffset, 0.2f).SetEase(Ease.OutBack));
            snapSeq.Join(visualTransform.DOScale(initialVisualScale, 0.2f));
            snapSeq.Join(visualTransform.DORotate(new Vector3(0, 0, Random.Range(-30f, 30f)), 0.2f));

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayPullingSound(visualTransform.position);
        }

        public void FollowHand(Vector3 pos, float followSpeed)
        {
            Vector3 targetPos = pos + handOffset;
            visualTransform.position = Vector3.Lerp(visualTransform.position, targetPos, followSpeed * Time.deltaTime);
        }

        public void Release(bool isPlucked)
        {
            visualTransform.DOKill();
            if (!isPlucked)
            {
                ReleaseUnplucked();
            }
            else
            {
                ReleasePlucked();
            }
        }

        private void ReleasePlucked()
        {
            visualTransform.DOKill();
            if (collider != null) collider.isTrigger = true;

            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 3f));
                rb.angularVelocity = Random.Range(-180f, 180f);
            }
            else
            {
                visualTransform.DOMoveY(visualTransform.position.y - 10f, 1.2f).SetEase(Ease.InQuad);
            }
        }

        private void ReleaseUnplucked()
        {
            visualTransform.DOKill();
            Sequence bounceBack = DOTween.Sequence();
            bounceBack.Append(visualTransform.DOLocalMove(initialVisualPos, 0.25f).SetEase(Ease.OutBounce));
            bounceBack.Join(visualTransform.DOScale(initialVisualScale, 0.25f).SetEase(Ease.OutBounce));
        }

    }
}
