using System.Diagnostics;
using UnityEngine;

namespace LevelItems
{
    public interface IInteractable
    {
        public void Push(Vector3 position, float radius) { }
        public void OnHover() { }
    }
}