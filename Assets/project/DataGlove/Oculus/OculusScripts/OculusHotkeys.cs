using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusHotkeys : MonoBehaviour
{
    public CalibrationSequence calibrationSequence;
    public PinchDetection pinch;
    public GameObject menu;
    private bool toggleMenu = true;

    // Update is called once per frame
    void Update()
    {

        // NOTE for other other hotkeys in the scene:
        // The position/rotaiton reset hotkeys for Rift and Quest 
        // are handled in ResetObject.cs

        // homing for the Oculus Rift
        if(Input.GetKeyDown(KeyCode.H))
        {
            UnityEngine.XR.InputTracking.Recenter();
        }

        if(OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch) || 
           OVRInput.GetDown(OVRInput.RawButton.X, OVRInput.Controller.LTouch))
        {
            Debug.Log("Start Calibration Sequence");
            calibrationSequence.StartCalibration();
        }

        if(OVRInput.GetDown(OVRInput.RawButton.Start, OVRInput.Controller.LTouch))
        {
            Debug.Log("Quitting App");
            Application.Quit(0);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Start Pinch Calibration Sequence");
            pinch.StartCalibration();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ToggleSelectionMenu();
        }
    }

    private void ToggleSelectionMenu()
    {
        if (toggleMenu)
        {
            menu.SetActive(false);
            toggleMenu = false;
        }
        else
        {
            menu.SetActive(true);
            toggleMenu = true;
        }
    }
}
