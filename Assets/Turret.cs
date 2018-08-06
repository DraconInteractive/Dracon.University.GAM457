using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    [RequireComponent(typeof(Sense))]
    public class Turret : MonoBehaviour
    {
        Player player;
        Sense sense;
        AI_Behaviour tree;
        public Renderer signalRenderer;
        public Transform gunPivot;

        public float playerDetectionLevel;
        public float detectionChangeModifier;

        public float attackDistance;
        public enum AlertLevel
        {
            Default,
            Suspicious,
            Alert
        }
        public AlertLevel alertLevel;

        public Coroutine actionRoutine;

        public AudioSource turretActiveAudio;
        public AudioClip defaultAudio, suspiciousAudio, alertAudio, fireAudio;
        private void Awake()
        {
            sense = GetComponent<Sense>();
            tree = GetComponent<AI_Behaviour>();
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

        void EvaluateDetection ()
        {
            if (sense.PlayerDetect())
            {
                playerDetectionLevel += Time.deltaTime * detectionChangeModifier;
            }
            else
            {
                if (playerDetectionLevel > 0)
                {
                    playerDetectionLevel -= Time.deltaTime * detectionChangeModifier;
                }
            }

            playerDetectionLevel = Mathf.Clamp(playerDetectionLevel, 0, 2);
        }

        public void ClearAction ()
        {
            if (actionRoutine != null)
            {
                StopCoroutine(actionRoutine);
            }
        }

        public void SetAlertLevel (AlertLevel newLevel)
        {
            switch (newLevel)
            {
                case AlertLevel.Default:
                    turretActiveAudio.PlayOneShot(defaultAudio);
                    break;
                case AlertLevel.Suspicious:
                    turretActiveAudio.PlayOneShot(suspiciousAudio);
                    break;
                case AlertLevel.Alert:
                    turretActiveAudio.PlayOneShot(alertAudio);
                    break;
            }

            alertLevel = newLevel;
            
        }
    }
}

