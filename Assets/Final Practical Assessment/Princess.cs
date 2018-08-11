using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    //Princess pickup class. Accessible after getting key, and makes Exit pickup accessible. 
    public class Princess : POI
    {
        //How fast the princess shoots of into space
        public float speed;
        //store the trail for use later. 
        public TrailRenderer trail;

        //Called when the player picks up the princess, the princess runs the base stuff (whether it should be destroyed), and then starts the process to zoom into space. 
        public override void OnHitPlayer()
        {
            base.OnHitPlayer();
            StartCoroutine(Zoomies());
            trail.enabled = true;
        }
        //Move the princess upward, in a zoom like fashion. 
        IEnumerator Zoomies ()
        {
            //Move for 10 seconds, upward, at a speed determined by a local variable. Then, destroy the princess and remove it from the static container (so that squad doesnt try to evaluate it).
            for (float f = 0; f < 10; f += Time.deltaTime)
            {
                Vector3 target = transform.position + Vector3.up;
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }
            Destroy(this.gameObject, 0.2f);
            all.Remove(this);
            yield break;
        }
    }
}

