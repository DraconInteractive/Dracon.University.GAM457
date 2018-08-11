using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Exit : POI
    {
        public override void OnHitPlayer()
        {
            base.OnHitPlayer();
            if (Player.player.hasPrincess)
            {
                print("We won!");
            }
        }
    }
}

