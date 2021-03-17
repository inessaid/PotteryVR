using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataGlove;

/// Data Glove module that determines what pose the hand is in.
/**
 * A Pose is a finger arrangement. 
 * If all fingers are curled then that counts as a grab pose.
  */
public class PoseRecognizer : MonoBehaviour {

    private float nextTest;
    private GloveController glove;
    private FingerVals[] fingerVals;
    private float interval = 0.25f;
    private string currentPose = "";
    private List<string> poseBuffer = new List<string>();
    private Dictionary<string, float[]> poses = new Dictionary<string, float[]>();

    /// Use this for initialization
    
    void Start () {
        nextTest = Time.time + interval;
        glove = FindObjectOfType<GloveController>();
        SetupPoseDefaultLibrary();
    }
	
	/// Update is called once per frame
    void Update () {
        fingerVals = glove.GetDataGlove().GetSensors();
        BufferManager();
    }

    /// Sets current state based on buffer
    /**
     * Check to see if the buffer contains the same values
     */
    private void BufferPoseAnalyzer()
    {
        if(poseBuffer.Count > 0)
        {
            string poseState = poseBuffer[0];
            for (int i = 1; i < poseBuffer.Count; i++)
            {
                if (poseState != poseBuffer[i])
                {
                    currentPose = "";
                    return;
                }
            }
            currentPose = poseState;
        }
    }

    /// Adds current poses to buffer
    /**
     * Dumps buffer based on an interval value
     */
    private void BufferManager()
    {
        // dump current buffer:
        if (Time.time > nextTest)
        {
            BufferPoseAnalyzer();
            poseBuffer.Clear();
            nextTest = Time.time + interval;
        }
        poseBuffer.Add(CurrentPose());
    }

    /// Determine the pose the hand is closest to being in
    /**
     */
    private string CurrentPose()
    {
        string poseKey = "";
        float min = float.PositiveInfinity;
        foreach (KeyValuePair<string, float[]> pose in poses)
        {
            float[] currentPose = pose.Value;
            float[] fingerPositions = GetFingerPositions();
            float distance = PoseDistance(fingerPositions, currentPose);
            
            if(distance < min)
            {
                min = distance;
                poseKey = pose.Key;
            }
        }
        return poseKey;
    }

    /// Returns current normalized finger values
    private float[] GetFingerPositions()
    {
        float[] normalizedFingerValues = new float[5];
        if(fingerVals == null)
        {
            return normalizedFingerValues;
        }
        for(int i=0; i<normalizedFingerValues.Length; i++)
        {
            float sum = (fingerVals[i].sensor1 * 127) + (fingerVals[i].sensor2 * 127);
            float mapValue = Mathf.InverseLerp(0, 254, sum);
            normalizedFingerValues[i] = mapValue;
        }
        return normalizedFingerValues;
    }

    /// Distance between one pose and another
    private float PoseDistance(float[] p1, float[] p2)
    {
        float thumb  = (p1[0] - p2[0]) * (p1[0] - p2[0]);
        float index  = (p1[1] - p2[1]) * (p1[1] - p2[1]);
        float middle = (p1[2] - p2[2]) * (p1[2] - p2[2]);
        float ring   = (p1[3] - p2[3]) * (p1[3] - p2[3]);
        float pinky  = (p1[4] - p2[4]) * (p1[4] - p2[4]);
        float distance = Mathf.Sqrt((thumb + index + middle + ring + pinky));
        return distance;
    }

