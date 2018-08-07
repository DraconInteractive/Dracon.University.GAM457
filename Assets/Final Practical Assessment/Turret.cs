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

        public AudioSource turretActiveAudio;
        public AudioClip defaultAudio, suspiciousAudio, alertAudio, fireAudio;

        public float coolDown, damage;

        Vector3 initShakePos;
        Quaternion initShakeRot;
        public float shakeDuration, shakeAmount;

        public LineRenderer laser;
        public float lPulseDuration, lPulseAmount;
        float initLaserWidth;

        public override void EnemyStart()
        {
            base.EnemyStart();
            initShakePos = transform.localPosition;
            initShakeRot = transform.localRotation;

            initLaserWidth = laser.startWidth;
        }
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
            //setting movementstop to false for now. We will see
            ClearAction(false);
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
                        Fire();
                    }
                    else if (hit.transform.tag == "Glass")
                    {
                        Fire();
                    }
                }

                yield return new WaitForSeconds(coolDown);
            }
            yield break;
        }

        void Fire ()
        {
            turretActiveAudio.PlayOneShot(fireAudio);
            StartShake();
            StartPulse();
            print("firing");
        }
        public void StartShake ()
        {
            StartCoroutine(Shake());
        }

        IEnumerator Shake ()
        {
            for (float f = 0; f < shakeDuration; f += Time.deltaTime)
            {
                transform.localPosition = initShakePos + Random.insideUnitSphere * shakeAmount;
                yield return null;
            }
            transform.localPosition = initShakePos;
            transform.localRotation = initShakeRot;
            yield break;
        }

        public void StartPulse()
        {
            StartCoroutine(LaserPulse());
        }

        IEnumerator LaserPulse ()
        {
            
            for (float f = 0; f < 1; f += Time.deltaTime / (lPulseDuration * 0.5f))
            {
                laser.startWidth = Mathf.Lerp(initLaserWidth, initLaserWidth + lPulseAmount, f);
                laser.endWidth = Mathf.Lerp(initLaserWidth, initLaserWidth + lPulseAmount, f);
                yield return null;
            }

            for (float f = 1; f > 0; f -= Time.deltaTime / (lPulseDuration * 0.5f))
            {
                laser.startWidth = Mathf.Lerp(initLaserWidth, initLaserWidth + lPulseAmount, f);
                laser.endWidth = Mathf.Lerp(initLaserWidth, initLaserWidth + lPulseAmount, f);
                yield return null;
            }
            yield return null;
            laser.startWidth = initLaserWidth;
            laser.endWidth = initLaserWidth;
            yield break;
        }
    }
}

