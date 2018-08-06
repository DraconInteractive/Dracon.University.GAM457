using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Squad : MonoBehaviour
    {
        public Melee_Enemy melee;
        public Blocker_Enemy blocker;
        public Ranged_Enemy ranged;

        public POI GetPlayerIntent ()
        {
            //TODO: Adjust for priority
            POI target = null;
            float dist = Mathf.Infinity;
            foreach (POI p in POI.all)
            {
                float d = Vector3.Distance(p.transform.position, Player.player.transform.position);
                if (d < dist)
                {
                    dist = d;
                    target = p;
                }
            }

            return target;
        }
    }
}

