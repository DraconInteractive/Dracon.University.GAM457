using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Blocker_Enemy : Enemy
    {
        public Squad squad;

        public override void EvaluateDetection()
        {
            if (sense.PlayerDetect())
            {
                squad.PlayerDetectionLevel += Time.deltaTime * detectionGain;
            }
        }

        public override void Attack()
        {
            base.Attack();
            actionRoutine = StartCoroutine(DoAttack());
        }
        IEnumerator DoAttack()
        {
            while (true)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Player.player.transform.position - transform.position, Vector3.up), rotationSpeed * Time.deltaTime);
                yield return null;
            }
            yield break;
        }

        public void GoToBlocking()
        {
            base.Attack();
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
    }
}

