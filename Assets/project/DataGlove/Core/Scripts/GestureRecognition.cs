using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataGlove;
using System;

public class GestureRecognition : MonoBehaviour
{
    Hashtable gestures = new Hashtable();
    Hashtable alphabet = new Hashtable();
    Hashtable numbers = new Hashtable();
    private string currGesture;
    private string currLetter;
    private string currNumber;
    private float currGesturePercent;
    private float currLetterPercent;
    private float currNumberPercent;

    public float swipeThreshold = 800f;
    private float rotationalVelocityMagnitude;
    private float[] fingerValsFloat;
    private Quaternion rotationalVelocity;
    private FingerVals[] fingerVals;

    float[] flat = { 0f, 0f, 0f, 0f, 0f };
    float[] thumbsUp = { 0f, 1f, 1f, 1f, 1f };
    float[] pointing = { .6f, 0f, 1f, 1f, 1f };
    float[] gun = { 0f, 0f, 1f, 1f, 1f };
    float[] peace = { 1f, 0f, 0f, 1f, 1f };
    float[] threeW = { 1f, 0f, 0f, 0f, 1f };
    float[] four = { 1f, 0f, 0f, 0f, 0f };
    float[] pinch = { 1f, 1f, 0f, 0f, 0f };
    float[] fist = { .9f, .9f, .9f, .9f, .9f };
    float[] hangTen = { 0f, 1f, 1f, 1f, 0f };

    float[] letter_A = { 0f, 1f, 1f, 1f, 1f, 1f };
    float[] letter_B = { 0.5f, 0f, 0f, 0f, 0f, 0.2f };
    float[] letter_C = { 0.2f, 0.3f, 0.3f, 0.3f, 0.3f, 0.2f };
    float[] letter_D = { 0.4f, 0f, 0.7f, 0.7f, 0.7f, 0.2f };
    float[] letter_E = { 0.55f, 0.7f, 0.7f, 0.7f, 0.7f, 0.2f };
    float[] letter_F = { 0.4f, 0.4f, 0f, 0f, 0f, 0.2f };
    float[] letter_G = { 0f, 0f, 1f, 1f, 1f, 0.8f };
    float[] letter_H = { 0f, 0f, 0f, 1f, 1f, 0.8f };
    float[] letter_I = { 0.35f, 0.8f, 0.8f, 0.7f, 0f, 0.2f };
    float[] letter_K = { 0.45f, 0f, 0f, 1f, 1f, 0.2f };
    float[] letter_L = { 0f, 0f, 1f, 1f, 1f, 0.2f };
    float[] letter_P = { 0.3f, 0f, 0f, 0.4f, 0.4f, 0.5f };
    float[] letter_Q = { 0.15f, 0f, 0.4f, 0.4f, 0.4f, 0.5f };
    float[] letter_U = { 0.7f, 0f, 0f, 1f, 1f, 0.2f };
    float[] letter_W = { 0.4f, 0f, 0f, 0f, 0.5f, 0.2f };
    float[] letter_X = { 0.4f, 0.2f, 1f, 1f, 1f, 0.2f };
    float[] letter_Y = { 0f, 1f, 1f, 0.9f, 0f, 0f };

    float[] number_0 = { 0f, 0f, 0f, 0f, 0f, 0f };
    float[] number_1 = { 0.3f, 0f, 0.5f, 0.5f, 0.5f, 0.25f };
    float[] number_2 = { 0.3f, 0f, 0f, 0.5f, 0.5f, 0.25f };
    float[] number_3 = { 0.15f, 0f, 0f, 0.5f, 0.5f, 0.25f };
    float[] number_4 = { 0.4f, 0f, 0f, 0f, 0f, 0.25f };
    float[] number_5 = { 0f, 0f, 0f, 0f, 0f, 0.25f };
    float[] number_6 = { 0.3f, 0f, 0f, 0f, 0.5f, 0.25f };
    float[] number_7 = { 0.3f, 0f, 0f, 0.5f, 0f, 0.25f };
    float[] number_8 = { 0.3f, 0f, 0.5f, 0f, 0f, 0.25f };
    float[] number_9 = { 0.3f, 0.4f, 0f, 0f, 0f, 0.25f };

    GloveController dataGlove;

