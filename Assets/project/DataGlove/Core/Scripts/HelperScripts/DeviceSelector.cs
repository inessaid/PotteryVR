using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceSelector : MonoBehaviour
{
    public bool isRightHand = true;

    [HideInInspector]
    public bool complete = false;

    private GloveController glove = null;
    private Dropdown dropdown;
    private Button connectionButton;
    private RawImage connectionImage;
    private List<string> prevNames = new List<string>();
    private Forte_GloveManager gloveManager;

    // Start is called before the first frame update
    void Start()
    {
        gloveManager = Forte_GloveManager.Instance;
        dropdown = GetComponentInChildren<Dropdown>();
        connectionButton = GetComponentInChildren<Button>();
        connectionImage = GetComponentInChildren<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (glove == null)
        {
            if (isRightHand && gloveManager.rightGlove != null)
            {
                glove = gloveManager.rightGlove;
            }
            else if (!isRightHand && gloveManager.leftGlove != null)
            {
                glove = gloveManager.leftGlove;
            }
        }

        if (glove != null)
        {
            List<string> names = new List<string>(glove.GetAvailableBluetoothDeviceScanNames());
            HashSet<string> set = new HashSet<string>(names);
            if (!set.SetEquals(prevNames))
            {
                Debug.Log("update name options");
                dropdown.ClearOptions();
                dropdown.AddOptions(names);
                prevNames = new List<string>(names);
            }

            if (glove.IsConnected())
            {
                if (connectionImage.color.Equals(Color.red))
                {
                    connectionImage.color = Color.green;
                    connectionButton.interactable = false;
                    dropdown.interactable = false;
                    complete = true;
                }
            }
        }
    }

    public void ConnectToSelectedBluetoothDevice()
    {
        if(dropdown.options.Count == 0)
        {
            Debug.Log("List is empty");
            return;
        }
        if(glove.IsConnected())
        {
            Debug.Log("Glove is already connected");
            return;
        }
        string deviceName = dropdown.options[dropdown.value].text;
        glove.ConnectToBluetoothDeviceByName(deviceName);
    }
}
