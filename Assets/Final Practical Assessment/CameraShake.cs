using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script derived (but not copied) from https://gist.github.com/ftvs/5822103

public class CameraShake : MonoBehaviour {
    public static CameraShake shake;
    Vector3 init = Vector3.zero;
    Coroutine shakeRoutine;
    public float amount;
    private void Awake()
    {
        shake = this;
        init = transform.localPosition;
    }
    public void StartShake (float duration)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(Shake(duration));
    }

    IEnumerator Shake (float duration)
    {
        
        for (float f = 0; f < duration; f += Time.deltaTime)
        {
            transform.localPosition = init + Random.insideUnitSphere * amount;
            yield return null;
        }
        transform.localPosition = init;
        yield break;
    }
}
