using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    //Overarching logic on a tactical level. 
    public class Squad : MonoBehaviour
    {
        //The members of the squad. Rigid format at the moment, but not too hard to just make it a list of "SquadEnemy"'s
        public Melee_Enemy melee;
        public Blocker_Enemy blocker;
        public Ranged_Enemy ranged;
        //directions for squad members positions. Public so squad can poll for them from individual AI, as needed, rather than being fed them every frame. 
        public Vector3 meleePos, blockerPos, rangedPos;
        //Alert level of the squad. Required by squad enemies for their own detection scenarios. 
        public Enemy.AlertLevel alertLevel;
        
        private float playerDetectionLevel;
        public float detectionGain, detectionLoss;

        //The only player detection level that is a property, made to clamp upon being fed by squad members. 
        public float PlayerDetectionLevel
        {
            get
            {
                return playerDetectionLevel;
            }

            set
            {
                playerDetectionLevel = value;
                playerDetectionLevel = Mathf.Clamp(playerDetectionLevel, 0, 3);
            }
        }
        //debug string
        public string playerTargetFeedback;

        //When game starts, assign squad to members. Could do this in inspector, but this is more a placeholder for the List<Enemy> approach i want to implement. 
        private void Start()
        {
            melee.squad = this;
            blocker.squad = this;
            ranged.squad = this;
        }
        //Every update, lose a little bit of detection. Detection gain on the squad members is higher than this loss (1.2 > 1), so if the player is in sights, it wont interfere with detection. 
        //This just makes the squad stop searching after a while of the player being lost. 
        private void Update()
        {
            PlayerDetectionLevel -= detectionLoss * Time.deltaTime;
            //No behaviour tree here, check the levels, and assign the appropriate alert level. 
            ChangeAlertLevel();
            //Check if each member is alive, and if they are, set their detection level to that of the squad. 
            //Note that we do not set the actual alert level, the transition to this is handled by the individual AI. 
            if (melee != null)
            {
                melee.playerDetectionLevel = playerDetectionLevel;
            }
            
            if (blocker != null)
            {
                blocker.playerDetectionLevel = playerDetectionLevel;
            }
            
            if (ranged != null)
            {
                ranged.playerDetectionLevel = playerDetectionLevel;
            }
           
            //Get where the squad members should move. This does not feed to the members, but stores it in those 3 public V3's up the top. 
            //Seemed more optimal to call this once per frame on this, rather than have the AI call it possibly 3 times. 
            GetSquadPositions();
            //Set the debug string to the current player target. So I can see where the AI *thinks* the player is going. 
            playerTargetFeedback = GetPlayerIntent().name;
        }

        /// <summary>
        /// Sorts and culls the list of goals the player could want to go to, then iterates through the suitable remains to find the most probable place the player is going. 
        /// </summary>
        /// <returns></returns>
        public POI GetPlayerIntent ()
        {
            //Variable to store final product.
            POI target = null;
            //Distance comparative
            float dist = Mathf.Infinity;
            //More optimal to store this here than to call heaps below
            Player player = Player.player;

            //list to sort through
            List<POI> goals = new List<POI>(POI.all);

            //conditions
            bool removeHealth = false;
            bool removeKey = false;
            bool removePrincess = false;
            bool removeExit = false;

            //if player is on high health, they wont need a pickup, so disregard. 
            if (player.health > 50)
            {
                removeHealth = true;
            }

            //If the player hasnt got the key yet, disregard the exit or the princess as goals. (meaning key and health are goals)
            if (!Player.player.hasKey)
            {
                removeExit = true;
                removePrincess = true;
            }
            else
            {
                //If the player DOES have the key, but the key somehow didnt get destroyed, disregard it. 
                removeKey = true;
                //If the player has the key, AND the princess, disregard the princess (thus setting the only goals as the health and the exit.)
                //if the player has the key, but no princess, disregard the exit (setting goals to princess and health)
                if (Player.player.hasPrincess)
                {
                    removePrincess = true;
                }
                else
                {
                    removeExit = true;
                }
            }
            //Sort through the list and get the points that need removing
            List<POI> removals = new List<POI>();
            foreach (POI p in goals)
            {
                if (removeKey && p is Key)
                {
                    removals.Add(p);
                    continue;
                }

                if (removeHealth && p is Health_Pickup)
                {
                    removals.Add(p);
                    continue;
                }

                if (removePrincess && p is Princess)
                {
                    removals.Add(p);
                    continue;
                }

                if (removeExit && p is Exit)
                {
                    removals.Add(p);
                    continue;
                }
            }
            //remove points from consideration
            foreach (POI p in removals)
            {
                if (goals.Contains(p))
                {
                    goals.Remove(p);
                }
            }
            //sort through remaining points.
            foreach (POI p in goals)
            {
                float d = Vector3.Distance(p.transform.position, Player.player.transform.position);
                if (d < dist)
                {
                    dist = d;
                    target = p;
                }
            }
            //return final product
            return target;
        }

        /// <summary>
        /// Use player intent to sort where each squad member should be. 
        /// </summary>
        void GetSquadPositions ()
        {
            //Get the approximate target
            POI target = GetPlayerIntent();
            //Melee doesnt care, it just attacks. 
            meleePos = Player.player.transform.position;
            //Get the vector from the player to the goal, aka their approximate path. Could maybe do this better with an a* path, but this is more optimal for now. 
            Vector3 playerToTarget = target.transform.position - Player.player.transform.position;
            //blocking position is 1.5 meters away from the player, along the path to the goal / in the way. 
            Vector3 bP = Player.player.transform.position + playerToTarget.normalized * blocker.attackDistance * 1.5f;
            blockerPos = bP;
            //Ranged position is 2 meters along the path, but 1 meter to the side, so as to flank the player as they go for the goal. Maximum damage. 
            Vector3 rP = (Player.player.transform.position + (playerToTarget * 2)) + Vector3.Cross(playerToTarget, Vector3.up).normalized * 1f;
            rangedPos = rP;
        }

        /// <summary>
        /// Set alert level to the appropriate enum based on the player detection level position in a range. 
        /// </summary>
        public void ChangeAlertLevel()
        {
            
            if (playerDetectionLevel < 1)
            {
                alertLevel = Enemy.AlertLevel.Default;
            }
            else if (playerDetectionLevel >= 1 && playerDetectionLevel < 2)
            {
                alertLevel = Enemy.AlertLevel.Suspicious;
            }
            else if (playerDetectionLevel >= 2)
            {
                alertLevel = Enemy.AlertLevel.Alert;
            }
        }
    }
}

