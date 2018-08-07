using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace Final
{
    public class AI_Behaviour : MonoBehaviour
    {
        //Behaviour tree integration for AI
        public RootNode rootNode;

        Enemy enemy;

        public string current;
        // Use this for initialization
        void Start()
        {
            enemy = GetComponent<Enemy>();
        }

        public void EvaluateTree()
        {
            //print("tree evaluated");

            if (enemy == null)
            {
                enemy = GetComponent<Enemy>();
            }

            BehaviourNode currentNode = rootNode.childNode;
            while (currentNode is ControlNode)
            {
                ControlNode c = currentNode as ControlNode;

                MethodInfo method = this.GetType().GetMethod(c.methodName);
                bool result = (bool)method.Invoke(this, null);

                if (result)
                {
                    currentNode = c.positive;
                }
                else
                {
                    currentNode = c.negative;
                }
            }
            if (currentNode is ExecutionNode)
            {
                ExecutionNode e = currentNode as ExecutionNode;

                if (e.methodName != current)
                {
                    Invoke(e.methodName, 0);
                    current = e.methodName;
                }
            }
        }

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

        public bool WithinAttackDistance ()
        {
            float dist = Vector3.Distance(Player.player.transform.position, transform.position);
            return (dist < enemy.attackDistance);
        }

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

        public void HandleError ()
        {
            //Referencing Star Wars: The Revenge of the Sith, because one hearing is quite enough. 
            print("How did this happen? We're smarter than this!");
            enemy.ClearAction(true);
        }

        public void Wait ()
        {
            enemy.ClearAction(true);
        }

        public void Attack ()
        {
            //turret.ClearAction();
            //turret.StartCoroutine(turret.DoAttack());
            enemy.ClearAction(true);
            enemy.Attack();
        }

        public void GoToPlayerLastSeen ()
        {
            enemy.ClearAction(false);
            enemy.MoveToStatic(enemy.sense.lastPlayerPos);
        }

        public void PathToPlayer ()
        {
            enemy.ClearAction(true);
            enemy.MoveToDynamic(Player.player.transform);
        }
    }
}

