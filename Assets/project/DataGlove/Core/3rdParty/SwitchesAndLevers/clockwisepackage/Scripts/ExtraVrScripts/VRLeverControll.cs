using UnityEngine;
using System.Collections;

public class VRLeverControll : MonoBehaviour
{
    public bool isVR;

    public Animator anim;

    public GameObject[] lamps;

    bool up = true;

    public string upTrigger;
    public string downTrigger;



    void Start()
    {
        anim.SetTrigger(upTrigger);
        foreach (GameObject lamp in lamps)
        {
            lamp.GetComponent<Renderer>().material = lamp.GetComponent<LightScript>().offMat;
            lamp.GetComponent<LightScript>().isOn = false;
        }
    }

    public void turn()
    {
        resetAnimator();
        if (up)
        {
            anim.SetTrigger(downTrigger);
            up = !up;
        } else
        {
            anim.SetTrigger(upTrigger);
            up = !up;
        }
        foreach (GameObject lamp in lamps)
        {
            if (lamp.GetComponent<LightScript>().isOn)
            {
                lamp.GetComponent<Renderer>().material = lamp.GetComponent<LightScript>().offMat;
                lamp.GetComponent<LightScript>().isOn = false;
            } else
            {
                lamp.GetComponent<Renderer>().material = lamp.GetComponent<LightScript>().onMat;
                lamp.GetComponent<LightScript>().isOn = true;
            }
        }
    }

    void resetAnimator()
    {
        anim.ResetTrigger(upTrigger);
        anim.ResetTrigger(downTrigger);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isVR && other.tag == "VRPoint")
            turn();
    }
}
