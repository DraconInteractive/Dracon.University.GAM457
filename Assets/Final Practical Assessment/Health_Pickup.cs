using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Health_Pickup : POI
    {
        void Update()
        {
            transform.Rotate(Vector3.up, 65 * Time.deltaTime);
        }
    }
}

