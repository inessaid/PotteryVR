using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Material onMat;
    public Material offMat;
    public float blinkDuration;
    private bool isBlinking = false;
    private bool isOn = false;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = offMat;    
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBlinking)
        {
            return;
        }

        time += Time.deltaTime;

        if (time > blinkDuration)
        {
            time = 0;
            isOn = !isOn;

            if (isOn)
            {
                GetComponent<Renderer>().material = onMat;
            }
            else
            {
                GetComponent<Renderer>().material = offMat;
            }
        }
    }

    public void SetBlinking(bool blinkState)
    {
        isBlinking = blinkState;
        isOn = blinkState;

        if (isBlinking)
        {
            GetComponent<Renderer>().material = onMat;
        }
        else
        {
            GetComponent<Renderer>().material = offMat;
        }
    }
}
