using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class SquadEnemy : Enemy
    {
        [Space]
        [Header("Squad")]
        public Squad squad;

        public override void EvaluateDetection()
        {
            if (sense.PlayerDetect())
            {
                squad.PlayerDetectionLevel += Time.deltaTime * detectionGain;
            }
        }

        public override void Die()
        {
            base.Die();
            squad.melee = null;
        }

        public override void Attack()
        {
            base.Attack();
            actionRoutine = StartCoroutine(DoAttack());
        }

        IEnumerator DoAttack()
        {
            float fireTimer = 0;
            while (true)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Player.player.transform.position - transform.position, Vector3.up), rotationSpeed * Time.deltaTime);

                fireTimer += Time.deltaTime;
                if (fireTimer > 1)
                {
                    fireTimer = 0;
                    Fire();
                }
                yield return null;
            }
            yield break;
        }

        public void Fire ()
        {
            activeAudio.PlayOneShot(fireAudio);
        }

        public void GoToBlocking()
        {
            actionRoutine = StartCoroutine(GTBlocking());
        }

        IEnumerator GTBlocking()
        {
            while (true)
            {
                Vector3 target = squad.blockerPos;
                agent.SetDestination(target);
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }

        public void GoToFlanking()
        {
            actionRoutine = StartCoroutine(GTFlanking());
        }

        IEnumerator GTFlanking()
        {
            while (true)
            {
                Vector3 target = squad.rangedPos;
                agent.SetDestination(target);
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }
    }
}

