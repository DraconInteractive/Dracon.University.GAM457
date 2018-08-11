using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script derived (but not copied) from https://gist.github.com/ftvs/5822103
//shakes the camera on call. 
public class CameraShake : MonoBehaviour {
    //static reference so anything can call shake
    public static CameraShake shake;
    //Save camera initial offset so that we can return to standard position. 
    Vector3 init = Vector3.zero;
    //Store routine so that we only have one shake IE working at once. 
    Coroutine shakeRoutine;
    //How far does the camera shake
    public float amount;
    
    private void Awake()
    {
        //set the singleton
        shake = this;
        //save the starting position.
        init = transform.localPosition;
    }
    //function to start the shake, with an input of duration
    public void StartShake (float duration)
    {
        //if already shaking, stahp.
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }
        //start the coroutine. 
        shakeRoutine = StartCoroutine(Shake(duration));
    }
    
    IEnumerator Shake (float duration)
    {
        //change position by a random direction * inspector set amount
        for (float f = 0; f < duration; f += Time.deltaTime)
        {
            transform.localPosition = init + Random.insideUnitSphere * amount;
            yield return null;
        }
        //when we are done, return to init pos
        transform.localPosition = init;
        yield break;
    }
}
