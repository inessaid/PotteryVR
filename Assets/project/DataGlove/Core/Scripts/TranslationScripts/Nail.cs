using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : MonoBehaviour
{
    private GameObject nail;
    private Vector3 oldPosition;
    float length = 0.02f;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        nail = gameObject;
        oldPosition = nail.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (nail.transform.localPosition.y < oldPosition.y - (length * 3))
        {
            nail.transform.localPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hammer" && timer <= 0)
        {
            timer = 0.1f;

            nail.transform.localPosition = new Vector3(oldPosition.x, nail.transform.localPosition.y - length, oldPosition.z);

            HapticController haptics = other.gameObject.GetComponentInParent<HapticController>();

            if (haptics != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    haptics.HapticPulse(i, 0.1f);
                }
            }
        }
    }
}
