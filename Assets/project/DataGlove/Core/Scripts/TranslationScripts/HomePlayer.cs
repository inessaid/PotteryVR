using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePlayer : MonoBehaviour
{
    public GameObject vrCamera;
    private Vector3 origin;

    private void Start()
    {
        origin = transform.localPosition;
        Vector3 newPosition = origin;
        newPosition.x -= PlayerPrefs.GetFloat("CamX", 0);
        newPosition.z -= (PlayerPrefs.GetFloat("CamZ", 0) - 0.36f);
        transform.localPosition = newPosition;
    }

    public void HomePosition()
    {
        Vector3 newPosition = origin;
        newPosition.x -= vrCamera.transform.localPosition.x;
        newPosition.z -= (vrCamera.transform.localPosition.z - 0.36f);
        transform.localPosition = newPosition;

        PlayerPrefs.SetFloat("CamX", vrCamera.transform.localPosition.x);
        PlayerPrefs.SetFloat("CamZ", vrCamera.transform.localPosition.z);
    }
}
