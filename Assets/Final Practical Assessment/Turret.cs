using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    [RequireComponent(typeof(Sense))]
    public class Turret : Enemy
    {
        
        
        public Renderer signalRenderer;
        public Transform gunPivot;

        

        public float attackDistance;
        

        

        public AudioSource turretActiveAudio;
        public AudioClip defaultAudio, suspiciousAudio, alertAudio, fireAudio;

        public float coolDown, damage;


        public override void SetAlertLevel (AlertLevel newLevel)
        {
            signalRenderer.material.EnableKeyword("_EMISSION");
            switch (newLevel)
            {
                case AlertLevel.Default:
                    turretActiveAudio.PlayOneShot(defaultAudio);
                    signalRenderer.material.color = Color.blue;
                    signalRenderer.material.SetColor("_EmissionColor", Color.blue * 3);
                    break;
                case AlertLevel.Suspicious:
                    turretActiveAudio.PlayOneShot(suspiciousAudio);
                    signalRenderer.material.color = Color.yellow;
                    signalRenderer.material.SetColor("_EmissionColor", Color.yellow * 3);
                    break;
                case AlertLevel.Alert:
                    turretActiveAudio.PlayOneShot(alertAudio);
                    signalRenderer.material.color = Color.red;
                    signalRenderer.material.SetColor("_EmissionColor", Color.red * 3);
                    break;
            }

            alertLevel = newLevel;
            
        }

        public override void Attack()
        {
            ClearAction();
            actionRoutine = StartCoroutine(DoAttack());
        }

        public IEnumerator DoAttack ()
        {
            while (true)
            {
                RaycastHit hit;
                if (Physics.Linecast (transform.position, player.transform.position, out hit))
                {
                    if (hit.transform.gameObject == player.transform.gameObject)
                    {
                        player.Damage(damage);
                        turretActiveAudio.PlayOneShot(fireAudio);
                    }
                }

                yield return new WaitForSeconds(coolDown);
            }
            yield break;
        }
    }
}

