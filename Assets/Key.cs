using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Key : POI
    {
        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, 65 * Time.deltaTime);
        }
    }
}

