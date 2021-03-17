using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DataGlove;

// This script is dynamically instantiated by the Forte_Interactor script for a selection of poses
public class Poses : MonoBehaviour
{
    // Add to this enum when creating a new pose
    public enum Pose
    {
        None, Flat, Fist, Point, Grab, Freeze, FreezeAll, Pinch, PinchMiddle, PinchRing, PinchPinky,
        Letter_A, Letter_B, Letter_C, Letter_D, Letter_E, Letter_F, Letter_G, Letter_H, Letter_I,
        Letter_J, Letter_K, Letter_L, Letter_M, Letter_N, Letter_O, Letter_P, Letter_Q, Letter_R,
        Letter_S, Letter_T, Letter_U, Letter_V, Letter_W, Letter_X, Letter_Y, Letter_Z
    }
    [HideInInspector]
    public FingerVals[] freezePose = new FingerVals[5];
    [HideInInspector]
    public bool[] fingersToFreeze;

    private GloveController gloveController = null;
    private Forte_Interactable activeObject = null;
    private Forte_Interactor interactor;
    private GestureRecognition gesture;
    private float rate;
    private float[] currentPose;
    private bool run = true;
    private int frame_count;
    private float pinchIndex;
    private float pinchMiddle;
    private float pinchRing;
    private float pinchPinky;



    // Start is called before the first frame update
    void Start()
    {
        gloveController = gameObject.GetComponent<GloveController>();
        interactor = GetComponent<Forte_Interactor>();
        gesture = gloveController.GetComponent<GestureRecognition>();

        if (gloveController == null)
        {
            Debug.Log("Error: No Glove Controller found near the Poses module");
        }
    }

    private void Update()
    {
        Forte_Interactable newObj = interactor.GetActiveObject();
        if (newObj != null && newObj.isGrabbed && newObj.grabHandRight == gloveController.rightHandGlove)
        {
            SwitchToObject(interactor.GetActiveObject());
        }
        else if (activeObject != null)
        {
            SwitchToObject(null);
        }
    }

    float[] FreezePose()
    {
        float[] freeze = new float[10];
        FingerVals[] fingerVals = gloveController.GetSensors();

        for (int i = 0; i < 5; i++)
        {
            // Freeze only the colliding fingers
            if (fingersToFreeze[i])
            {
                freeze[i * 2] = freezePose[i].sensor1;
                freeze[i * 2 + 1] = freezePose[i].sensor2;
            }
            else
            {
                freeze[i * 2] = fingerVals[i].sensor1;
                freeze[i * 2 + 1] = fingerVals[i].sensor2;
            }
        }

        return freeze;
    }

    float[] FreezeAllPose()
    {
        float[] freeze = new float[10];

        for (int i = 0; i < 5; i++)
        {
            freeze[i * 2] = freezePose[i].sensor1;
            freeze[i * 2 + 1] = freezePose[i].sensor2;
        }

        return freeze;
    }

    // Poses are 10 float arrays, each value has a range of 0 to 1
    float[] FlatPose()
    {
        float[] flatPose = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        return flatPose;
    }

    // Fully curled fingers
    float[] FistPose()
    {
        float[] fistPose = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        return fistPose;
    }

    // All fingers curled except index finger
    float[] PointPose()
    {
        float[] pointPose = { 1, 1, 0, 0, 1, 1, 1, 1, 1, 1 };
        return pointPose;
    }

    // ------------------------------------------------------------ //
    // Begin alphabet poses

    float[] Letter_A_Pose()
    {
        float[] Letter_A_Pose = { 0.2f, 0.2f, 0.6f, 1, 0.6f, 1, 0.6f, 1, 0.6f, 1 };
        return Letter_A_Pose;
    }

    float[] Letter_B_Pose()
    {
        float[] Letter_B_Pose = { 0.7f, 0.5f, 0, 0, 0, 0, 0, 0, 0, 0 };
        return Letter_B_Pose;
    }

    float[] Letter_C_Pose()
    {
        float[] Letter_C_Pose = { 0.25f, 0.25f, 0.4f, 0.6f, 0.4f, 0.6f, 0.4f, 0.6f, 0.4f, 0.6f };
        return Letter_C_Pose;
    }

    float[] Letter_D_Pose()
    {
        float[] Letter_D_Pose = { 0.45f, 0.45f, 0, 0, 0.2f, 0.85f, 0.2f, 0.85f, 0.2f, 0.85f };
        return Letter_D_Pose;
    }

    float[] Letter_E_Pose()
    {
        float[] Letter_E_Pose = { 0.9f, 0.9f, 0.55f, 0.8f, 0.45f, 0.8f, 0.45f, 0.8f, 0.55f, 0.8f };
        return Letter_E_Pose;
    }

