using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DataGlove;
using System.IO;
using UnityEngine.SceneManagement;

/// This script controls a 3D hand model and calibrates raw sensor data.
/**
 * This script controls a hand by affecting the hand's rotation, and contracts
 * the fingers by rotating the finger joints.
 */
public class GloveController : MonoBehaviour {
    public int fingerAxis = 0;
    public bool rightHandGlove;
    public bool rotateOnWrist = false;
    public bool translationEnabled = false;
    public bool swapLeftRight = false;
    public float smoothInputVal = 0;
    public bool isGrabbing = false;
    public bool recallOnStart = false;
    public int recallSlot = 0;
    public int saveSlot = 0;
    public bool smoothRotation = false;
    public float QuatLerpTime = 0.5f;
    public string bluetoothScanName = "";
    public bool useBluetoothDeviceScanName;
    public bool useUISelectionMenu = false;
    public float thumbCurveIn = 0.5f;
    public float thumbCurveOut = 0.5f;
    public bool loadArmature = false;
    public GameObject[] knuckleJoints = new GameObject[NUM_OF_FINGERS];
    public GameObject[] middleJoints = new GameObject[NUM_OF_FINGERS];
    public GameObject[] tipJoints = new GameObject[NUM_OF_FINGERS];
    public GameObject wrist = null;
    public GameObject elbow = null;

    // The dead zone is a 0.0 - 1.0 value that makes it easier to make a flat hand
    // This also masks a problem called hyperextension when fingers bend backwards from the flat calibration position
    // A zero value means there is no dead zone - a 0.5 value means that a sensor must curl 50% before a flat joint moves.
    public float[] thumbDeadZone = new float[2];
    public float[] indexDeadZone = new float[2];
    public float[] middleDeadZone = new float[2];
    public float[] ringDeadZone = new float[2];
    public float[] pinkyDeadZone = new float[2];

    public bool stayPinching = false;
    public float minPinch = 1f;
    public int minIndex = -1;
    public bool[] inPinch = new bool[4];

    private const int NUM_OF_FINGERS = 5;
    private const float TIP_MULTIPLIER = 0.5f;
    private const float THUMB_WEIGHT_S1 = 0.5f;

    private float[] indexPinch;
    private float[] middlePinch;
    private float[] ringPinch;
    private float[] pinkyPinch;
    private float[] pinchDistance = new float[4];

    private DataGloveIO dataGlove;
    private Forte_Armature armature = new Forte_Armature();
    private Quaternion newQuat;
    private Quaternion prevQuat;
    private FingerVals[] fingerVals; // Current finger values
    private FingerVals[] prevFingerVals; // Previous finger values
    private FingerVals[] posedFingerVals;
    private Vector3 oldPosition;
    private Vector3 newPosition;
    private Vector3 palmVelocity;
    private Triggers triggers;
    private float[] fingerDeltas = { 0, 0, 0, 0, 0 };
    private float sensorSum;

    private float[] pose = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private float posePercent = 0;
    private bool transitionToPose = false;
    private float transitionRate;

    private List<Collider> colliders = new List<Collider>();

    void Awake()
    {
        if (wrist == null)
        {
            wrist = new GameObject();
        }

        if (elbow == null)
        {
            elbow = new GameObject();
        }

        if (thumbDeadZone.Length == 0) {
            thumbDeadZone = new float[2];
        }
        if (indexDeadZone.Length == 0)
        {
            indexDeadZone = new float[2];
        }
        if (middleDeadZone.Length == 0)
        {
            middleDeadZone = new float[2];
        }
        if (ringDeadZone.Length == 0)
        {
            ringDeadZone = new float[2];
        }
        if (pinkyDeadZone.Length == 0)
        {
            pinkyDeadZone = new float[2];
        }

        SetupCollidersList();
        gameObject.AddComponent<DataGloveIO>();
        dataGlove = GetComponent<DataGloveIO>();
        
        if (!useBluetoothDeviceScanName)
        {
            bluetoothScanName = "";
        }

        if (useUISelectionMenu)
        {
            dataGlove.SetupGlove(rightHandGlove, "SCAN MODE");
        }
        else
        {
            dataGlove.SetupGlove(rightHandGlove, bluetoothScanName);
        }

        QualitySettings.vSyncCount = 0;
        fingerVals = new FingerVals[NUM_OF_FINGERS];
        prevFingerVals = fingerVals;
        posedFingerVals = new FingerVals[NUM_OF_FINGERS];

        if (loadArmature)
        {
            armature.LoadDynamically(gameObject);
        }
        else
        {
            armature.knuckleJoints = knuckleJoints;
            armature.middleJoints = middleJoints;
            armature.tipJoints = tipJoints;
            armature.wrist = wrist;
            armature.elbow = elbow;
        }
    }