    /// Sets up default pose values
    private void SetupPoseDefaultLibrary()
    {
        float[] flat = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        poses["flat"] = flat;
        float[] grab = { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
        poses["grab"] = grab;
        float[] devilHorns = { 1.0f, 0.0f, 1.0f, 1.0f, 0.0f };
        poses["devil_horns"] = devilHorns;
        float[] iLoveYou = { 0.0f, 0.0f, 1.0f, 1.0f, 0.0f };
        poses["i_love_you"] = iLoveYou;
        float[] peaceSign = { 1.0f, 0.0f, 0.0f, 1.0f, 1.0f };
        poses["peace"] = peaceSign;
        float[] hangLoose = { 0.2368421f, 1.0f, 0.9612403f, 0.9850746f, 0.2180451f };
        poses["hang_loose"] = hangLoose;
        float[] gun = { 0.0f, 0.0f, 1.0f, 1.0f, 1.0f };
        poses["gun"] = gun;
        float[] okay = { 1.0f, 0.5963303f, 0.0f, 0.02985075f, 0.2631579f };
        poses["okay"] = okay;
        float[] indexPoint = { 1.0f, 0.0f, 1.0f, 1.0f, 1.0f };
        poses["index_point"] = indexPoint;
        float[] thumbsUp = { 0.2105263f, 0.8440367f, 0.9302326f, 0.9328358f, 0.8270677f };
        poses["thumbs_up"] = thumbsUp;
        float[] middlePoint = { 1.0f, 1.0f, 0.0f, 1.0f, 1.0f };
        poses["middle_point"] = middlePoint;
        float[] pinkyPoint = { 1.0f, 1.0f, 1.0f, 1.0f, 0.0f };
        poses["pinky_point"] = pinkyPoint;
    }

    //---------------------------------------------------------------------------------
    // Template Pose Functions
    //---------------------------------------------------------------------------------

    /// Flat pose
    public bool FlatPose()
    {
        return (currentPose == "flat") ? true : false; 
    }

    /// Grab pose
    public bool GrabPose()
    {
        return (currentPose == "grab") ? true : false;
    }

    /// Devil Horns Pose
    public bool DevilHornsPose()
    {
        return (currentPose == "devil_horns") ? true : false;
    }

    /// I Love You Pose
    public bool ILoveYouPose()
    {
        return (currentPose == "i_love_you") ? true : false;
    }

    /// Peace Sign Pose
    public bool PeaceSignPose()
    {
        return (currentPose == "peace") ? true : false;
    }

    /// Hang Loose Pose
    public bool HangLoosePose()
    {
        return (currentPose == "hang_loose") ? true : false;
    }

    /// Gun Pose
    public bool GunPose()
    {
        return (currentPose == "gun") ? true : false;
    }

    /// Okay Pose
    public bool OkayPose()
    {
        return (currentPose == "okay") ? true : false;
    }

    /// Thumbs Up
    public bool ThumbsUp()
    {
        return (currentPose == "thumbs_up") ? true : false;
    }

    /// Index Point Pose
    public bool IndexPointPose()
    {
        return (currentPose == "index_point") ? true : false;
    }

    /// Middle Point Pose
    public bool MiddlePointPose()
    {
        return (currentPose == "middle_point") ? true : false;
    }

    /// Pinky Point Pose
    public bool PinkyPointPose()
    {
        return (currentPose == "pinky_point") ? true : false;
    }

    //---------------------------------------------------------------------------------
    // Getters
    //---------------------------------------------------------------------------------

    /// returns a string with the given pose name 
    /**
     */
    public string GetCurrentPoseName()
    {
        return currentPose;
    }

    /// returns the normalized values of the current pose
    /**
     */
    public float[] GetCurrentPoseValues()
    {
        return GetFingerPositions();
    }

    //---------------------------------------------------------------------------------
    // Setters
    //---------------------------------------------------------------------------------

    /// Sets the interval
    /**
     */
    public void SetInterval(float val)
    {
        interval = val;
    }

    //---------------------------------------------------------------------------------
    // Debug
    //---------------------------------------------------------------------------------

    /// Prints out the current pose values
    /**
     */
    public void DebugPose()
    {
        float[] normalizedFingerValues = GetFingerPositions();
        string debugInfo = "";
        for (int i = 0; i < normalizedFingerValues.Length; i++)
        {
            debugInfo += normalizedFingerValues[i] + " ";
        }
        Debug.Log(debugInfo);
    }

}
