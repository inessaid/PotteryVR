using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatObject : MonoBehaviour
{
    float amplitudeY = 0.07f;
    float omegaY = 2.5f;
    float index;

    public void Update()
    {
        index += Time.deltaTime;
        float y = (amplitudeY * Mathf.Sin((omegaY * index) + 1));
        transform.localPosition = new Vector3(0, 0, y);
    }
}