    float[] Letter_F_Pose()
    {
        float[] Letter_F_Pose = { 0.35f, 0.35f, 0.15f, 0.63f, 0, 0, 0, 0, 0, 0 };
        return Letter_F_Pose;
    }

    float[] Letter_G_Pose()
    {
        float[] Letter_G_Pose = { 0.2f, 0.2f, 0, 0, 0.6f, 0.9f, 0.6f, 0.9f, 0.6f, 1 };
        return Letter_G_Pose;
    }

    float[] Letter_H_Pose()
    {
        float[] Letter_H_Pose = { 0.2f, 0.2f, 0, 0, 0, 0, 0.6f, 1, 0.6f, 1 };
        return Letter_H_Pose;
    }

    float[] Letter_I_Pose()
    {
        float[] Letter_I_Pose = { 0.35f, 0.35f, 0.6f, 1, 0.6f, 1, 0.6f, 1, 0, 0 };
        return Letter_I_Pose;
    }

    float[] Letter_J_Pose()
    {
        float[] Letter_J_Pose = { 0.35f, 0.35f, 0.6f, 1, 0.6f, 1, 0.6f, 1, 0, 0 };
        return Letter_J_Pose;
    }

    float[] Letter_K_Pose()
    {
        float[] Letter_K_Pose = { 0.8f, 0.35f, 0, 0, 0, 0, 0.6f, 0.9f, 0.6f, 0.9f };
        return Letter_K_Pose;
    }

    float[] Letter_L_Pose()
    {
        float[] Letter_L_Pose = { 0, 0, 0, 0, 0.6f, 1, 0.6f, 1, 0.6f, 1 };
        return Letter_L_Pose;
    }

    float[] Letter_M_Pose()
    {
        float[] Letter_M_Pose = { 0.45f, 0.45f, 0.4f, 1, 0.6f, 1, 0.6f, 1, 0.6f, 1 };
        return Letter_M_Pose;
    }

    float[] Letter_N_Pose()
    {
        float[] Letter_N_Pose = { 0.65f, 0.65f, 0.4f, 1, 0.4f, 1, 0.6f, 1, 0.6f, 1 };
        return Letter_N_Pose;
    }

    float[] Letter_O_Pose()
    {
        float[] Letter_O_Pose = { 0.3f, 0.3f, 0.15f, 0.6f, 0.15f, 0.6f, 0.15f, 0.6f, 0.15f, 0.6f };
        return Letter_O_Pose;
    }

    float[] Letter_P_Pose()
    {
        float[] Letter_P_Pose = { 0.8f, 0.35f, 0, 0, 0.1f, 0, 0.6f, 0.9f, 0.6f, 0.9f };
        return Letter_P_Pose;
    }

    float[] Letter_Q_Pose()
    {
        float[] Letter_Q_Pose = { 0.2f, 0.2f, 0, 0, 0.6f, 0.9f, 0.6f, 0.9f, 0.6f, 1 };
        return Letter_Q_Pose;
    }

    float[] Letter_U_Pose()
    {
        float[] Letter_U_Pose = { 0.7f, 0.7f, 0, 0, 0, 0, 0.6f, 1, 0.6f, 1 };
        return Letter_U_Pose;
    }

    float[] Letter_V_Pose()
    {
        float[] Letter_V_Pose = { 0.7f, 0.7f, 0, 0, 0, 0, 0.6f, 1, 0.6f, 1 };
        return Letter_V_Pose;
    }

    float[] Letter_W_Pose()
    {
        float[] Letter_W_Pose = { 0.87f, 0.87f, 0, 0, 0, 0, 0, 0, 0.45f, 0.95f };
        return Letter_W_Pose;
    }

    float[] Letter_X_Pose()
    {
        float[] Letter_X_Pose = { 0.45f, 0.45f, 0, 0.5f, 0.2f, 0.85f, 0.2f, 0.85f, 0.2f, 0.85f };
        return Letter_X_Pose;
    }

    float[] Letter_Y_Pose()
    {
        float[] Letter_Y_Pose = { 0, 0, 0.5f, 1, 0.5f, 1, 0.5f, 1, 0, 0 };
        return Letter_Y_Pose;
    }

    float[] Letter_Z_Pose()
    {
        float[] Letter_Z_Pose = { 0.45f, 0.45f, 0, 0, 0.2f, 0.85f, 0.2f, 0.85f, 0.2f, 0.85f };
        return Letter_Z_Pose;
    }

    // ----------------------------------------------------------------- //
    // Begin Pinch poses