    /// Initialize the hand values.
	void Start () {
        Forte_GloveManager gloveManager = Forte_GloveManager.Instance;

        if (gloveManager != null)
        {
            if (rightHandGlove)
            {
                gloveManager.rightGlove = this;
            }
            else
            {
                gloveManager.leftGlove = this;
            }
        }

        oldPosition = armature.wrist.transform.position;
        sensorSum = 0;
        prevQuat = new Quaternion();
        triggers = new Triggers();

        for (int i = 0; i < 4; i++)
        {
            inPinch[i] = false;
        }
    }

    /// Each frame the new sensor values will be assigned.
    private void Update()
    {
        ProcessKeyInputs();
        ReceiveSensorVals();

        UpdateRotation();
        UpdateFingers();

        CalcFingerDeltas();

        CalculateVelocity();
        DetectGrab();

        LerpPose();
    }

    private void UpdateRotation()
    {
        newQuat = dataGlove.GetIMU();

        if (smoothRotation)
        {
            newQuat = Quaternion.Lerp(prevQuat, newQuat, QuatLerpTime * Time.deltaTime);
            prevQuat = newQuat;
        }

        if (translationEnabled)
        {
            // Don't rotate
            return;
        }

        if (rotateOnWrist)
        {
            if (rightHandGlove)
            {
                armature.wrist.transform.localRotation = newQuat;
            }
            else
            {
                Quaternion leftHand = new Quaternion(newQuat.x, -newQuat.y, -newQuat.z, newQuat.w);
                armature.wrist.transform.localRotation = leftHand;
            }
        }
        else
        {
            if (swapLeftRight)
            {
                Quaternion swapDirection = new Quaternion(newQuat.x, -newQuat.y, -newQuat.z, newQuat.w);
                armature.elbow.transform.localRotation = swapDirection;
            }
            else
            {
                armature.elbow.transform.localRotation = newQuat;
            }
        }
    }

