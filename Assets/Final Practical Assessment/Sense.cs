using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    //class for detection functions. Only vision right now, with space for audio input. 
    public class Sense : MonoBehaviour
    {
        //how far can the player see
        public float sightDist;
        //reference to the player
        Player player;
        //Transform to base eye ray from
        public Transform eyePos;
        //position of where the player was last seen
        public Vector3 lastPlayerPos;

        //debug string for ... debug
        [Space]
        [Header("Debug")]
        public string debug;
        //distance from player, and dot product. Made them class wide and public so i could see them from the inspector. 
        public float dist = 0, dot = 0;
        
        private void Start()
        {
            //get the player singleton reference
            player = Player.player;
        }
        //The main detection function. 
        public bool PlayerDetect()
        {
            //Check distance from player. if it is within sightRange, continue
            bool distCheck;
            dist = Vector3.Distance(transform.position, player.transform.position);
            distCheck = (dist < sightDist);

            if (distCheck)
            {
                //Get the dot product of the player compared to the enemy. If it is above 0.5, the player is in a 60 degree cone in front of the player
                //if the player is in the cone, continue
                bool frontCheck = false;
                Vector3 vectorToPlayer = player.transform.position - transform.position;
                dot = Vector3.Dot(vectorToPlayer.normalized, transform.forward);
                //0.5 = 30 degrees either side / 60 total
                frontCheck = (dot > 0.5f);
                if (frontCheck)
                {
                    //Do a linecast from the enemy to the player to see if sight is obstructed. 
                    RaycastHit hit;
                    if (Physics.Linecast(eyePos.position, player.transform.position + Vector3.up * 0.5f, out hit))
                    {
                        debug = "Hit: " + hit.transform.name;
                        //if the linecast hits nothing but the player, return detected, and save the players position. 
                        bool hitCheck = (hit.transform.gameObject == player.transform.gameObject);
                        if (hitCheck)
                        {
                            lastPlayerPos = hit.point;
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}

