using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace Final
{
    //Base system for the evaluation of an AI's behaviour tree. 
    public class AI_Behaviour : MonoBehaviour
    {
        //The starting node / parent of all control nodes. 
        public RootNode rootNode;
        //Reference to AI whom the tree belongs to.
        //Note that the variable is an "Enemy". Were I to have time to make this suitable for both companion and enemy AI, I would have derivative classes, of both the behaviour tree, and the AI class. 
        Enemy enemy;
        //Debug string so I can see the AI's current execution result. Was more necessary when they werent giving visual feedback. 
        public string current;
        //If i want to see a full breakdown of decisionmaking for a frame, i enable this. Warning, it spews out information. 
        public bool printEvents;
        // Use this for initialization
        void Start()
        {
            //Get the AI and store it. 
            enemy = GetComponent<Enemy>();
        }
        /// <summary>
        /// Used to find the execution node / action that the AI should perform this frame. 
        /// </summary>
        public void EvaluateTree()
        {
            //Had some weird error where the AI wasnt being retrieved in start, so i put this check in and it fixed it.
            if (enemy == null)
            {
                enemy = GetComponent<Enemy>();
            }
            //Using an algorithm based off A*, it will iterate through the nodes in the tree, finding the most suitable node to go through next, until it finds the appropriate execution node. 
            //It wasnt until i did this that I understood why people call navigation systems "node trees". Revelation AF. 
            BehaviourNode currentNode = rootNode.childNode;
            //While we are still evaluating conditions...
            while (currentNode is ControlNode)
            {
                //convert and store the provided node.
                ControlNode c = currentNode as ControlNode;
                //use system.reflection to get the string from the node, and call the appropriate function.
                MethodInfo method = this.GetType().GetMethod(c.methodName);
                //store the result.
                bool result = (bool)method.Invoke(this, null);
                //set the current node to the appropriate resultant. 
                if (result)
                {
                    currentNode = c.positive;
                }
                else
                {
                    currentNode = c.negative;
                }
                //If we are reporting all steps of the ai process, print what just happened. 
                if (printEvents)
                {
                    print(c.name);
                }
            }
            //If the resultant is an execution node, convert and store it. 
            if (currentNode is ExecutionNode)
            {
                ExecutionNode e = currentNode as ExecutionNode;
                //As we have no return, there is no need for reflection, if we arent already running this event, invoke it instantly. 
                if (e.methodName != current)
                {
                    Invoke(e.methodName, 0);
                    //set the public string to the method so i can see what the AI is supposed to be doing. 
                    current = e.methodName;
                    //See above
                    if (printEvents)
                    {
                        print(e.name);
                    }
                }
            }
        }
        //The public bool's here are condition node functions. They source, evaluate and return the results of the requested condition, mostly from the AI. 
        //These three return whether we are at a specific alert level. 
        public bool AtDefault ()
        {
            return (enemy.alertLevel == Enemy.AlertLevel.Default);
        }

        public bool AtSuspicious ()
        {
            return (enemy.alertLevel == Enemy.AlertLevel.Suspicious);
        }

        public bool AtAlert ()
        {
            return (enemy.alertLevel == Enemy.AlertLevel.Alert);
        }
        //Similar to the function in "Squad", this evaluates whether the playerDetectionLevel of the AI lies within the appropriate range of values for its current alert level. 
        public bool AlertLevelAppropriate ()
        {
            bool b = false;
            switch (enemy.alertLevel)
            {
                case Enemy.AlertLevel.Default:
                    if (enemy.playerDetectionLevel < 1)
                    {
                        b = true;
                    }
                    break;
                case Enemy.AlertLevel.Suspicious:
                    if (enemy.playerDetectionLevel < 2 && enemy.playerDetectionLevel >= 1)
                    {
                        b = true;
                    }
                    break;
                case Enemy.AlertLevel.Alert:
                    if (enemy.playerDetectionLevel >= 2)
                    {
                        b = true;
                    }
                    break;
            }

            return b;
        }
        //Is the AI within a certain distance of the player?
        public bool WithinAttackDistance ()
        {
            float dist = Vector3.Distance(Player.player.transform.position, transform.position);
            return (dist < enemy.attackDistance);
        }
        //AI has navmesh, so can use this to see if there is a valid path to the player. 
        public bool CanPathToPlayer ()
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            
            if (agent != null)
            {
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(Player.player.transform.position, path);
                return (path.status == NavMeshPathStatus.PathComplete);
            }
            return false;
        }
        //is the melee squad member alive?
        public bool MeleeAlive ()
        {
            if (enemy is Blocker_Enemy)
            {
                return ((enemy as Blocker_Enemy).squad.melee != null);
            }
            if (enemy is Melee_Enemy)
            {
                return true;
            }
            if (enemy is Ranged_Enemy)
            {
                return ((enemy as Ranged_Enemy).squad.melee != null);
            }

            return false;
        }
        //is the AI at a close enough distance to the positon set by the squad controller as "blocking position"
        //This only works for enemies with a squad, so it just returns false for any other. 
        public bool AtBlockingPosition ()
        {
            Vector3 bp = Vector3.zero;
            if (enemy is SquadEnemy)
            {
                bp = (enemy as SquadEnemy).squad.blockerPos;
                return (Vector3.Distance(enemy.transform.position, bp) < 0.5f);
            }

            return false;
        }
        //Same goes as blocking position. 
        public bool AtRangedPosition ()
        {
            Vector3 rp = Vector3.zero;

            if (enemy is SquadEnemy)
            {
                rp = (enemy as SquadEnemy).squad.rangedPos;
                return (Vector3.Distance(enemy.transform.position, rp) < 0.5f);
            }

            return false;
        }

        //From here we have the execution node functions. Most of them will either call a coroutine, or a function on the enemy. 
        //Nigh on all will call enemy.ClearAction(bool), which clears the actions the enemy *was* doing to make way for their new directive. 

        //Only works for blocker enemies, need to expand this to ranged. 
        //Disregard above, i moved the functions to SquadEnemy. Booya. 
        public void MoveToBlockingPosition ()
        {
            enemy.ClearAction(true);
            if (enemy is SquadEnemy)
            {
                (enemy as SquadEnemy).GoToBlocking();
            }
        }

        //I moved GoToBlocking to the SquadEnemy class, so i did the same with GoToRanged, just for future proofing. 
        public void MoveToRangedPosition ()
        {
            enemy.ClearAction(true);
            if (enemy is SquadEnemy)
            {
                (enemy as SquadEnemy).GoToFlanking();
            }
        }
        //Find the appropriate alert level to be changed to, then tell the enemy. Enemy will run transition. 
        public void ChangeAlertLevel ()
        {
            enemy.ClearAction(true);
            if (enemy.playerDetectionLevel < 1)
            {
                enemy.SetAlertLevel(Enemy.AlertLevel.Default);
            }
            else if (enemy.playerDetectionLevel >= 1 && enemy.playerDetectionLevel < 2)
            {
                enemy.SetAlertLevel(Enemy.AlertLevel.Suspicious);
            }
            else if (enemy.playerDetectionLevel >= 2)
            {
                enemy.SetAlertLevel(Enemy.AlertLevel.Alert);
            }
        }
        //This is used when the ai gets handled something it should. In this case, it should only ever be called when the ai is not at default, suspicious OR combat alert level. 
        //Or in other words, never. 
        public void HandleError ()
        { 
            print("Exceeded alert state parameters: if this happens, I lose all confidence in my knowledge of this programming");
            enemy.ClearAction(true);
        }
        //I feel this explains itself.
        public void Wait ()
        {
            enemy.ClearAction(true);
        }
        //Tells the enemy to remain stationary, but to twist to face the player. 
        public void FacePlayer ()
        {
            enemy.ClearAction(true);
            enemy.FacePlayer();
        }
        //Attack can be different per enemy, but the base Enemy class attack can be overriden, so just call that. 
        public void Attack ()
        {
            enemy.ClearAction(true);
            enemy.Attack();
        }
        //Does what it says. Normal Enemy GOTO would follow the player, MoveToStatic goes to a fixed position. 
        public void GoToPlayerLastSeen ()
        {
            enemy.ClearAction(false);
            enemy.MoveToStatic(enemy.sense.lastPlayerPos);
        }
        //go to the player. Interesting point (to me), is that I dont need to put any type of restriction on this, such as to stop 2 meters away. The tree does that for me. Yay :)
        public void PathToPlayer ()
        {
            enemy.ClearAction(true);
            enemy.MoveToDynamic(Player.player.transform);
        }


    }
}

