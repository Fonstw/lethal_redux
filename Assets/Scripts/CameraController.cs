using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dif = .1f;
    public float shakeTime = .25f;

    public IEnumerator Shake()
    {
        transform.position = new Vector3(Random.Range(-dif, dif), Random.Range(-dif, dif), 0);

        yield return new WaitForSeconds(shakeTime);
    }
}
