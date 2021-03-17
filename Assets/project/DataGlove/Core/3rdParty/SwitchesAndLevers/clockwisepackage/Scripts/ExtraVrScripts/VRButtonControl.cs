using UnityEngine;
using System.Collections;

public class VRButtonControl : MonoBehaviour {

    public bool isVR;

    public Animator anim;

    public GameObject[] lamps;

    public string downTrigger;

    void Start()
    {
        for (int i = 0; i < lamps.Length; i++)
        {
            lamps[i].GetComponent<Renderer>().material = lamps[i].GetComponent<LightScript>().offMat;
            lamps[i].GetComponent<LightScript>().isOn = false;
        }
    }

    public void turn()
    {
        resetAnimator();
        anim.SetTrigger(downTrigger);
        foreach (GameObject lamp in lamps)
        {
            if (lamp.GetComponent<LightScript>().isOn)
            {
                lamp.GetComponent<Renderer>().material = lamp.GetComponent<LightScript>().offMat;
                lamp.GetComponent<LightScript>().isOn = false;
            }
            else
            {
                lamp.GetComponent<Renderer>().material = lamp.GetComponent<LightScript>().onMat;
                lamp.GetComponent<LightScript>().isOn = true;
            }
        }
    }

    void resetAnimator()
    {
        anim.ResetTrigger(downTrigger);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isVR && other.tag == "VRPoint")
            turn();
    }
}
