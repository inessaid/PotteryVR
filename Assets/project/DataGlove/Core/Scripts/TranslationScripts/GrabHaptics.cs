using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHaptics : MonoBehaviour
{
    private HapticController haptics;
    private Rigidbody rigidBody;
    private bool wasKinematic = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        HapticController newHaptics = collision.gameObject.GetComponent<HapticController>();
        if (newHaptics != null)
        {
            newHaptics = collision.gameObject.GetComponentInChildren<HapticController>();
        }
        if (newHaptics != null)
        {
            haptics = newHaptics;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidBody.isKinematic && !wasKinematic && haptics != null)
        {
            for (int i = 0; i < 6; i++)
            {
                haptics.HapticPulse(i, 0.1f);
            }
        }
        wasKinematic = rigidBody.isKinematic;
    }
}