    // Start is called before the first frame update
    void Awake()
    {
        // GESTURE INITIALIZATION
        gestures.Add("Flat", flat);
        gestures.Add("Thumbs_Up", thumbsUp);
        gestures.Add("Pointing", pointing);
        gestures.Add("Gun", gun);
        gestures.Add("Peace", peace);
        gestures.Add("Three_W", threeW);
        gestures.Add("Four", four);
        gestures.Add("Pinch", pinch);
        gestures.Add("Fist", fist);
        gestures.Add("HangTen", hangTen);

        alphabet.Add("letter_A", letter_A);
        alphabet.Add("letter_B", letter_B);
        alphabet.Add("letter_C", letter_C);
        alphabet.Add("letter_D", letter_D);
        alphabet.Add("letter_E", letter_E);
        alphabet.Add("letter_F", letter_F);
        alphabet.Add("letter_G", letter_G);
        alphabet.Add("letter_H", letter_H);
        alphabet.Add("letter_I", letter_I);
        alphabet.Add("letter_K", letter_K);
        alphabet.Add("letter_L", letter_L);
        alphabet.Add("letter_P", letter_P);
        alphabet.Add("letter_Q", letter_Q);
        alphabet.Add("letter_U", letter_U);
        alphabet.Add("letter_W", letter_W);
        alphabet.Add("letter_X", letter_X);
        alphabet.Add("letter_Y", letter_Y);

        numbers.Add("number_0", number_0);
        numbers.Add("number_1", number_1);
        numbers.Add("number_2", number_2);
        numbers.Add("number_3", number_3);
        numbers.Add("number_4", number_4);
        numbers.Add("number_5", number_5);
        numbers.Add("number_6", number_6);
        numbers.Add("number_7", number_7);
        numbers.Add("number_8", number_8);
        numbers.Add("number_9", number_9);

        currGesture = "Flat";

        dataGlove = GetComponent<GloveController>();

    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetGesture();
        SetLetter();
        SetNumber();
        fingerValsFloat = getFingerFloatsFull(fingerVals);
        //GetIMUChange();  
    }

    private float GetEuclideanDistance(float[] first, float[] second)
    {
        return Mathf.Sqrt(
            Mathf.Pow((first[0] - second[0]), 2) +
            Mathf.Pow((first[1] - second[1]), 2) +
            Mathf.Pow((first[2] - second[2]), 2) +
            Mathf.Pow((first[3] - second[3]), 2) +
            Mathf.Pow((first[4] - second[4]), 2)
            );
    }

    private float GetEuclideanDistanceIMU(float[] first, float[] second)
    {
        return Mathf.Sqrt(
            Mathf.Pow((first[0] - second[0]), 2) +
            Mathf.Pow((first[1] - second[1]), 2) +
            Mathf.Pow((first[2] - second[2]), 2) +
            Mathf.Pow((first[3] - second[3]), 2) +
            Mathf.Pow((first[4] - second[4]), 2) +
            Mathf.Pow((first[5] - second[5]), 2)
            );
    }

    private float[] getFingerVals(FingerVals[] fingerVals)
    {
        float[] fingers = { 0f, 0f, 0f, 0f, 0f };
        for (int i = 0; i < 5; i++)
        {
            fingers[i] = (fingerVals[i].sensor1 + fingerVals[i].sensor2) / 2f;
        }
        return fingers;
    }

    private float[] getFingerFloatsFull(FingerVals[] fingerVals)
    {
        float[] fingers = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        for (int i = 0; i < 5; i++)
        {
            fingers[i*2] = fingerVals[i].sensor1;
            fingers[i*2+1] = fingerVals[i].sensor2;
        }
        return fingers;
    }

    // Compares the finger's positions with the closest gestures in gesture list
    private void SetGesture()
    {
        float min = Mathf.Infinity;
        string name = "None";
        fingerVals = dataGlove.GetSensors();
        float[] fingerValsFloat = getFingerVals(fingerVals);
        foreach (DictionaryEntry item in gestures)
        {
            float distance = GetEuclideanDistance(fingerValsFloat, (float[])item.Value);
            if (distance < min)
            {
                min = distance;
                name = item.Key.ToString();
            }
        }
        currGesture = name;
        currGesturePercent = min;
    }
   
    public string GetCurrentGesture()
    {
        return currGesture;
    }

    public float GetCurrentGesturePercent()
    {
        return currGesturePercent;
    }

