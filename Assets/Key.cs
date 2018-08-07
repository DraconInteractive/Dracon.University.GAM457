using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Key : POI
    {
        public GameObject wallToDestroy;
        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, 65 * Time.deltaTime);
        }

        public override void OnHitPlayer ()
        {
            base.OnHitPlayer();
            Destroy(wallToDestroy);
        }
    }
}

