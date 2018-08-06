using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    [CreateAssetMenu(fileName = "Node", menuName = "Control Node")]
    public class ControlNode : BehaviourNode
    {
        public BehaviourNode positive, negative;
        public string methodName;
    }
}

