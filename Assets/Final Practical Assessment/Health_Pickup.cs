using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Health_Pickup : POI
    {
        public float healAmount;
        void Update()
        {
            transform.Rotate(Vector3.up, 65 * Time.deltaTime);
        }

        public override void OnHitPlayer()
        {
            base.OnHitPlayer();
            Player.player.Heal(healAmount);
        }
    }
}

