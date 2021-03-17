using UnityEngine;

public class SelectorManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // The selection menu is only for selecting a Data Glove
        // from a list of unpaired Data Gloves on Windows. On Android/Quest, 
        // the user must manually add the Data Glove through the Quest 
        // Bluetooth device menu.
        if (Application.platform == RuntimePlatform.Android)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DeviceSelector[] selectors = GetComponentsInChildren<DeviceSelector>();
        bool finishedSelectingGloves = true;
        foreach (DeviceSelector selector in selectors)
        {
            if (!selector.complete)
            {
                finishedSelectingGloves = false;
            }
        }
        if (finishedSelectingGloves)
        {
            gameObject.SetActive(false);
        }
    }

    public void Dismiss()
    {
        gameObject.SetActive(false);
    }
}