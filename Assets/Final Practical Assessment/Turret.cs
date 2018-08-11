using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Final
{
    //Turret enemy type
    [RequireComponent(typeof(Sense))]
    public class Turret : Enemy
    {
        //get the pivot point of the gun
        public Transform gunPivot;

        //weapon variables
        public float coolDown, damage;
        //get initial position and rotation for use in shaking
        Vector3 initShakePos;
        Quaternion initShakeRot;
        //variables for duration and amount of shaking
        public float shakeDuration, shakeAmount;
        //linerenderer for the attack laser
        public LineRenderer laser;
        //duration and amount of laser pulse
        public float lPulseDuration, lPulseAmount;
        //initial laser width for pulse
        float initLaserWidth;

        //on start, get initial shake pos, and rotation as well as initial laser width
        public override void EnemyStart()
        {
            base.EnemyStart();
            initShakePos = transform.localPosition;
            initShakeRot = transform.localRotation;

            initLaserWidth = laser.startWidth;
        }
        //start the attack coroutine
        public override void Attack()
        {
            actionRoutine = StartCoroutine(DoAttack());
        }
        
        public IEnumerator DoAttack ()
        {
            while (true)
            {
                //do a line cast from enemy to the player.
                RaycastHit hit;
                if (Physics.Linecast (transform.position, player.transform.position, out hit))
                {
                    //if it hits the player, damage the player and call the fire function. 
                    if (hit.transform.gameObject == player.transform.gameObject)
                    {
                        player.Damage(damage);
                        Fire();
                    }
                    else if (hit.transform.tag == "Glass")
                    {
                        //if the linecast hits glass, just do the showy stuff, but dont apply the damage. 
                        Fire();
                    }
                }

                yield return new WaitForSeconds(coolDown);
            }
            yield break;
        }

        //the showy part of firing
        void Fire ()
        {
            //play the audio, shake the turret and pulse the laser
            activeAudio.PlayOneShot(fireAudio);
            StartShake();
            StartPulse();
            print("firing");
        }

        //start the shake routine
        public void StartShake ()
        {
            StartCoroutine(Shake());
        }
        //^
        IEnumerator Shake ()
        {
            //for the duration of the shake, move the enemy in random directions multiplied by amounts
            for (float f = 0; f < shakeDuration; f += Time.deltaTime)
            {
                transform.localPosition = initShakePos + Random.insideUnitSphere * shakeAmount;
                yield return null;
            }
            //once we are done, reset the position and rotation of the turret. 
            transform.localPosition = initShakePos;
            transform.localRotation = initShakeRot;
            yield break;
        }

        //start the pulse routine
        public void StartPulse()
        {
            StartCoroutine(LaserPulse());
        }
        //^
        IEnumerator LaserPulse ()
        {
            // for half the pulse duration, lerp the laser width upward. 
            for (float f = 0; f < 1; f += Time.deltaTime / (lPulseDuration * 0.5f))
            {
                laser.startWidth = Mathf.Lerp(initLaserWidth, initLaserWidth + lPulseAmount, f);
                laser.endWidth = Mathf.Lerp(initLaserWidth, initLaserWidth + lPulseAmount, f);
                yield return null;
            }
            // for the other half, lerp the laser width back down. 
            for (float f = 1; f > 0; f -= Time.deltaTime / (lPulseDuration * 0.5f))
            {
                laser.startWidth = Mathf.Lerp(initLaserWidth, initLaserWidth + lPulseAmount, f);
                laser.endWidth = Mathf.Lerp(initLaserWidth, initLaserWidth + lPulseAmount, f);
                yield return null;
            }
            //wait a frame then set laser width back to original. 
            yield return null;
            laser.startWidth = initLaserWidth;
            laser.endWidth = initLaserWidth;
            yield break;
        }
    }
}

