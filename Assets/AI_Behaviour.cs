using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Final
{
    public class AI_Behaviour : MonoBehaviour
    {
        //Behaviour tree integration for AI
        public RootNode rootNode;

        Turret turret;

        public string current;
        // Use this for initialization
        void Start()
        {
            turret = GetComponent<Turret>();
        }

        public void EvaluateTree()
        {
            //print("tree evaluated");

            if (turret == null)
            {
                turret = GetComponent<Turret>();
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
            return (turret.alertLevel == Turret.AlertLevel.Default);
        }

        public bool AtSuspicious ()
        {
            return (turret.alertLevel == Turret.AlertLevel.Suspicious);
        }

        public bool AtAlert ()
        {
            return (turret.alertLevel == Turret.AlertLevel.Alert);
        }

        public bool AlertLevelAppropriate ()
        {
            bool b = false;
            switch (turret.alertLevel)
            {
                case Turret.AlertLevel.Default:
                    if (turret.playerDetectionLevel < 1)
                    {
                        b = true;
                    }
                    break;
                case Turret.AlertLevel.Suspicious:
                    if (turret.playerDetectionLevel < 2 && turret.playerDetectionLevel >= 1)
                    {
                        b = true;
                    }
                    break;
                case Turret.AlertLevel.Alert:
                    if (turret.playerDetectionLevel >= 2)
                    {
                        b = true;
                    }
                    break;
            }

            return b;
        }

        public void ChangeAlertLevel ()
        {
            if (turret.playerDetectionLevel < 1)
            {
                turret.SetAlertLevel(Turret.AlertLevel.Default);
            }
            else if (turret.playerDetectionLevel >= 1 && turret.playerDetectionLevel < 2)
            {
                turret.SetAlertLevel(Turret.AlertLevel.Suspicious);
            }
            else if (turret.playerDetectionLevel >= 2)
            {
                turret.SetAlertLevel(Turret.AlertLevel.Alert);
            }
        }

        public void HandleError ()
        {
            //Referencing Star Wars: The Revenge of the Sith, because one hearing is quite enough. 
            print("How did this happen? We're smarter than this!");
        }
    }
}

