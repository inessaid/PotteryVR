using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCailibration : MonoBehaviour
{
    CalibrationSequence sequence;

    // Start is called before the first frame update
    void Start()
    {
        sequence = FindObjectOfType<CalibrationSequence>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (sequence != null)
            {
                sequence.StartCalibration();
            }
        }
    }
}
