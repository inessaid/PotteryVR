using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreHaptics : MonoBehaviour
{
    HapticController[] hapticControllers;

    // Start is called before the first frame update
    void Start()
    {
        hapticControllers = FindObjectsOfType<HapticController>();
        StartCoroutine(LateStart(0.5f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        hapticControllers = FindObjectsOfType<HapticController>();

        for (int i = 0; i < hapticControllers.Length; i++)
        {
            hapticControllers[i].SetGrainFadeForAllActuators(0);
            hapticControllers[i].SetGrainLocationForAllActuators(0);
            hapticControllers[i].SetGrainSizeForAllActuators(1);
            hapticControllers[i].SelectHapticWaveForAllActuators(0);
        }
    }

    public void ThumbHaptic()
    {
        for (int i = 0; i < hapticControllers.Length; i++)
        {
            hapticControllers[i].ThumbLoop(0, 1);
        }
    }

    public void IndexHaptic()
    {
        for (int i = 0; i < hapticControllers.Length; i++)
        {
            hapticControllers[i].IndexLoop(0, 1);
        }
    }

    public void MiddleHaptic()
    {
        for (int i = 0; i < hapticControllers.Length; i++)
        {
            hapticControllers[i].MiddleLoop(0, 1);
        }
    }

    public void RingHaptic()
    {
        for (int i = 0; i < hapticControllers.Length; i++)
        {
            hapticControllers[i].RingLoop(0, 1);
        }
    }

    public void PinkyHaptic()
    {
        for (int i = 0; i < hapticControllers.Length; i++)
        {
            hapticControllers[i].PinkyLoop(0, 1);
        }
    }

    public void PalmHaptic()
    {
        for (int i = 0; i < hapticControllers.Length; i++)
        {
            hapticControllers[i].PalmLoop(0, 1);
        }
    }
}
