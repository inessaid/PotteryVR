using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Animator animator;
    public SwitchTrigger top;
    public SwitchTrigger bottom;
    private bool isUp = false;
    private float switchTime = 0;
    private float timeLimit = 0.2f;

    // Update is called once per frame
    void Update()
    {
        if (switchTime < timeLimit)
        {
            switchTime += Time.deltaTime;
        }
    }

    private void BuzzHaptic(GameObject finger)
    {
        HapticController haptics = finger.gameObject.GetComponentInParent<HapticController>();

        if (haptics == null)
        {
            return;
        }

        HapticSettings settings = new HapticSettings
        {
            waveform = 9,
            amplitude = 1,
            grainFade = 0,
            grainLocation = 0,
            grainSize = 0.1f,
            pitch = 0
        };

        haptics.PulseOnFingerSettings(finger, settings, 0.2f);

    }

    public void Flick(SwitchTrigger trigger, GameObject finger)
    {
        if (trigger == top)
        {
            if (!isUp)
            {
                switchTime = 0;
            }
        } 
        else // Trigger is bottom collider
        {
            if (isUp)
            {
                switchTime = 0;
            }
        }

        if (switchTime < timeLimit)
        {
            return;
        }

        bool fireHaptics = false;

        if (trigger == top)
        {
            if (isUp)
            {
                fireHaptics = true;
                isUp = false;
                animator.SetBool("isUp", false);
            }
        }
        else
        {
            if (!isUp)
            {
                fireHaptics = true;
                isUp = true;
                animator.SetBool("isUp", true);
            }
        }

        if (fireHaptics)
        {
            BuzzHaptic(finger);
        }

        switchTime = 0;
    }

    public bool GetIsUp()
    {
        return isUp;
    }
}
