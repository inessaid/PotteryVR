using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataGlove;

/// Updates battery status prefab
/** 
 */
public class BatteryStatus : MonoBehaviour
{
    public Texture2D batteryIconEmpty;
    public Texture2D batteryIconFillWhite;
    public Texture2D batteryIconFillYellow;
    public Texture2D batteryIconFillRed;
    public Texture2D batteryIconDisabled;
    public Texture2D BTIcon;
    public Texture2D USBIcon;
    public Texture2D DongleIcon;
    Texture2D batteryIconLeft;
    Texture2D batteryIconRight;
    Texture2D batteryIconFillLeft;
    Texture2D batteryIconFillRight;
    Texture2D connIconLeft;
    Texture2D connIconRight;
    float currentPowerLeft = -1;
    float currentPowerRight = -1;
    float sizeDivisor = (5000 - Screen.width) * .00123f;
    float timeSinceUpdate = 10;
    float battDisplayHeight;
    float battDisplayWidth;
    int leftIndex = -1;
    int rightIndex = -1;
    bool isConnectedLeft = false;
    bool isConnectedRight = false;
    int topOfScreenY = 10;

    // data glove
    private List<DataGloveIO> dataGloves;
    private GloveController[] controllers;

    /// use this for intialization
    private void Start()
    {
        controllers = FindObjectsOfType<GloveController>();

        battDisplayWidth = batteryIconEmpty.width / sizeDivisor;
        battDisplayHeight = batteryIconEmpty.height / sizeDivisor;

        batteryIconLeft = batteryIconDisabled;
        batteryIconRight = batteryIconDisabled;

        batteryIconFillLeft = batteryIconFillWhite;
        batteryIconFillRight = batteryIconFillWhite;
    }

    /// switch battery icons
    /** 
     */
    private void Update()
    {
        if (dataGloves == null)
        {
            dataGloves = new List<DataGloveIO>();

            for (int i = 0; i < controllers.Length; i++)
            {
                dataGloves.Add(controllers[i].GetDataGlove());
                if (controllers[i].rightHandGlove == true)
                {
                    rightIndex = i;
                }
                else
                {
                    leftIndex = i;
                }
            }
        }

        timeSinceUpdate += Time.deltaTime;

        if (timeSinceUpdate > 2)
        {
            timeSinceUpdate = 0;
            UpdateLeftBattery(leftIndex);
            UpdateRightBattery(rightIndex);
        }
    }

    void UpdateLeftBattery(int index)
    {
        if (index < 0)
        {
            return;
        }

        currentPowerLeft = dataGloves[index].GetBatteryStatus();
        batteryIconFillLeft = GetFillColor(currentPowerLeft);
        connIconLeft = GetConnectionIcon(index);

        isConnectedLeft = (connIconLeft != null);
    }

    void UpdateRightBattery(int index)
    {
        if (index < 0)
        {
            return;
        }

        currentPowerRight = dataGloves[index].GetBatteryStatus();
        batteryIconFillRight = GetFillColor(currentPowerRight);
        connIconRight = GetConnectionIcon(index);

        isConnectedRight = (connIconRight != null);
    }

    Texture2D GetFillColor(float batteryVal)
    {
        if (batteryVal < 0.2f)
        {
            return batteryIconFillRed;
        }
        else if (batteryVal < 0.6f)
        {
            return batteryIconFillYellow;
        }
        else
        {
            return batteryIconFillWhite;
        }
    }

    Texture2D GetConnectionIcon(int index)
    {
        Texture2D connIcon = null;

        switch (dataGloves[index].GetGloveConnectionState())
        {
            case "USB":
                connIcon = USBIcon;
                break;
            case "Bluetooth":
                connIcon = BTIcon;
                break;
            case "Dongle Bluetooth":
                connIcon = DongleIcon;
                break;
            default:
                // None
                break;
        }

        return connIcon;
    }

    /// display battery status UI
    /** 
     */
    private void OnGUI()
    {
        DrawBattery(true);
        DrawBattery(false);
        DrawConnIndicator(true);
        DrawConnIndicator(false);
    }

    void DrawBattery(bool isLeft)
    {
        float xPos;
        float width = battDisplayWidth;
        float height = battDisplayHeight;
        float scaledPower;
        Texture2D icon;
        Texture2D backgroundIcon;

        if (isLeft)
        {
            xPos = 10;
            scaledPower = Scale(currentPowerLeft, 0, 1, 0, 100);
            icon = batteryIconFillLeft;

            if (isConnectedLeft)
            {
                backgroundIcon = batteryIconEmpty;
            }
            else
            {
                backgroundIcon = batteryIconDisabled;
                icon = batteryIconEmpty;
            } 
        }
        else
        {
            xPos = Screen.width - (width * 2) - 20;
            scaledPower = Scale(currentPowerRight, 0, 1, 0, 100);
            icon = batteryIconFillRight;

            if (isConnectedRight)
            {
                backgroundIcon = batteryIconEmpty;
            }
            else
            {
                backgroundIcon = batteryIconDisabled;
                icon = batteryIconEmpty;
            }
        }

        Rect rectImage = new Rect(xPos, topOfScreenY, width, height);
        Rect rectImageFill = new Rect(0, 0, width, height);
        Rect rectMask = new Rect(xPos, topOfScreenY, width, height);
        Rect percentRect = new Rect(Screen.width - 50, topOfScreenY, width, height);

        rectMask.width *= (scaledPower / 100.0f);

        GUI.BeginGroup(rectMask);
        GUI.DrawTexture(rectImageFill, icon);
        GUI.EndGroup();
        GUI.DrawTexture(rectImage, backgroundIcon);
    }

    void DrawConnIndicator(bool isLeft)
    {
        float xPos;
        float width = battDisplayWidth; // The BT and USB icons have the same dimensions
        float height = battDisplayHeight;
        Texture2D icon;

        if (isLeft)
        {
            xPos = width + 20;
            icon = connIconLeft;

            if (!isConnectedLeft)
            {
                return;
            }
        }
        else
        {
            xPos = Screen.width - width - 10;
            icon = connIconRight;

            if (!isConnectedRight)
            {
                return;
            }
        }

        Rect rectImage = new Rect(xPos, topOfScreenY, width, height);
        GUI.DrawTexture(rectImage, icon);
    }

    // Scales from the input range to an output range.
    float Scale(float value, float inMin, float inMax, float outMin, float outMax)
    {
        float scaled = outMin + (float)(value - inMin) / (inMax - inMin) * (outMax - outMin);
        return scaled;
    }
}
