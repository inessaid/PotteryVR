using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticTexture : MonoBehaviour
{
    private List<int> collidingFingers = new List<int>();

    // Update is called once per frame
    void Update()
    {
        sendTextureHaptics();
    }

    private void OnTriggerEnter(Collider other)
    {
        int finger = GetFingerIndex(other);
        if (finger > -1)
        {
            collidingFingers.Add(finger);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        int finger = GetFingerIndex(other);
        if (finger > -1)
        {
            collidingFingers.Remove(finger);
        }
    }

    private int GetFingerIndex(Collider col)
    {
        GloveController controller = col.gameObject.GetComponentInParent<GloveController>();
        int finger = -1;

        if (controller == null)
        {
            return finger;
        }
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                if (col.gameObject == controller.middleJoints[i])
                {
                    finger = i;
                    break;
                }
            }
            else if (col.gameObject == controller.tipJoints[i])
            {
                finger = i;
                break;
            }
        }

        return finger;
    }

    private void sendTextureHaptics()
    {

    }
}
