using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    public class Melee_Enemy : Enemy
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
        IEnumerator DoAttack ()
        {
            while (true)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Player.player.transform.position - transform.position, Vector3.up), rotationSpeed * Time.deltaTime);
                yield return null;
            }
            yield break;
        }

        public override void Die()
        {
            base.Die();
            squad.melee = null;
        }
    }
}

