using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class POI : MonoBehaviour
    {
        public static List<POI> all = new List<POI>();

        public bool destroyOnHit;
        private void Awake()
        {
            all.Add(this);
        }

        void OnTriggerEnter (Collider col)
        {
            if (col.transform.gameObject == Player.player.gameObject)
            {
                OnHitPlayer();
            }
        }

        public virtual void OnHitPlayer ()
        {
            if (destroyOnHit)
            {
                Destroy(this.gameObject, 0.1f);
                all.Remove(this);
            }
        }
    }
}

