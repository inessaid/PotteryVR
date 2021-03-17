using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = gameObject.GetComponent<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        //float glowAmount = Triangle(1, 6, 4, 0, Time.deltaTime);
        //material.SetFloat("_RimPower", glowAmount);
        //Debug.Log("Glow Amount = " + glowAmount);
    }

    float Triangle(float minLevel, float maxLevel, float period, float phase, float t)
    {
        float pos = Mathf.Repeat(t - phase, period) / period;

        if (pos < .5f)
        {
            return Mathf.Lerp(minLevel, maxLevel, pos * 2f);
        }
        else
        {
            return Mathf.Lerp(maxLevel, minLevel, (pos - .5f) * 2f);
        }
    }
}
