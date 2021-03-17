using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Adds a Data Glove battery status icon on the top right of the screen
/**
 */
public class Battery : MonoBehaviour
{
    public Texture2D batteryIcon;
    public Texture2D batteryIconFillWhite;
    public Texture2D batteryIconFillYellow;
    public Texture2D batteryIconFillRed;
    
    private GloveController glove;
    private Text uiText;
    private RawImage batteryIconFill;

    // Start is called before the first frame update
    void Start()
    {
        glove = FindObjectOfType<GloveController>();
        uiText = GetComponent<Text>();
        batteryIconFill = GetComponentInChildren<RawImage>();
        batteryIconFill.texture = batteryIconFillWhite;
    }

    // Update is called once per frame
    void Update()
    {
        float currentPower = glove.GetDataGlove().GetBatteryStatus();
        uiText.text = (int)(currentPower * 100) + "%";

        if (currentPower < 0.2f)
        {
            batteryIconFill.texture = batteryIconFillRed;
        }
        else if (currentPower < 0.7f)
        {
            batteryIconFill.texture = batteryIconFillYellow;
        }
        else
        {
            batteryIconFill.texture = batteryIconFillWhite;
        }
    }
}
