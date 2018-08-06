using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Squad : MonoBehaviour
    {
        public Melee_Enemy melee;
        public Blocker_Enemy blocker;
        public Ranged_Enemy ranged;

        public Vector3 meleePos, blockerPos, rangedPos;

        public Enemy.AlertLevel alertLevel;

        private float playerDetectionLevel;
        public float detectionGain, detectionLoss;

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

        private void Start()
        {
            melee.squad = this;
            blocker.squad = this;
            ranged.squad = this;
        }

        private void Update()
        {
            playerDetectionLevel -= detectionLoss * Time.deltaTime;

            ChangeAlertLevel();
            melee.alertLevel = alertLevel;
            blocker.alertLevel = alertLevel;
            ranged.alertLevel = alertLevel;
        }

        public POI GetPlayerIntent ()
        {
            //TODO: Adjust for priority
            POI target = null;
            float dist = Mathf.Infinity;
            float health = Player.player.health;
            foreach (POI p in POI.all)
            {
                float mod = 1;
                if (health < 50 && p is Health_Pickup)
                {
                    //prioritize for low health
                    mod = 1.2f;
                } else if (p is Key)
                {
                    //prioritize for main goal
                    mod = 1.2f;
                }
                float d = Vector3.Distance(p.transform.position, Player.player.transform.position) * mod;
                if (d < dist)
                {
                    dist = d;
                    target = p;
                }
            }

            return target;
        }

        void GetSquadPositions ()
        {
            POI target = GetPlayerIntent();

            meleePos = Player.player.transform.position;

            Vector3 playerToTarget = target.transform.position - Player.player.transform.position;
            Vector3 bP = Player.player.transform.position + playerToTarget.normalized * 3;
            blockerPos = bP;

            Vector3 rP = Player.player.transform.position + (playerToTarget * 0.5f) + Vector3.Cross(playerToTarget, Vector3.up) * 2f;
            rangedPos = rP;
        }

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

