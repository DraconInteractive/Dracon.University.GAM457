using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    //derivative of the enemy class, with variables and functions for enemies in a squad. 
    public class SquadEnemy : Enemy
    {
        //reference to the squad. 
        [Space]
        [Header("Squad")]
        public Squad squad;

        //get detection, and apply it to the detection level meter
        public override void EvaluateDetection()
        {
            if (sense.PlayerDetect())
            {
                squad.PlayerDetectionLevel += Time.deltaTime * detectionGain;
            }
        }
        
        //run base attack, then start the custom IEnumerator 
        public override void Attack()
        {
            base.Attack();
            actionRoutine = StartCoroutine(DoAttack());
        }

        IEnumerator DoAttack()
        {
            //make a variable for a timer
            float fireTimer = 0;
            while (true)
            {
                //turn toward the player
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Player.player.transform.position - transform.position, Vector3.up), rotationSpeed * Time.deltaTime);
                //increment the fire timer
                fireTimer += Time.deltaTime;
                //if the firetimer is greater than 1, then call the fire function.
                if (fireTimer > 1)
                {
                    fireTimer = 0;
                    Fire();
                }
                yield return null;
            }
            yield break;
        }
        //fire function
        public void Fire ()
        {
            //play appropriate sound
            activeAudio.PlayOneShot(fireAudio);
        }
        //start a coroutine to go to the blocking position
        public void GoToBlocking()
        {
            actionRoutine = StartCoroutine(GTBlocking());
        }
        //^
        IEnumerator GTBlocking()
        {
            while (true)
            {
                //get the target position from the squad controller
                Vector3 target = squad.blockerPos;
                //tell the enemy to move
                agent.SetDestination(target);
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }
        //start a coroutine to go to flanking position
        public void GoToFlanking()
        {
            actionRoutine = StartCoroutine(GTFlanking());
        }
        //^
        IEnumerator GTFlanking()
        {
            while (true)
            {
                //get ranged pos from squad controller,then move there. 
                Vector3 target = squad.rangedPos;
                agent.SetDestination(target);
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }
    }
}

