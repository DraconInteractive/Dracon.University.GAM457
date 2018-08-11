using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    //Exit point of interest
    public class Exit : POI
    {
        //when "pickup up"
        public override void OnHitPlayer()
        {
            base.OnHitPlayer();
            //do win condition if the player hits the goal when they have already saved the princess. 
            if (Player.player.hasPrincess)
            {
                print("We won!");
            }
        }
    }
}

