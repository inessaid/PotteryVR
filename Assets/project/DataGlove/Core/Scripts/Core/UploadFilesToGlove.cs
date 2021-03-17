using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataGlove;

// demonstrates how to upload .wav files to the glove on start
public class UploadFilesToGlove : MonoBehaviour
{
    public AudioClip[] uploadSlots = new AudioClip[13];

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            sendHaptics();
        }
    }



    void sendHaptics()
    {
        Debug.Log("Sending Haptics");

        DataGloveIO[] gloves = FindObjectsOfType<DataGloveIO>();

        for (int i = 0; i < gloves.Length; i++)
        {
            for (int j = 0; j < uploadSlots.Length; j++)
            {
                AudioClip clip = uploadSlots[j];
                if (clip != null)
                {
#if UNITY_EDITOR
                    string path = Application.dataPath + AssetDatabase.GetAssetPath(clip).Replace("Assets", "");
                    Debug.Log("Uploading: " + path);
                    Debug.Log("Success: " + gloves[i].UploadFile(path, j));
#endif
                }
            }
        }
    }

}
