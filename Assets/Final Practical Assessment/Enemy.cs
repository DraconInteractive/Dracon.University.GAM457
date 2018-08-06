using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Enemy : MonoBehaviour
    {
        [HideInInspector]
        public Player player;
        [HideInInspector]
        public Sense sense;
        [HideInInspector]
        public AI_Behaviour tree;

        public static List<Enemy> all;

        public enum AlertLevel
        {
            Default,
            Suspicious,
            Alert
        }
        public AlertLevel alertLevel;

        public float playerDetectionLevel;
        public float detectionGain, detectionLoss;

        public Coroutine actionRoutine;

        private void Awake()
        {
            sense = GetComponent<Sense>();
            tree = GetComponent<AI_Behaviour>();
            all.Add(this);
        }

        // Use this for initialization
        void Start()
        {
            player = Player.player;
        }

        // Update is called once per frame
        void Update()
        {
            EvaluateDetection();
            tree.EvaluateTree();
        }

        void EvaluateDetection()
        {
            if (sense.PlayerDetect())
            {
                playerDetectionLevel += Time.deltaTime * detectionGain;
            }
            else
            {
                if (playerDetectionLevel > 0)
                {
                    playerDetectionLevel -= Time.deltaTime * detectionLoss;
                }
            }

            playerDetectionLevel = Mathf.Clamp(playerDetectionLevel, 0, 3);
        }

        public void AlertAllInRange(float range)
        {
            foreach (Enemy e in all)
            {
                if (Vector3.Distance(e.transform.position, transform.position) < range)
                {
                    e.playerDetectionLevel = 3;
                }
            }
        }

        public virtual void SetAlertLevel(AlertLevel newLevel)
        {

            alertLevel = newLevel;

        }

        public void ClearAction()
        {
            if (actionRoutine != null)
            {
                StopCoroutine(actionRoutine);
            }
        }

        public virtual void Attack ()
        {

        }
    }

    
}