    // All fingers curl equally up to a 50% curl to simulate grabbing a large object
    float[] GrabPose()
    {
        float grabCurl = gloveController.GetNormalizedSensorSum();

        grabCurl = Mathf.Clamp(grabCurl, 0, 0.5f);

        float[] grabPose = { grabCurl, grabCurl, grabCurl, grabCurl, grabCurl,
                             grabCurl, grabCurl, grabCurl, grabCurl, grabCurl };
        return grabPose;
    }

    // The Pinch pose leaves all fingers un modified but the thumb and index finger curl to max 60% 
    // -- used for picking up small objects
    public float[] PinchPose()
    {
        FingerVals[] fingerVals = gloveController.GetSensors();

        float thumbCurl = 0.6f;
        float fingerKnuckle = 0.42f;
        float fingerJoint = 0.78f;

        float[] pinchPose = { thumbCurl, thumbCurl, fingerKnuckle, fingerJoint, fingerVals[2].sensor1, fingerVals[2].sensor2,
                              fingerVals[3].sensor1, fingerVals[3].sensor2, fingerVals[4].sensor1, fingerVals[4].sensor2 };
        return pinchPose;
    }

    // The Pinch pose leaves all fingers un modified but the thumb and index finger curl to max 60% 
    // -- used for picking up small objects
    public float[] PinchMiddlePose()
    {
        FingerVals[] fingerVals = gloveController.GetSensors();

        float thumbCurl = 0.8f;
        float fingerKnuckle = 0.4f;
        float fingerJoint = 0.85f;

        float[] pinchMiddlePose = { thumbCurl, thumbCurl,  fingerVals[1].sensor1, fingerVals[1].sensor2, fingerKnuckle, fingerJoint,
                              fingerVals[3].sensor1, fingerVals[3].sensor2, fingerVals[4].sensor1, fingerVals[4].sensor2 };
        return pinchMiddlePose;
    }

    public float[] PinchRingPose()
    {
        FingerVals[] fingerVals = gloveController.GetSensors();

        float thumbCurl = 0.95f;
        float fingerKnuckle = 0.4f;
        float fingerJoint = 0.85f;

        float[] pinchRingPose = { thumbCurl, thumbCurl,  fingerVals[1].sensor1, fingerVals[1].sensor2, fingerVals[2].sensor1, fingerVals[2].sensor2,
                              fingerKnuckle, fingerJoint, fingerVals[4].sensor1, fingerVals[4].sensor2 };
        return pinchRingPose;
    }

    public float[] PinchPinkyPose()
    {
        FingerVals[] fingerVals = gloveController.GetSensors();

        float thumbCurl = 0.95f;
        float fingerKnuckle = 0.55f;
        float fingerJoint = 0.95f;

        float[] pinchPinkyPose = { thumbCurl, thumbCurl,  fingerVals[1].sensor1, fingerVals[1].sensor2, fingerVals[2].sensor1, fingerVals[2].sensor2,
                               fingerVals[3].sensor1, fingerVals[3].sensor2, fingerKnuckle, fingerJoint };
        return pinchPinkyPose;
    }

    // The sensor values get passed through with no fingers modified
    float[] NonePose()
    {
        FingerVals[] fingerVals = gloveController.GetSensors();

        float[] nonePose = { fingerVals[0].sensor1, fingerVals[0].sensor2, fingerVals[1].sensor1, fingerVals[1].sensor2,
                              fingerVals[2].sensor1, fingerVals[2].sensor2, fingerVals[3].sensor1, fingerVals[3].sensor2,
                                fingerVals[4].sensor1, fingerVals[4].sensor2};
        return nonePose;
    }

    // Every update, check if the active object has changed
    private void SwitchToObject(Forte_Interactable newActive)
    {
        if (newActive == null)
        {
            // Tell the controller to exit the pose 
            gloveController.ExitPose(rate);
            activeObject = newActive;
            return;
        }

        if (newActive != activeObject)
        {
            // Start a new pose transition
            currentPose = CalculatePose(newActive.pose);
            rate = 10f;
            gloveController.EnterPose(currentPose, rate);
        }

        activeObject = newActive;

        currentPose = CalculatePose(activeObject.pose);
        gloveController.UpdatePose(currentPose, rate);
    }

