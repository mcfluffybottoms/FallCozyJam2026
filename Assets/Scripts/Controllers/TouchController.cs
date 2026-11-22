using System.Collections.Generic;
using LevelItems;
using UnityEngine;

namespace Controllers
{

    public class TouchController : MonoBehaviour
    {
        [SerializeField] private float touchRadius = 1.6f;
        [SerializeField] private new Camera camera;
        private readonly Collider2D[] hitBuffer = new Collider2D[32];
        [SubclassSelector] private readonly HashSet<IInteractable> currentInteractables = new();
        [SubclassSelector] private readonly HashSet<IInteractable> previousInteractables = new();

        private ContactFilter2D contactFilter;

        private void Awake()
        {
            if (camera == null)
                camera = Camera.main;

            contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LevelGlobalConfig.WeedFlowerLayer);
            contactFilter.useTriggers = true;
        }
        private void Update()
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();

            int count = Physics2D.OverlapCircle(
                mouseWorldPos,
                touchRadius,
                contactFilter,
                hitBuffer
            );
            for (int i = 0; i < count; i++)
            {
                if (hitBuffer[i].TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.Push(mouseWorldPos, touchRadius);
                    currentInteractables.Add(interactable);
                    previousInteractables.Remove(interactable);
                }
            }

            foreach (var interactable in previousInteractables)
            {
                if (interactable != null)
                {
                    interactable.OnHover();
                }
            }
        }
        private Vector3 GetMouseWorldPosition()
        {
            if (camera == null) return Vector3.zero;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new(Vector3.forward, Vector3.zero);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                worldPoint.z = 0f;
                return worldPoint;
            }

            return Vector3.zero;
        }
    }
}

