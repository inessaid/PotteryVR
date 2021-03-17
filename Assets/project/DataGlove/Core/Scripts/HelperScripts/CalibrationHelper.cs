#if !UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Manages UI for the calibration helper prefab
/** 
 */
public class CalibrationHelper : MonoBehaviour {

    GameObject popup;
    GameObject buttonObject;
    GameObject handImageObject;
    Button popupButton;
    Text text;
    CanvasGroup cg;
    RawImage rw;

    public Texture flat;
    public Texture knuckles;
    public Texture thumb;

    private float fade = 0.99f;
    private float fadeSpeed = 0.004f;
    private bool buttonClicked = false;
    private bool calibrationComplete = false;
    private string startText = "First, point your fingers forward with palm facing down and press the 'H' key.";

    /// Use this for initialization
    void Start () {
        popup = GameObject.Find("PopUp");

        buttonObject = GameObject.Find("PopUpButton");
        popupButton = buttonObject.GetComponent<Button>();

        handImageObject = GameObject.Find("HandImage");
        rw = handImageObject.GetComponent<RawImage>();

        text = popup.GetComponentInChildren<Text>();
        text.text = startText;
        cg = popup.GetComponent<CanvasGroup>();
    }
	
	/// Update is called once per frame
	void Update () {

        ProcessInput();

        // button will fade in/out until clicked
        if (!buttonClicked)
        {
            if (fade <= 0.4f || fade >= 1.0f)
            {
                fadeSpeed *= -1;
            }
            var color = popupButton.targetGraphic.color;
            color.a = fade;
            popupButton.targetGraphic.color = color;
            fade -= fadeSpeed;
        }

        // calibration is done, destroy gameObject
        if (calibrationComplete)
        {
            Invoke("Reset", 1.5f);
        }

    }

    /// button uses this to turn on pop-up
    public void TurnOnPopUp()
    {
        cg.alpha = 1.0f;
        buttonObject.SetActive(false);
        buttonClicked = true;
        text.text = startText;
        rw.color = new Color(1, 1, 1, 0);
    }

    /// goes through calibration steps
    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            rw.color = Color.white;
            rw.texture = flat;
            text.text = "Very good, now flatten your hand and press the 'F' key.";
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            rw.texture = knuckles;
            text.text = "Next, curl your knuckles in at 90 degree angles and press the 'K' key.";
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            rw.texture = thumb;
            text.text = "Now open your hand and bend your thumb into your palm and press the 'T' key.";
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            rw.color = new Color(1, 1, 1, 0);
            text.text = "Calibration is complete.";
            calibrationComplete = true;
        }
    }

    /// resets variables for another iteration of calibration help (if needed)
    /** 
     */
    private void Reset()
    {
        cg.alpha = 0.0f;
        buttonClicked = false;
        calibrationComplete = false;
        buttonObject.SetActive(true);
        text.text = startText;
    }

}

#else

using UnityEngine;

public class CalibrationHelper : MonoBehaviour {
    void Start () {
        Destroy(gameObject);
	}
}

#endif
