using AnimationView;
using UnityEditor;
using UnityEngine;

namespace LevelItems
{
    public enum WeedType { Common, Large }
    public class Weed : MonoBehaviour, IInteractable
    {
        [Header("Weed Type")]
        [SerializeField] private WeedType weedType = WeedType.Common;
        [Header("Weed Properties")]
        [SerializeField] private float pullDistanceThreshold = 2.0f;
        [SerializeField] private float maxVisualLift = 0.6f;
        [SerializeField] private float followSpeed = 0.5f;
        [SerializeField] private float pluckPower = 2f;
        private float personalToughness;

        [Header("Weed Reward")]
        [SerializeField] private int commonReward = 5;
        [SerializeField] private int largeReward = 10;

        private WeedAnimationModule animationModule;

        // current weed state
        private bool isGrabbed;
        public bool IsPlucked { get; private set; }
        private bool needFollowHand = false;
        private Vector3 startMousePos;

        public WeedType Type => weedType;
        public int Reward => weedType == WeedType.Large ? largeReward : commonReward;

        public event System.Action<Weed> OnPlucked;

        private void Awake()
        {
            animationModule = new WeedAnimationModule(this);
            float baseToughness = weedType == WeedType.Large ? 1.3f : 1.0f;
            personalToughness = Random.Range(0.85f, 1.3f) * baseToughness;
        }

        public void Press(Vector3 pos)
        {
            if (IsPlucked || isGrabbed) return;
            isGrabbed = true;
            startMousePos = pos;
            animationModule.InitGrab(pos);
        }

        public void Hold(Vector3 pos, float grabRadius)
        {
            if (needFollowHand)
            {
                animationModule.FollowHand(pos, followSpeed);
                return;
            }

            if (!isGrabbed || IsPlucked) return;

            float deltaY = Mathf.Max(0, pos.y - startMousePos.y);
            float progress = Mathf.Clamp01(deltaY / (pullDistanceThreshold * personalToughness)); // move to weed
            animationModule.TryPluck(progress, maxVisualLift);

            if (progress >= 1f)
            {
                IsPlucked = true;
                needFollowHand = true;
                animationModule.RipAndAttachToHand(grabRadius, pluckPower);
                OnPlucked?.Invoke(this);
            }
        }

        public void Release(Vector3 pos)
        {
            isGrabbed = false;
            needFollowHand = false;
            animationModule.Release(IsPlucked);
        }

        public void Push(Vector3 position, float radius)
        {
        }
    }
}