    // Returns the modified finger values for all fingers after the pose is applied 
    float[] CalculatePose(Pose newPose)
    {
        float[] poseValues;
        //Console.WriteLine(Pose.newPose);

        switch (newPose)
        {
            case Pose.Flat:
                poseValues = FlatPose();
                break;
            case Pose.Fist:
                poseValues = FistPose();
                break;
            case Pose.Grab:
                poseValues = GrabPose();
                break;
            case Pose.Point:
                poseValues = PointPose();
                break;

            case Pose.Freeze:
                poseValues = FreezePose();
                break;
            case Pose.FreezeAll:
                poseValues = FreezeAllPose();
                break;

            case Pose.Pinch:
                poseValues = PinchPose();
                break;
            case Pose.PinchMiddle:
                poseValues = PinchMiddlePose();
                break;
            case Pose.PinchRing:
                poseValues = PinchRingPose();
                break;
            case Pose.PinchPinky:
                poseValues = PinchPinkyPose();
                break;

            case Pose.Letter_A:
                poseValues = Letter_A_Pose();
                break;
            case Pose.Letter_B:
                poseValues = Letter_B_Pose();
                break;
            case Pose.Letter_C:
                poseValues = Letter_C_Pose();
                break;
            case Pose.Letter_D:
                poseValues = Letter_D_Pose();
                break;
            case Pose.Letter_E:
                poseValues = Letter_E_Pose();
                break;
            case Pose.Letter_F:
                poseValues = Letter_F_Pose();
                break;
            case Pose.Letter_G:
                poseValues = Letter_G_Pose();
                break;
            case Pose.Letter_H:
                poseValues = Letter_H_Pose();
                break;
            case Pose.Letter_I:
                poseValues = Letter_I_Pose();
                break;
            case Pose.Letter_J:
                poseValues = Letter_J_Pose();
                break;
            case Pose.Letter_K:
                poseValues = Letter_K_Pose();
                break;
            case Pose.Letter_L:
                poseValues = Letter_L_Pose();
                break;
            case Pose.Letter_M:
                poseValues = Letter_M_Pose();
                break;
            case Pose.Letter_N:
                poseValues = Letter_N_Pose();
                break;
            case Pose.Letter_O:
                poseValues = Letter_O_Pose();
                break;
            case Pose.Letter_P:
                poseValues = Letter_P_Pose();
                break;
            case Pose.Letter_Q:
                poseValues = Letter_Q_Pose();
                break;
            case Pose.Letter_U:
                poseValues = Letter_U_Pose();
                break;
            case Pose.Letter_V:
                poseValues = Letter_V_Pose();
                break;
            case Pose.Letter_W:
                poseValues = Letter_W_Pose();
                break;
            case Pose.Letter_X:
                poseValues = Letter_X_Pose();
                break;
            case Pose.Letter_Y:
                poseValues = Letter_Y_Pose();
                break;
            case Pose.Letter_Z:
                poseValues = Letter_Z_Pose();
                break;

            default:
                poseValues = NonePose();
                break;
        }

        return poseValues;
    }

    public float[] CalculatePoseASL(string newPose)
    {
        float[] poseValues;
        //Console.WriteLine(Pose.newPose);

        switch (newPose)
        {
            case "letter_A":
                poseValues = Letter_A_Pose();
                break;
            case "letter_B":
                poseValues = Letter_B_Pose();
                break;
            case "letter_C":
                poseValues = Letter_C_Pose();
                break;
            case "letter_D":
                poseValues = Letter_D_Pose();
                break;
            case "letter_E":
                poseValues = Letter_E_Pose();
                break;
            case "letter_F":
                poseValues = Letter_F_Pose();
                break;
            case "letter_G":
                poseValues = Letter_G_Pose();
                break;
            case "letter_H":
                poseValues = Letter_H_Pose();
                break;
            case "letter_I":
                poseValues = Letter_I_Pose();
                break;
            case "letter_J":
                poseValues = Letter_J_Pose();
                break;
            case "letter_K":
                poseValues = Letter_K_Pose();
                break;
            case "letter_L":
                poseValues = Letter_L_Pose();
                break;
            case "letter_M":
                poseValues = Letter_M_Pose();
                break;
            case "letter_N":
                poseValues = Letter_N_Pose();
                break;
            case "letter_O":
                poseValues = Letter_O_Pose();
                break;
            case "letter_P":
                poseValues = Letter_P_Pose();
                break;
            case "letter_Q":
                poseValues = Letter_Q_Pose();
                break;
            case "letter_U":
                poseValues = Letter_U_Pose();
                break;
            case "letter_V":
                poseValues = Letter_V_Pose();
                break;
            case "letter_W":
                poseValues = Letter_W_Pose();
                break;
            case "letter_X":
                poseValues = Letter_X_Pose();
                break;
            case "letter_Y":
                poseValues = Letter_Y_Pose();
                break;
            case "letter_Z":
                poseValues = Letter_Z_Pose();
                break;

            default:
                poseValues = NonePose();
                break;
        }
        return poseValues;
    }
}
