using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableHaptics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GloveController controller = other.gameObject.GetComponentInParent<GloveController>();
        if (controller == null) { return; }

        int fingerIndex = -1;
        string lowercaseChild = other.gameObject.name.ToLower();

        if (lowercaseChild.Contains("thumb"))
        {
            fingerIndex = 0;
        }
        else if (lowercaseChild.Contains("index"))
        {
            fingerIndex = 1;
        }
        else if (lowercaseChild.Contains("middle"))
        {
            fingerIndex = 2;
        }
        else if (lowercaseChild.Contains("ring"))
        {
            fingerIndex = 3;
        }
        else if (lowercaseChild.Contains("pinky"))
        {
            fingerIndex = 4;
        }
        
        if (controller.IsTipCollider(other))
        {
            HapticController haptics = controller.gameObject.GetComponent<HapticController>();

            if (haptics != null)
            {
                haptics.HapticPulse(fingerIndex, 0.1f);
            }
        }
    }
}