    private void SetLetter()
    {
        float min = Mathf.Infinity;
        string name = "None";
        fingerVals = dataGlove.GetSensors();
        float[] fingerValsFloat = getFingerVals(fingerVals);
        foreach (DictionaryEntry item in alphabet)
        {
            float distance = GetEuclideanDistance(fingerValsFloat, (float[])item.Value);
            if (distance < min)
            {
                min = distance;
                name = item.Key.ToString();
            }
        }
        currLetter = name;
        currLetterPercent = min;
    }

    public string GetCurrentLetter()
    {
        return currLetter;
    }

    public float GetCurrentLetterPercent()
    {
        return currLetterPercent;
    }

    private void SetNumber()
    {
        float min = Mathf.Infinity;
        string name = "None";
        fingerVals = dataGlove.GetSensors();
        float[] fingerValsFloat = getFingerVals(fingerVals);
        foreach (DictionaryEntry item in numbers)
        {
            float distance = GetEuclideanDistance(fingerValsFloat, (float[])item.Value);
            if (distance < min)
            {
                min = distance;
                name = item.Key.ToString();
            }
        }
        currNumber = name;
        currNumberPercent = min;
    }

    public string GetCurrentNumber()
    {
        return currNumber;
    }

    public float GetCurrentNumberPercent()
    {
        return currNumberPercent;
    }


    /*private void GetIMUChange()
    {
        rotationalVelocity = dataGlove.GetRotationalVelocity();
        rotationalVelocityMagnitude = Mathf.Sqrt(rotationalVelocity.x * rotationalVelocity.x +
                                                 rotationalVelocity.y * rotationalVelocity.y +
                                                 rotationalVelocity.z * rotationalVelocity.z +
                                                 rotationalVelocity.w * rotationalVelocity.w);
    }*/

    //--------------------------------------------------------------------------------------//

    private string CurrentPose()
    {
        string poseKey = "";
        float min = float.PositiveInfinity;
        foreach (DictionaryEntry pose in gestures)
        {
            float[] currentPose = (float[])pose.Value;
            float[] fingerPositions = GetFingerPositions();
            float distance = PoseDistance(fingerPositions, currentPose);

            if (distance < min)
            {
                min = distance;
                poseKey = pose.Key.ToString();
            }
        }
        return poseKey;
    }

    /// Returns current normalized finger values
    private float[] GetFingerPositions()
    {
        float[] normalizedFingerValues = new float[5];
        if (fingerVals == null)
        {
            return normalizedFingerValues;
        }
        for (int i = 0; i < normalizedFingerValues.Length; i++)
        {
            float sum = (fingerVals[i].sensor1 * 127) + (fingerVals[i].sensor2 * 127);
            float mapValue = Mathf.InverseLerp(0, 254, sum);
            normalizedFingerValues[i] = mapValue;
        }
        return normalizedFingerValues;
    }

    private float PoseDistance(float[] p1, float[] p2)
    {
        float thumb = (p1[0] - p2[0]) * (p1[0] - p2[0]);
        float index = (p1[1] - p2[1]) * (p1[1] - p2[1]);
        float middle = (p1[2] - p2[2]) * (p1[2] - p2[2]);
        float ring = (p1[3] - p2[3]) * (p1[3] - p2[3]);
        float pinky = (p1[4] - p2[4]) * (p1[4] - p2[4]);
        float distance = Mathf.Sqrt((thumb + index + middle + ring + pinky));
        return distance;
    }

    private float PoseDistanceFull(float[] p1, float[] p2)
    {
        float thumb = (p1[0] - p2[0]) * (p1[0] - p2[0]);
        float index = (p1[1] - p2[1]) * (p1[1] - p2[1]);
        float middle = (p1[2] - p2[2]) * (p1[2] - p2[2]);
        float ring = (p1[3] - p2[3]) * (p1[3] - p2[3]);
        float pinky = (p1[4] - p2[4]) * (p1[4] - p2[4]);
        float thumb2 = (p1[5] - p2[5]) * (p1[5] - p2[5]);
        float index2 = (p1[6] - p2[6]) * (p1[6] - p2[6]);
        float middle2 = (p1[7] - p2[7]) * (p1[7] - p2[7]);
        float ring2 = (p1[8] - p2[8]) * (p1[8] - p2[8]);
        float pinky2 = (p1[9] - p2[9]) * (p1[9] - p2[9]);
        float distance = Mathf.Sqrt((thumb + index + middle + 
            ring + pinky + thumb2 + index2 + middle2 + ring2 + pinky2));
        return distance;
    }

    float[] getFingerValsFloat()
    {
        return fingerValsFloat;
    }
}
