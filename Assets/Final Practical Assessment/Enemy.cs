using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Final
{
    //base level enemy class
    public class Enemy : MonoBehaviour
    {
        //useful references to attached scripts and components
        [HideInInspector]
        public Player player;
        [HideInInspector]
        public Sense sense;
        [HideInInspector]
        public AI_Behaviour tree;
        [HideInInspector]
        public NavMeshAgent agent;
        //static list of enemies. 
        public static List<Enemy> all = new List<Enemy>();
        //Alert level of the enemy
        public enum AlertLevel
        {
            Default,
            Suspicious,
            Alert
        }
        public AlertLevel alertLevel;

        //store the current ai actions. 
        public Coroutine actionRoutine;
        //detection variables, including current level, gain and loss floats.
        [Space]
        [Header("Detection")]
        public float playerDetectionLevel;
        public float detectionGain, detectionLoss;

        //self explanatory variables
        [Space]
        [Header("Properties")]
        public float attackDistance;
        public float rotationSpeed;

        //variables for the glowy cube to change according to alert level
        [Space]
        [Header("State Communication")]
        public bool hasSignal;
        public Renderer signalRenderer;
        
        //audiosource for audio output, as well as relevant audio clips. 
        [Space]
        [Header("Audio")]
        public bool hasAudio;
        public AudioSource activeAudio;
        public AudioClip defaultAudio, suspiciousAudio, alertAudio, fireAudio;

        private void Awake()
        {
            //Get the attached components
            sense = GetComponent<Sense>();
            tree = GetComponent<AI_Behaviour>();
            agent = GetComponent<NavMeshAgent>();
            //add this to the global container
            all.Add(this);
            //run the virtual enemy awake function
            EnemyAwake();
        }
        //aforementioned overridable awake function
        public virtual void EnemyAwake ()
        {

        }

        //get the player instance, then run the virtual start function
        void Start()
        {
            player = Player.player;

            EnemyStart();
        }

        //aforementioned overridable enemy start function
        public virtual void EnemyStart ()
        {

        }
        // Update is called once per frame
        void Update()
        {
            //Run detection for player
            EvaluateDetection();
            //find what action we should be running
            tree.EvaluateTree();

        }

        //function to detect player
        public virtual void EvaluateDetection()
        {
            //detection is outsourced to another component, so tell that component 
            if (sense.PlayerDetect())
            {
                //player detected, increase our counter by the gain variable. 
                playerDetectionLevel += Time.deltaTime * detectionGain;
            }
            else
            {
                //if not, reduce how detected the player is. 
                if (playerDetectionLevel > 0)
                {
                    playerDetectionLevel -= Time.deltaTime * detectionLoss;
                }
            }
            //clamp the detection level. 
            playerDetectionLevel = Mathf.Clamp(playerDetectionLevel, 0, 3);
        }

        //if need be, make every enemy in range of the player go straight to level 3, combat phase. 
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
        //set the alert level to the supplied variable
        public virtual void SetAlertLevel(AlertLevel newLevel)
        {
            //change the local storage
            alertLevel = newLevel;
            //depending on the new level, set the signal color and play the appropriate audio.
            switch (newLevel)
            {
                case AlertLevel.Default:
                    SetSignal(Color.blue, 3);
                    if (hasAudio)
                    {
                        activeAudio.PlayOneShot(defaultAudio);
                    }
                    break;
                case AlertLevel.Suspicious:
                    SetSignal(Color.yellow, 3);
                    if (hasAudio)
                    {
                        activeAudio.PlayOneShot(suspiciousAudio);
                    }
                    break;
                case AlertLevel.Alert:
                    SetSignal(Color.red, 3);
                    if (hasAudio)
                    {
                        activeAudio.PlayOneShot(alertAudio);
                    }
                    break;
            }
        }
        //stop doing what the enemy was doing. Stop movement too depending on input. 
        public void ClearAction(bool stopMovement)
        {
            //stop the coroutine if its doing something
            if (actionRoutine != null)
            {
                StopCoroutine(actionRoutine);
            }
            //set movement target to current position (aka stop) depending on input. 
            if (stopMovement)
            {
                agent.SetDestination(transform.position);
            }
        }
        
        //overridable attack function
        public virtual void Attack ()
        {
            
        }
        //set destination to static point. 
        public void MoveToStatic (Vector3 point)
        {
            agent.SetDestination(point);
        }
        
        //Start a coroutine directing the enemy toward a transform.
        public void MoveToDynamic (Transform target)
        {
            actionRoutine = StartCoroutine(DynamicMove(target));
        }

        //Do that ^
        IEnumerator DynamicMove (Transform target)
        {
            //can use while true, because ClearAction will exit it for us. 
            while (true)
            {
                //Dont set this every frame, because then the enemy freezes. Also, this makes it have reaction time. 
                agent.SetDestination(target.position);
                yield return new WaitForSeconds(0.1f);
            }
            yield break;
        }

        //Go to the last saved position of the player. Will probs replace this with MoveToStatic later. 
        public void GoToLastSeenPlayer ()
        {
            agent.SetDestination(sense.lastPlayerPos);
        }

        //Set the glowy cube to a color and glowyness. 
        public void SetSignal (Color c, float emissive)
        {
            if (hasSignal)
            {
                //enable keywords so emission works, then set the color and the shader input. 
                signalRenderer.material.EnableKeyword("_EMISSION");
                signalRenderer.material.color = c;
                signalRenderer.material.SetColor("_EmissionColor", c * emissive);
            }
        }
        //Die function. Never actually called as the player cant attack at this point. 
        //Its a game of dodging at the moment :P
        public virtual void Die ()
        {
            Destroy(this.gameObject, 0.2f);
        }
        //Start a coroutine to constantly face the player
        public void FacePlayer ()
        {
            actionRoutine = StartCoroutine(TurnToPlayer());
        }
        //^
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

