
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Items.Actions
{
    public class GrabAction
    {
        private Collider2D[] hitColliders = new Collider2D[64];
        readonly List<LevelItems.Weed> activeWeeds = new();
        private ContactFilter2D contactFilter;

        public GrabAction()
        {
            contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LevelGlobalConfig.WeedLayer);
            contactFilter.useTriggers = true;
        }
        public void Pull(Vector3 pos, float grabRadius)
        {
            activeWeeds.Clear();
            int count = Physics2D.OverlapCircle(
                pos,
                grabRadius,
                contactFilter,
                hitColliders
            );
            for (int i = 0; i < count; i++)
            {
                if (hitColliders[i].TryGetComponent<LevelItems.Weed>(out var weed) && !weed.IsPlucked)
                {
                    weed.Press(pos);
                    activeWeeds.Add(weed);
                }
            }
        }

        public void Move(Vector3 pos, float radius)
        {
            for (int i = 0; i < activeWeeds.Count; i++)
            {
                if (activeWeeds[i] != null)
                {
                    activeWeeds[i].Hold(pos, radius);
                }
            }
        }

        public void Release(Vector3 pos)
        {
            for (int i = 0; i < activeWeeds.Count; i++)
            {
                if (activeWeeds[i] != null)
                {
                    activeWeeds[i].Release(pos);
                }
            }
            activeWeeds.Clear();
        }
    }
}