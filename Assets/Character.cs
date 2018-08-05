using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weekly
{
    public class Character : MonoBehaviour
    {

        public State idleState, movingState;
        public State activeState;
        float idleTimer;
        Vector3 targetPos;

        public float speed, rot;
        private void Start()
        {
            CharacterStart();

        }

        public virtual void CharacterStart()
        {
            idleState = new State();
            idleState.onExit.Play += IdleExit;
            idleState.onEnter.Play += IdleEnter;
            movingState = new State();
            movingState.onEnter.Play += MovingEnter;
            movingState.onExit.Play += MovingExit;

            activeState = idleState;
            StartCoroutine(LocalFSMUpdate());
        }

        IEnumerator LocalFSMUpdate()
        {
            idleTimer = Random.value * 5;
            while (true)
            {
                if (activeState == idleState)
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer > 5)
                    {
                        print("Change to moving");
                        yield return ChangeState(movingState);
                    }
                }
                else if (activeState == movingState)
                {
                    print("Change to idle");
                    if (Vector3.Distance(transform.position, targetPos) < 0.25f)
                    {
                        yield return new WaitForSeconds(0.1f);
                        yield return ChangeState(idleState);

                    }
                    else
                    {
                        transform.position += transform.forward * speed * Time.deltaTime;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((targetPos - transform.position).normalized, Vector3.up), rot * Time.deltaTime);
                    }


                }

                yield return null;
            }
            yield break;
        }

        IEnumerator ChangeState(State target)
        {
            activeState.onExit.Play();
            yield return null;
            activeState = target;
            activeState.onEnter.Play();
            yield break;
        }

        void IdleEnter()
        {
            print("Idle Enter");
            idleTimer = 0;
            GetComponent<Renderer>().material.color = Color.blue;
        }

        void IdleExit()
        {
            print("Idle Exit");
        }

        void MovingEnter()
        {
            print("Moving Enter");
            GetComponent<Renderer>().material.color = Color.red;
            targetPos = GameManager.manager.tileManager.GetAvailableTile().transform.position + Vector3.up;
            //transform.Rotate(Vector3.up, Random.value * 360);
        }

        void MovingExit()
        {
            print("Moving Exit");
        }
    }

    public class State
    {
        public Transition onEnter, onExit;

        public State()
        {
            onEnter = new Transition();
            onExit = new Transition();
        }
    }

    public class Transition
    {
        public delegate void OnTransition();
        public OnTransition Play;
    }

}