    /// Updates the rotation of the finger joints based on the sensor values and the tip multiplier.
    private void UpdateFingers()
    {
        float adjustedRotation;

        // Thumb offsets are optional.
        Vector3 knuckleRot = new Vector3();
        Vector3 middleRot = new Vector3();
        Vector3 tipRot = new Vector3();

        sensorSum = 0;

        for (int i = 0; i < NUM_OF_FINGERS; i++)
        {
            knuckleRot = armature.knuckleJoints[i].transform.localRotation.eulerAngles;
            middleRot = armature.middleJoints[i].transform.localRotation.eulerAngles;

            float calibratedS1 = posedFingerVals[i].sensor1 * 75.0f;
            float calibratedS2 = posedFingerVals[i].sensor2 * 75.0f;

            sensorSum += fingerVals[i].sensor1 + fingerVals[i].sensor2;

            // Special handling for the thumb finger.
            if (i == 0)
            {
                float calibratedSum = (calibratedS1 + calibratedS2) * 0.8f;

                // The thumb weight of sensor 1 is any value between 0.0 and 1.0
                // If the sensor 1 weight is .25 then the sensor 2 weight is .75 
                calibratedS1 = calibratedSum * THUMB_WEIGHT_S1;
                calibratedS2 = calibratedSum * (1 - THUMB_WEIGHT_S1);

            }

            if (fingerAxis == 0)
            {
                knuckleRot.x = -calibratedS1;
                middleRot.x = -calibratedS2;
            }
            else if (fingerAxis == 1)
            {
                knuckleRot.y = -calibratedS1;
                middleRot.y = -calibratedS2;
            }
            else
            {
                knuckleRot.z = -calibratedS1;
                middleRot.z = -calibratedS2;
            }

            armature.knuckleJoints[i].transform.localRotation = Quaternion.Euler(knuckleRot.x, knuckleRot.y, knuckleRot.z);
            armature.middleJoints[i].transform.localRotation = Quaternion.Euler(middleRot.x, middleRot.y, middleRot.z);

            // Non thumb fingers need tip joint angle computation.
            if (i > 0)
            {
                // Tip multiplier values greater than 1.0 causes the final finger bone to rotate faster
                // than its parent finger bone, while a lower value causes a slower rotation.
                adjustedRotation = posedFingerVals[i].sensor2 * 127 * TIP_MULTIPLIER;

                tipRot = armature.tipJoints[i].transform.localRotation.eulerAngles;

                if (fingerAxis == 0)
                {
                    tipRot.x = -adjustedRotation;
                }
                else if (fingerAxis == 1)
                {
                    tipRot.y = -adjustedRotation;
                }
                else
                {
                    tipRot.z = -adjustedRotation;
                }

                armature.tipJoints[i].transform.localRotation = Quaternion.Euler(tipRot.x, tipRot.y, tipRot.z);
            }
        }
    }

