using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataGlove;

/// Manages device info for the DeviceInfo prefab
public class DeviceInfo : MonoBehaviour {

    public GameObject popup;
    public Text batteryText;
    public Text firmwareText;
    public Text hardwareText;
    public Text bootloaderText;
    public Text deviceIDText;
    
    /// can't get this info in start and battery needs to update in case of change
    void Update() {

        // Temporary solution for this prefab
        // Just get the device info for the 1st glove Unity finds
        GloveController[] gloves = FindObjectsOfType<GloveController>();
        for(int i=0; i<gloves.Length; i++)
        {
            DataGloveIO dg = gloves[i].GetDataGlove();
            if (dg.IsConnected())
            {
                batteryText.text = "Battery: " + dg.GetBatteryStatus() + " / 1";
                firmwareText.text = "Firmware Version: " + dg.GetFirmware();
                hardwareText.text = "Hardware Version: " + dg.GetHardwareVersion();
                bootloaderText.text = "Bootloader Version: " + dg.GetBootloader();
                deviceIDText.text = "Hardware Rev: " + dg.GetHardwareRev();
                break;
            }
        }
    }

    /// device info button uses this
    public void BringUpDeviceInfo()
    {
        popup.SetActive(true);
    }

    /// the close button on the pop-up uses this
    public void CloseDeviceInfo()
    {
        popup.SetActive(false);
    }

}
