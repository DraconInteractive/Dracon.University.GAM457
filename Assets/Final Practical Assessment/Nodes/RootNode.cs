using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    [CreateAssetMenu(fileName = "Node", menuName = "Root Node")]
    public class RootNode : BehaviourNode
    {
        public BehaviourNode childNode;
    }
}

