using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forte_Armature
{
    private const int NUM_OF_FINGERS = 5;

    public GameObject[] knuckleJoints = new GameObject[NUM_OF_FINGERS];
    public GameObject[] middleJoints = new GameObject[NUM_OF_FINGERS];
    public GameObject[] tipJoints = new GameObject[NUM_OF_FINGERS];
    public GameObject wrist = null;
    public GameObject elbow = null;

    public void LoadDynamically(GameObject root)
    {
        Transform[] allChildren = root.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            string lowercaseChild = child.gameObject.name.ToLower();
            GameObject[] fingerSegment = null;
            int fingerIndex = -1;

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

            if (lowercaseChild.Contains("0"))
            {
                fingerSegment = knuckleJoints;
            }
            else if (lowercaseChild.Contains("1"))
            {
                fingerSegment = middleJoints;
            }
            else if (lowercaseChild.Contains("2"))
            {
                fingerSegment = tipJoints;
            }

            if (fingerIndex != -1 && fingerSegment != null)
            {
                fingerSegment[fingerIndex] = child.gameObject;
            }
            
            if(lowercaseChild.Contains("root"))
            {
                wrist = child.gameObject;
                elbow = child.gameObject;
            }

        }
    }
}
