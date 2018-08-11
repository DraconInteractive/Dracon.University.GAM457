using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class POI : MonoBehaviour
    {
        //global static list of all points of interest. 
        public static List<POI> all = new List<POI>();
        //should the poi be destroyed when picked up
        public bool destroyOnHit;

        private void Awake()
        {
            //add this to the global list
            all.Add(this);
        }

        //when the player enters the trigger, call the appropriate function. 
        void OnTriggerEnter (Collider col)
        {
            if (col.transform.gameObject == Player.player.gameObject)
            {
                OnHitPlayer();
            }
        }

        //when we hit the player run the base function, where we destroy the poi (if DoH is enabled)
        public virtual void OnHitPlayer ()
        {
            if (destroyOnHit)
            {
                Destroy(this.gameObject, 0.1f);
                //Remove this from the global list
                all.Remove(this);
            }
        }
    }
}

