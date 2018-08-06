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

        public float coolDown, damage;
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
                    playerDetectionLevel -= Time.deltaTime * detectionChangeModifier * 0.5f;
                }
            }

            playerDetectionLevel = Mathf.Clamp(playerDetectionLevel, 0, 3);
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
            signalRenderer.material.EnableKeyword("_EMISSION");
            switch (newLevel)
            {
                case AlertLevel.Default:
                    turretActiveAudio.PlayOneShot(defaultAudio);
                    signalRenderer.material.color = Color.blue;
                    signalRenderer.material.SetColor("_EmissionColor", Color.blue * 3);
                    break;
                case AlertLevel.Suspicious:
                    turretActiveAudio.PlayOneShot(suspiciousAudio);
                    signalRenderer.material.color = Color.yellow;
                    signalRenderer.material.SetColor("_EmissionColor", Color.yellow * 3);
                    break;
                case AlertLevel.Alert:
                    turretActiveAudio.PlayOneShot(alertAudio);
                    signalRenderer.material.color = Color.red;
                    signalRenderer.material.SetColor("_EmissionColor", Color.red * 3);
                    break;
            }

            alertLevel = newLevel;
            
        }

        public IEnumerator Attack ()
        {
            while (true)
            {
                RaycastHit hit;
                if (Physics.Linecast (transform.position, player.transform.position, out hit))
                {
                    if (hit.transform.gameObject == player.transform.gameObject)
                    {
                        player.Damage(damage);
                        turretActiveAudio.PlayOneShot(fireAudio);
                    }
                }

                yield return new WaitForSeconds(coolDown);
            }
            yield break;
        }
    }
}

