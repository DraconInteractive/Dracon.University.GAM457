using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        [HideInInspector]
        public NavMeshAgent agent;

        public static List<Enemy> all = new List<Enemy>();

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

        public float attackDistance, rotationSpeed;

        public bool hasSignal;
        public Renderer signalRenderer;


        private void Awake()
        {
            sense = GetComponent<Sense>();
            tree = GetComponent<AI_Behaviour>();
            agent = GetComponent<NavMeshAgent>();
            all.Add(this);
            EnemyAwake();
        }

        public virtual void EnemyAwake ()
        {

        }

        // Use this for initialization
        void Start()
        {
            player = Player.player;

            EnemyStart();
        }

        public virtual void EnemyStart ()
        {

        }
        // Update is called once per frame
        void Update()
        {
            EvaluateDetection();
            tree.EvaluateTree();

            if (Input.GetKeyDown (KeyCode.L))
            {
                if (this is Turret)
                {
                    (this as Turret).StartShake();
                    (this as Turret).StartPulse();
                }
            }
        }

        public virtual void EvaluateDetection()
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

            switch (newLevel)
            {
                case AlertLevel.Default:
                    SetSignal(Color.blue, 3);
                    break;
                case AlertLevel.Suspicious:
                    SetSignal(Color.yellow, 3);
                    break;
                case AlertLevel.Alert:
                    SetSignal(Color.red, 3);
                    break;
            }
        }

        public void ClearAction(bool stopMovement)
        {
            if (actionRoutine != null)
            {
                StopCoroutine(actionRoutine);
            }

            if (stopMovement)
            {
                agent.SetDestination(transform.position);
            }
        }

        public virtual void Attack ()
        {
            
        }

        public void MoveToStatic (Vector3 point)
        {
            agent.SetDestination(point);
        }

        public void MoveToDynamic (Transform target)
        {
            actionRoutine = StartCoroutine(DynamicMove(target));
        }

        IEnumerator DynamicMove (Transform target)
        {
            while (true)
            {
                agent.SetDestination(target.position);
                yield return new WaitForSeconds(0.1f);
            }
            yield break;
        }
        public void GoToLastSeenPlayer ()
        {
            agent.SetDestination(sense.lastPlayerPos);
        }

        public void SetSignal (Color c, float emissive)
        {
            if (hasSignal)
            {
                signalRenderer.material.EnableKeyword("_EMISSION");
                signalRenderer.material.color = c;
                signalRenderer.material.SetColor("_EmissionColor", c * emissive);
            }
        }

        public virtual void Die ()
        {
            Destroy(this.gameObject, 0.2f);
        }

        public void FacePlayer ()
        {
            actionRoutine = StartCoroutine(TurnToPlayer());
        }

        IEnumerator TurnToPlayer ()
        {
            while (true)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Player.player.transform.position - transform.position, Vector3.up), rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    
}

