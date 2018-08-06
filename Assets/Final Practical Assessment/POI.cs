using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class POI : MonoBehaviour
    {

        public static List<POI> all = new List<POI>();

        private void Awake()
        {
            all.Add(this);
        }
    }
}

