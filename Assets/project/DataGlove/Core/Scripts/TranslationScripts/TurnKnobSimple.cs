using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnKnobSimple : Forte_Interactable
{
    public bool floatBack = false;
    public float floatTime = 1.0f;

    private GameObject hand;
    private Quaternion lastRot;
    private HapticController haptics;
    private Transform handParent = null;
    private float lerpAmp = 0;

    private Quaternion lastHandParentRot = Quaternion.identity;

    private void Update()
    {
        if (isGrabbed && handParent != null && lastHandParentRot != Quaternion.identity)
        {
            Quaternion diff = (handParent.transform.rotation *  Quaternion.Inverse(lastHandParentRot));
            Quaternion currentRot = gameObject.transform.rotation;
            float z = currentRot.eulerAngles.z + diff.eulerAngles.x;
            gameObject.transform.rotation = Quaternion.Euler(currentRot.eulerAngles.x, currentRot.eulerAngles.y, z);

            if (haptics != null)
            {
                float amp = RotationalAmp();

                haptics.SelectHapticWaveForAllActuators(12);
                haptics.SetGrainSizeForAllActuators(1);
                haptics.SetGrainLocationForAllActuators(0);
                haptics.SetGrainFadeForAllActuators(0);
                haptics.LoopAllActuators(0, amp);
            }

            /*Vector3 distance = hand.transform.position - transform.position;
            if (distance.magnitude > 0.2)
            {
                //ExitGrab(hand);
            }*/
        }

        lastRot = gameObject.transform.rotation;
        
        if(handParent != null)
        {
            lastHandParentRot = handParent.transform.rotation;
        }
    }

    public override void EnterGrab(GameObject handObj)
    {
        isGrabbed = true;
        hand = handObj;
        haptics = hand.GetComponentInParent<HapticController>();

        handParent = handObj.transform.parent;
        hand.GetComponent<Forte_Interactor>().FreezeHand(gameObject);
        grabHandRight = hand.GetComponent<GloveController>().rightHandGlove;
    }

    public override void ExitGrab(GameObject handObj)
    {
        if (handObj != null)
        {
            haptics = handObj.GetComponentInParent<HapticController>();

            if (haptics != null)
            {
                haptics.SilenceHaptics();
            }
        }

        handObj.GetComponent<Forte_Interactor>().UnfreezeHand();

        handParent = null;

        isGrabbed = false;
        hand = null;
        haptics = null;

        lastHandParentRot = Quaternion.identity;
    }

    private float RotationalAmp()
    {
        float triangleWave = Triangle(0.9f, 1, 40, 0, gameObject.transform.eulerAngles.z);

        Quaternion difference = gameObject.transform.rotation * Quaternion.Inverse(lastRot);
        Vector3 diffEuler = difference.eulerAngles;
        float rawZ = diffEuler.z;

        if (rawZ > 180)
        {
            rawZ -= 360;
        }

        rawZ = Mathf.Abs(rawZ);
        float triangleZ = rawZ * triangleWave;

        float newAmp = Mathf.Clamp(triangleZ, 0, 1);

        int lerpWeight = 1;
        lerpAmp = ((lerpAmp * lerpWeight) + newAmp) / (lerpWeight + 1);

        return lerpAmp;
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