    /// Remap a value to another range
    float Remap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        float scaled = outMin + (float)(value - inMin) / (inMax - inMin) * (outMax - outMin);
        return scaled;
    }

    /// Process keys pressed by the user.
    private void ProcessKeyInputs()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Type the "F" key with hand flat to set the min sensor values.
            dataGlove.CalibrateFlat();
            dataGlove.SaveCalibration(saveSlot);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            // Type the "K" key while making a fist with thumb out.
            dataGlove.CalibrateFingersIn();
            dataGlove.SaveCalibration(saveSlot);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            // Type "T" with hand flat and thumb in.
            dataGlove.CalibrateThumbIn();
            dataGlove.SaveCalibration(saveSlot);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            dataGlove.SaveCalibration(saveSlot);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dataGlove.RecallCalibration(recallSlot);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            dataGlove.ResetCalibration();
        }
        else if (Input.GetKeyDown(KeyCode.H)) {
            dataGlove.SetIMUHome();
        }
    }

    /// Gets the finger sensor values from the DataGloveIO class.
    /**
     */
    private void ReceiveSensorVals()
    {
        fingerVals = dataGlove.GetSensors();
        float pre = fingerVals[0].sensor1;
        
        if (fingerVals[0].sensor1 > fingerVals[0].sensor2)
        {
            fingerVals[0].sensor2 = fingerVals[0].sensor1;
        }
        else
        {
            fingerVals[0].sensor1 = fingerVals[0].sensor2;
        }

        fingerVals = ApplyCurve(fingerVals);
        fingerVals = ApplyDeadZone(fingerVals);

        FingerVals[] blendedVals = BlendPose(fingerVals);        

        for (int i = 0; i < NUM_OF_FINGERS; i++) {
            posedFingerVals[i].sensor1 = SmoothInput(posedFingerVals[i].sensor1, blendedVals[i].sensor1);
            posedFingerVals[i].sensor2 = SmoothInput(posedFingerVals[i].sensor2, blendedVals[i].sensor2);
        }
    }

    /// Check for a grab gesture.
    private void DetectGrab()
    {
        if (GetNormalizedSensorSum() < 0.3f && isGrabbing) {
            isGrabbing = false;
        } else if (GetNormalizedSensorSum() > 0.3f && !isGrabbing ) {
            isGrabbing = true;
        }
    }

    /// Determine the direction and speed of the hand.
    private void CalculateVelocity()
    {
        newPosition = armature.wrist.transform.position;
        var media = (newPosition - oldPosition);
        palmVelocity = media / Time.deltaTime;
        oldPosition = newPosition;
    }

    /// Smooths the input.
    private float SmoothInput(float prevVal, float newVal)
    {
        if (smoothInputVal < 1)
        {
            return newVal;
        }
        return (float)(((prevVal * smoothInputVal) + newVal) / (smoothInputVal + 1));
    }

    /// Return the direction and speed of the hand.
    public Vector3 GetPalmVelocity(){
        return palmVelocity;
    }

    /// Return the elbow bone.
    public GameObject GetElbow() {
        return armature.elbow;
    }

    /// return the wrist bone.
    public GameObject GetWrist() {
        return armature.wrist;
    }

    /// Gets the sensor sum.
    public float GetSensorSum(){
        return sensorSum;
    }

    /// Gets the normalized sensor sum (0-1)
    public float GetNormalizedSensorSum()
    {
        float sum = 0.0f;
        FingerVals[] fingers = dataGlove.GetSensors();
        foreach(FingerVals finger in fingers)
        {
            sum += finger.sensor1 + finger.sensor2;
        }
        return Mathf.InverseLerp(0.0f, 10.0f, sum);
    }

    /// returns an array of the most recent sensor values for five fingers.
    public FingerVals[] GetSensors() {
        return fingerVals;
    }

    /// returns whether the glove is grabbing.
    public bool GetIsGrabbing(){
        return isGrabbing;
    }

    /// Gets the DataGloveIO instance.
    public DataGloveIO GetDataGlove()
    {
        return dataGlove;
    }

    /// Display the video frame rate;
    /**void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), ((int)(1.0f / Time.smoothDeltaTime)).ToString());
    }*/

    /// Call this method from another script every update frame to check for triggers.
    public List<Triggers.TriggerState> GetTriggers() {
        return triggers.calculateAllTrigs(fingerVals);
    }

    // Call once to initiate a lerp into a pose
    public void EnterPose(float[] nextPose, float rate)
    {
        pose = nextPose;
        posePercent = 0;
        transitionToPose = true;
        transitionRate = rate;
    }

    // Call every frame in Update to update a dynamic (non-fixed) pose
    // Must have called EnterPose or these changes can't be seen.
    public void UpdatePose(float[] nextPose, float rate)
    {
        pose = nextPose;
        transitionRate = rate;
    }

    // Call once to initiate a lerp out of a pose
    public void ExitPose(float rate)
    {
        transitionToPose = false;
        transitionRate = rate;
    }

    // Allows for crossfading between live sensor input and poses
    // A pose is a preset array of sensor values, a fist would be a pose array of all 1s
    // A pose of a flat hand would be an array of all 0s
    private FingerVals[] BlendPose(FingerVals[] newVals)
    {
        FingerVals[] blendedVals = new FingerVals[5];
        int poseIndex = 0;

        for (int i = 0; i < NUM_OF_FINGERS; i++)
        {
            // Pose percent is a float between 0 and 1 that crossfades between live input(newVals) and a set pose
            // A 0 pose percent means live input is passed through untouched, while a 1 means only the pose is returned
            // A 0.5 percent is a 50/50 mix between live and pose 
            blendedVals[i].sensor1 = newVals[i].sensor1 * (1 - posePercent) + (pose[poseIndex] * posePercent);
            poseIndex++;

            blendedVals[i].sensor2 = newVals[i].sensor2 * (1 - posePercent) + (pose[poseIndex] * posePercent);
            poseIndex++;
        }

        return blendedVals;
    }

    // Called in the update loop to determine the pose percent based on the transition rate
    private void LerpPose()
    {
        if (transitionToPose)
        {
            posePercent += Time.deltaTime * transitionRate;
            if (posePercent > 1)
            {
                posePercent = 1;
            }
        }
        else
        {
            posePercent -= Time.deltaTime * transitionRate;
            if (posePercent < 0)
            {
                posePercent = 0;
            }
        }
    }

    // upload a processed .wav or .bin file to slots 0-13 of the data glove
    public void UploadFile(string filename, int uploadSlot)
    {
        dataGlove.UploadFile(filename, uploadSlot);
    }

    private FingerVals[] ApplyDeadZone(FingerVals[] fingers)
    {
        FingerVals[] deadZoneVals = new FingerVals[NUM_OF_FINGERS];

        deadZoneVals[0].sensor1 = Remap(Mathf.Clamp(fingers[0].sensor1, thumbDeadZone[0], 1), thumbDeadZone[0], 1, 0, 1);
        deadZoneVals[0].sensor2 = Remap(Mathf.Clamp(fingers[0].sensor2, thumbDeadZone[1], 1), thumbDeadZone[1], 1, 0, 1);
        deadZoneVals[1].sensor1 = Remap(Mathf.Clamp(fingers[1].sensor1, indexDeadZone[0], 1), indexDeadZone[0], 1, 0, 1);
        deadZoneVals[1].sensor2 = Remap(Mathf.Clamp(fingers[1].sensor2, indexDeadZone[1], 1), indexDeadZone[1], 1, 0, 1);
        deadZoneVals[2].sensor1 = Remap(Mathf.Clamp(fingers[2].sensor1, middleDeadZone[0], 1), middleDeadZone[0], 1, 0, 1);
        deadZoneVals[2].sensor2 = Remap(Mathf.Clamp(fingers[2].sensor2, middleDeadZone[1], 1), middleDeadZone[1], 1, 0, 1);
        deadZoneVals[3].sensor1 = Remap(Mathf.Clamp(fingers[3].sensor1, ringDeadZone[0], 1), ringDeadZone[0], 1, 0, 1);
        deadZoneVals[3].sensor2 = Remap(Mathf.Clamp(fingers[3].sensor2, ringDeadZone[1], 1), ringDeadZone[1], 1, 0, 1);
        deadZoneVals[4].sensor1 = Remap(Mathf.Clamp(fingers[4].sensor1, pinkyDeadZone[0], 1), pinkyDeadZone[0], 1, 0, 1);
        deadZoneVals[4].sensor2 = Remap(Mathf.Clamp(fingers[4].sensor2, pinkyDeadZone[1], 1), pinkyDeadZone[1], 1, 0, 1);

        return deadZoneVals;
    }

    private FingerVals[] ApplyCurve(FingerVals[] fingers)
    {
        FingerVals[] newVals = new FingerVals[NUM_OF_FINGERS];

        float thumbAvg = (fingers[0].sensor1 + fingers[0].sensor2) / 2f;
        float newThumb;

        if (thumbAvg <= thumbCurveIn)
        {
            newThumb = Remap(thumbAvg, 0, thumbCurveIn, 0, thumbCurveOut);
        }
        else
        {
            newThumb = Remap(thumbAvg, thumbCurveIn, 1, thumbCurveOut, 1);
        }

        newVals[0].sensor1 = newThumb;
        newVals[0].sensor2 = newThumb;
        newVals[1].sensor1 = fingers[1].sensor1;
        newVals[1].sensor2 = fingers[1].sensor2;
        newVals[2].sensor1 = fingers[2].sensor1;
        newVals[2].sensor2 = fingers[2].sensor2;
        newVals[3].sensor1 = fingers[3].sensor1;
        newVals[3].sensor2 = fingers[3].sensor2;
        newVals[4].sensor1 = fingers[4].sensor1;
        newVals[4].sensor2 = fingers[4].sensor2;

        return newVals;
    }

    public string[] GetAvailableBluetoothDeviceScanNames()
    {
        if (dataGlove != null)
        {
            return dataGlove.GetAvailableBluetoothDeviceNames();
        }
        else
        {
            return new string[0];
        }
    }

    public void ConnectToBluetoothDeviceByName(string name)
    {
        bluetoothScanName = name;
        dataGlove.SetBluetoothDeviceByName(name);
    }

    public bool IsConnected()
    {
        if (dataGlove == null)
        {
            return false;
        }

        return dataGlove.IsConnected();
    }

    public float[] GetFingerDeltas(){
        return fingerDeltas;
    }

    private void CalcFingerDeltas()
    {
        for (int i = 0; i < NUM_OF_FINGERS; i++)
        {
            float newSum = fingerVals[i].sensor1 + fingerVals[i].sensor2;
            float prevSum = prevFingerVals[i].sensor1 + prevFingerVals[i].sensor2;
            fingerDeltas[i] = newSum - prevSum;
        }

        prevFingerVals = fingerVals;
    }


    private void SetupCollidersList()
    {
        Collider[] handColliders = wrist.GetComponentsInChildren<Collider>();
        foreach (Collider col in handColliders)
        {
            colliders.Add(col);
        }
    }

    public List<Collider> GetColliders(){
        return colliders;
    }

    // Determine if a collider belongs to a fingertip
    public bool IsTipCollider(Collider col)
    {
        GameObject fingerSection = col.gameObject;

        if (fingerSection == middleJoints[0])
        {
            return true;
        }

        for (int i = 1; i < 5; i++)
        {
            if (fingerSection == tipJoints[i])
            {
                return true;
            }
        }

        return false;
    }

    // Returns all ten of the sensor finger values as a list of floats
    public float[] getFingerFloatsFull()
    {
        FingerVals[] rawfingerVals = dataGlove.GetSensors();
        float[] fingers = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        for (int i = 0; i < 5; i++)
        {
            fingers[i * 2] = rawfingerVals[i].sensor1;
            fingers[i * 2 + 1] = rawfingerVals[i].sensor2;
        }

        return fingers;
    }

    //Pinching calibration methods
    public void SetIndexPinch()
    {
        indexPinch = getFingerFloatsFull();
    }

    public float[] GetIndexPinch()
    {
        return indexPinch;
    }

    public void SetMiddlePinch()
    {
        middlePinch = getFingerFloatsFull();
    }

    public float[] GetMiddlePinch()
    {
        return middlePinch;
    }
    public void SetRingPinch()
    {
        ringPinch = getFingerFloatsFull();
    }

    public float[] GetRingPinch()
    {
        return ringPinch;
    }
    public void SetPinkyPinch()
    {
        pinkyPinch = getFingerFloatsFull();
    }

    public float[] GetPinkyPinch()
    {
        return pinkyPinch;
    }

    private float PoseDistanceFull(float[] p1, float[] p2)
    {
        float thumb   = (p1[0] - p2[0]) * (p1[0] - p2[0]);
        float thumb2  = (p1[1] - p2[1]) * (p1[1] - p2[1]);
        float index   = (p1[2] - p2[2]) * (p1[2] - p2[2]);
        float index2  = (p1[3] - p2[3]) * (p1[3] - p2[3]);
        float middle  = (p1[4] - p2[4]) * (p1[4] - p2[4]);
        float middle2 = (p1[5] - p2[5]) * (p1[5] - p2[5]);
        float ring    = (p1[6] - p2[6]) * (p1[6] - p2[6]);
        float ring2   = (p1[7] - p2[7]) * (p1[7] - p2[7]);
        float pinky   = (p1[8] - p2[8]) * (p1[8] - p2[8]);
        float pinky2  = (p1[9] - p2[9]) * (p1[9] - p2[9]);
        float distance = Mathf.Sqrt((thumb + index + middle +
            ring + pinky + thumb2 + index2 + middle2 + ring2 + pinky2));
        return distance;
    }

    private float checkIndexDistance()
    {
        return PoseDistanceFull(GetIndexPinch(), getFingerFloatsFull());
    }
    private float checkMiddleDistance()
    {
        return PoseDistanceFull(GetMiddlePinch(), getFingerFloatsFull());
    }
    private float checkRingDistance()
    {
        return PoseDistanceFull(GetRingPinch(), getFingerFloatsFull());
    }
    private float checkPinkyDistance()
    {
        return PoseDistanceFull(GetPinkyPinch(), getFingerFloatsFull());
    }

    public float[] GetPinchDistance()
    {
        pinchDistance[0] = checkIndexDistance();
        pinchDistance[1] = checkMiddleDistance();
        pinchDistance[2] = checkRingDistance();
        pinchDistance[3] = checkPinkyDistance();
        return pinchDistance;
    }
}
