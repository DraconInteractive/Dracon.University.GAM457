using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Sense : MonoBehaviour
    {
        public float sightDist;
        Player player;
        public Transform eyePos;

        [Space]
        [Header("Debug")]
        public string debug;
        public float dist = 0, dot = 0;
        
        private void Start()
        {
            player = Player.player;
        }

        public bool PlayerDetect()
        {
            bool distCheck;
            dist = Vector3.Distance(transform.position, player.transform.position);
            distCheck = (dist < sightDist);

            if (distCheck)
            {
                bool frontCheck = false;
                Vector3 vectorToPlayer = player.transform.position - transform.position;
                dot = Vector3.Dot(vectorToPlayer.normalized, transform.forward);
                //0.5 = 30 degrees either side / 60 total
                frontCheck = (dot > 0.5f);
                if (frontCheck)
                {
                    RaycastHit hit;
                    if (Physics.Linecast(eyePos.position, player.transform.position + Vector3.up * 0.5f, out hit))
                    {
                        debug = "Hit: " + hit.transform.name;
                        bool hitCheck = (hit.transform.gameObject == player.transform.gameObject);
                        if (hitCheck)
                        {
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

