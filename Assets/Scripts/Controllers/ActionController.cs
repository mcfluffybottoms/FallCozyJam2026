using System.Collections.Generic;
using LevelItems;
using UnityEngine;

namespace Controllers
{
    public class ActionController : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        private readonly List<IInteractable> currentInteractables = new();

        // controller state
        private bool isPerformingAction;
        private Vector3 startMousePos;
        private Vector3 startGrabPos;

        // inventory
        [SerializeField] private ItemManager itemManager;

        private void Awake()
        {
            if (camera == null)
                camera = Camera.main;
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

        private void Update()
        {
            if (camera == null) return;
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            mouseWorldPos.z = 0;
            HandleInput(mouseWorldPos);
        }
        private void HandleInput(Vector3 mouseWorldPos)
        {
            if (Input.GetMouseButtonDown(0))
            {
                itemManager.GetActiveItem().Press(mouseWorldPos);
            }

            if (Input.GetMouseButton(0))
            {
                itemManager.GetActiveItem().Hold(mouseWorldPos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                itemManager.GetActiveItem().Release(mouseWorldPos);
            }
        }

        public void Press()
        {
            isPerformingAction = true;
        }

        public void Release()
        {
            if (!isPerformingAction) return;

            isPerformingAction = false;
        }

        public void Cancel()
        {
            if (!isPerformingAction) return;

            isPerformingAction = false;
        }

        public void Hold()
        {
            if (!isPerformingAction) return;
        }
    }

}

