using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataGlove;

public class GlovePair : MonoBehaviour
{
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;
    private List<GloveController> gloves = new List<GloveController>();
    private List<HapticController> haptics = new List<HapticController>();
    private List<DataGloveIO> ios = new List<DataGloveIO>(); 

    // Start is called before the first frame update
    void OnEnable()
    {
        GloveController left = leftHandPrefab.GetComponentInChildren<GloveController>();
        GloveController right = rightHandPrefab.GetComponentInChildren<GloveController>();

        if (left.GetDataGlove().IsConnected())
        {
            gloves.Add(left);
            haptics.Add(left.gameObject.GetComponent<HapticController>());
            ios.Add(left.GetDataGlove());
        }
        else
        {
            Destroy(leftHandPrefab);
        }
        if (right.GetDataGlove().IsConnected())
        {
            gloves.Add(right);
            haptics.Add(right.gameObject.GetComponent<HapticController>());
            ios.Add(right.GetDataGlove());
        }
        else
        {
            Destroy(rightHandPrefab);
        }

    }

    public List<GloveController> GetGloves()
    {
        return gloves;
    }

    public List<HapticController> GetHaptics()
    {
        return haptics;
    }

    public List<DataGloveIO> GetDataGloveIOs()
    {
        return ios;
    }
}
