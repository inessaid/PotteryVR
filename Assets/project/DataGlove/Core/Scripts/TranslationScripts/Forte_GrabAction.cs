using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Forte_GrabAction : MonoBehaviour
{
    [HideInInspector]
    public bool isGrabbing = false;
    [HideInInspector]
    public GameObject hand;
    [HideInInspector]
    public int fingerCollisions = 0;

    public virtual void EnterGrab(GameObject grabHand)
    {
        hand = grabHand;
        isGrabbing = true;
        GrabEnter();
    }

    public virtual void ExitGrab(GameObject grabHand)
    {
        hand = null;
        isGrabbing = false;
        GrabExit();
    }

    public virtual void GrabEnter()
    {

    }

    public virtual void GrabExit()
    {

    }

}
