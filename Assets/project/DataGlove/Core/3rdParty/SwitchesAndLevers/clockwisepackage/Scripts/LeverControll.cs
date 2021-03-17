using UnityEngine;
using System.Collections;

public class LeverControll : MonoBehaviour {

    public Animator anim;

    public GameObject[] lamps;

    bool up = true;

    public string upTrigger;
    public string downTrigger;

    void Start()
    {
        anim.SetTrigger(upTrigger);
        for (int i = 0; i < lamps.Length; i++)
        {
            lamps[i].GetComponent<Renderer>().material = lamps[i].GetComponent<LightScript>().offMat;
            lamps[i].GetComponent<LightScript>().isOn = false;
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
        for (int i = 0; i < lamps.Length; i++)
        {
            if (lamps[i].GetComponent<LightScript>().isOn)
            {
                lamps[i].GetComponent<Renderer>().material = lamps[i].GetComponent<LightScript>().offMat;
                lamps[i].GetComponent<LightScript>().isOn = false;
            } else
            {
                lamps[i].GetComponent<Renderer>().material = lamps[i].GetComponent<LightScript>().onMat;
                lamps[i].GetComponent<LightScript>().isOn = true;
            }
        }
    }

    void resetAnimator()
    {
        anim.ResetTrigger(upTrigger);
        anim.ResetTrigger(downTrigger);
    }
}
