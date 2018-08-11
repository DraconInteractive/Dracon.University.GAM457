using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    //health pickup
    public class Health_Pickup : POI
    {
        //how much does the pickup heal the player for
        public float healAmount;
        void Update()
        {
            //spin, because spinning things are cool
            transform.Rotate(Vector3.up, 65 * Time.deltaTime);
        }
        //when picked up, call the heal function on the player
        public override void OnHitPlayer()
        {
            base.OnHitPlayer();
            Player.player.Heal(healAmount);
            
        }
    }
}

