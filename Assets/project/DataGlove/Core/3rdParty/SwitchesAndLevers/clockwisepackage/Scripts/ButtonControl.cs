using UnityEngine;
using System.Collections;

public class ButtonControl : MonoBehaviour {

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
        for (int i = 0; i < lamps.Length; i++)
        {
            if (lamps[i].GetComponent<LightScript>().isOn)
            {
                lamps[i].GetComponent<Renderer>().material = lamps[i].GetComponent<LightScript>().offMat;
                lamps[i].GetComponent<LightScript>().isOn = false;
            }
            else
            {
                lamps[i].GetComponent<Renderer>().material = lamps[i].GetComponent<LightScript>().onMat;
                lamps[i].GetComponent<LightScript>().isOn = true;
            }
        }
    }

    void resetAnimator()
    {
        anim.ResetTrigger(downTrigger);
    }
}
