using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Handles the UI for Quaternion and Sensor data in the HapticsTester scene
/** 
 */
public class GloveData : MonoBehaviour {

    private const int NUM_OF_FINGERS = 5;

    private List<GloveController> gloves;
    private Text imuText;
    private GameObject sensorValues;
    private Text[,] fingerTexts = new Text[2, 5];
    private string[] fingerNames = { "thumb ", "index  ", "middle", "ring     ", "pinky   " };
    private string space = "                        ";

    public Quaternion imuQuat;
    public GameObject textPrefab;

    /// Use this for initialization
    void Start () {
        GlovePair pair = FindObjectOfType<GlovePair>();
        gloves = pair.GetGloves();
        sensorValues = GameObject.Find("sensorValues");

        // setup IMU text object:
        imuText = GameObject.Find("imuText").GetComponent<Text>();

        // setup finger text object
        for (int k = 0; k < gloves.Count; k++)
        {
            Text header = CreateTextPrefab(Color.white, sensorValues.transform);
            header.text = space + "knuckle1" + space + "knuckle2";
            header.fontSize = 12;
            for (int i = 0; i < NUM_OF_FINGERS; i++)
            {
                fingerTexts[k, i] = CreateTextPrefab(Color.white, sensorValues.transform);
                fingerTexts[k, i].fontSize = 12;
            }
        }
    }

    /** 
     * Updates quat and sensor values
     */
    void Update () {
        imuText.text = "";
        for (int i = 0; i < gloves.Count; i++)
        {
            for (int j = 0; j < NUM_OF_FINGERS; j++)
            {
                fingerTexts[i, j].text = "";
            }
        }

        for (int i = 0; i < gloves.Count; i++)
        {
            // update imu text
            imuQuat = gloves[i].GetDataGlove().GetIMU();
            imuText.text += "IMU Quaternion:\nw : " + (int)imuQuat.w + "\nx : " + (int)imuQuat.x + "\ny : " + (int)imuQuat.y + "\nz : " + (int)imuQuat.z + "\n\n\n\n";

            // update finger sensor text
            for (int j = 0; j < NUM_OF_FINGERS; j++)
            {
                float sensor1 = gloves[i].GetDataGlove().GetSensors()[j].sensor1 * 127;
                float sensor2 = gloves[i].GetDataGlove().GetSensors()[j].sensor2 * 127;
                fingerTexts[i, j].text += fingerNames[j] + space + sensor1 + space + sensor2;
            }
        }
 
	}

    /// Creates a new text prefab
    private Text CreateTextPrefab(Color color, Transform parent)
    {
        Text text = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, parent).GetComponent<Text>();
        text.color = color;
        return text;
    }

}
