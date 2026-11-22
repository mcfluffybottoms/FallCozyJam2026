


using System.Collections.Generic;
using UnityEngine;

namespace Items.Actions
{
    public class WaterAction
    {
        private Collider2D[] hitColliders = new Collider2D[64];
        private readonly List<LevelItems.Flower> activeFlowers = new();
        private ContactFilter2D contactFilter;

        public WaterAction()
        {
            contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LevelGlobalConfig.FlowerLayer);
            contactFilter.useTriggers = true;
        }

        public void StartWater(Vector3 pos, float waterRadius)
        {
            activeFlowers.Clear();
            int count = Physics2D.OverlapCircle(
                pos,
                waterRadius,
                contactFilter,
                hitColliders
            );

            for (int i = 0; i < count; i++)
            {
                if (hitColliders[i].TryGetComponent<LevelItems.Flower>(out var flower) && flower)
                {
                    activeFlowers.Add(flower);

                }
            }
        }

        public void Water(Vector3 pos, float waterRadius)
        {
            StartWater(pos, waterRadius);
            for (int i = 0; i < activeFlowers.Count; i++)
            {
                if (activeFlowers[i] != null)
                {
                    activeFlowers[i].ContinueReceiveWater(pos);
                }
            }
        }

        public void StopWater(Vector3 pos, float waterRadius)
        {
            activeFlowers.Clear();
        }

    }
}