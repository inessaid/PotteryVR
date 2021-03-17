using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataGlove;

public class ASLtext : MonoBehaviour
{
    Text txt;
    private string currLetter;
    private string currPose;
    private string currNumber;
    private float currPosePercent;
    private float currLetterPercent;
    private float currNumberPercent;
    private float[] currentPose;
    private float rate;
    private int checkPose = 0;
    private bool handLoaded = false;

    private ASLMenu button;
    private GloveController dataGlove;
    private GestureRecognition gesture;
    private Poses poses;

    private GameObject highlight_None;
    private GameObject highlight_A;
    private GameObject highlight_B;
    private GameObject highlight_C;
    private GameObject highlight_D;
    private GameObject highlight_E;
    private GameObject highlight_F;
    private GameObject highlight_G;
    private GameObject highlight_H;
    private GameObject highlight_I;
    private GameObject highlight_J;
    private GameObject highlight_K;
    private GameObject highlight_L;
    private GameObject highlight_M;
    private GameObject highlight_N;
    private GameObject highlight_O;
    private GameObject highlight_P;
    private GameObject highlight_Q;
    private GameObject highlight_R;
    private GameObject highlight_S;
    private GameObject highlight_T;
    private GameObject highlight_U;
    private GameObject highlight_V;
    private GameObject highlight_W;
    private GameObject highlight_X;
    private GameObject highlight_Y;
    private GameObject highlight_Z;
    // Start is called before the first frame update
    void Start()
    {
        txt = gameObject.GetComponent<Text>();
        txt.text = "Current Pose: \nCurrent Letter \nCurrent Number";

        rate = 10f;
        button = GameObject.FindGameObjectWithTag("ASLMenu").GetComponent<ASLMenu>();

        highlight_None = GameObject.Find("highlight_None");
        highlight_A = GameObject.Find("highlight_A");
        highlight_B = GameObject.Find("highlight_B");
        highlight_C = GameObject.Find("highlight_C");
        highlight_D = GameObject.Find("highlight_D");
        highlight_E = GameObject.Find("highlight_E");
        highlight_F = GameObject.Find("highlight_F");
        highlight_G = GameObject.Find("highlight_G");
        highlight_H = GameObject.Find("highlight_H");
        highlight_I = GameObject.Find("highlight_I");
        highlight_J = GameObject.Find("highlight_J");
        highlight_K = GameObject.Find("highlight_K");
        highlight_L = GameObject.Find("highlight_L");
        highlight_M = GameObject.Find("highlight_M");
        highlight_N = GameObject.Find("highlight_N");
        highlight_O = GameObject.Find("highlight_O");
        highlight_P = GameObject.Find("highlight_P");
        highlight_Q = GameObject.Find("highlight_Q");
        highlight_R = GameObject.Find("highlight_R");
        highlight_S = GameObject.Find("highlight_S");
        highlight_T = GameObject.Find("highlight_T");
        highlight_U = GameObject.Find("highlight_U");
        highlight_V = GameObject.Find("highlight_V");
        highlight_W = GameObject.Find("highlight_W");
        highlight_X = GameObject.Find("highlight_X");
        highlight_Y = GameObject.Find("highlight_Y");
        highlight_Z = GameObject.Find("highlight_Z");
    }

    // Update is called once per frame
    void Update()
    {
        if (!handLoaded && Forte_GloveManager.Instance.rightGlove != null)
        {
            dataGlove = Forte_GloveManager.Instance.rightGlove;
            gesture = dataGlove.GetComponent<GestureRecognition>();
            poses = dataGlove.GetComponent<Poses>();
            handLoaded = true;
        }

        if (!handLoaded)
        {
            return;
        }

        currLetter = gesture.GetCurrentLetter();
        currPose = gesture.GetCurrentGesture();
        currNumber = gesture.GetCurrentNumber();
        currLetterPercent = gesture.GetCurrentLetterPercent();
        currPosePercent = gesture.GetCurrentGesturePercent();
        currNumberPercent = gesture.GetCurrentNumberPercent();
        txt.text = "Current Letter: " + currLetter + " " + currLetterPercent.ToString() + "\n" +
                   "Current Number: " + currNumber + " " + currNumberPercent.ToString();
        highlight_A.gameObject.SetActive(currLetter == "letter_A");
        highlight_B.gameObject.SetActive(currLetter == "letter_B");
        highlight_C.gameObject.SetActive(currLetter == "letter_C");
        highlight_D.gameObject.SetActive(currLetter == "letter_D");
        highlight_E.gameObject.SetActive(currLetter == "letter_E");
        highlight_F.gameObject.SetActive(currLetter == "letter_F");
        highlight_G.gameObject.SetActive(currLetter == "letter_G");
        highlight_H.gameObject.SetActive(currLetter == "letter_H");
        highlight_I.gameObject.SetActive(currLetter == "letter_I");
        highlight_J.gameObject.SetActive(currLetter == "letter_J");
        highlight_K.gameObject.SetActive(currLetter == "letter_K");
        highlight_L.gameObject.SetActive(currLetter == "letter_L");
        highlight_M.gameObject.SetActive(currLetter == "letter_M");
        highlight_N.gameObject.SetActive(currLetter == "letter_N");
        highlight_O.gameObject.SetActive(currLetter == "letter_O");
        highlight_P.gameObject.SetActive(currLetter == "letter_P");
        highlight_Q.gameObject.SetActive(currLetter == "letter_Q");
        highlight_R.gameObject.SetActive(currLetter == "letter_R");
        highlight_S.gameObject.SetActive(currLetter == "letter_S");
        highlight_T.gameObject.SetActive(currLetter == "letter_T");
        highlight_U.gameObject.SetActive(currLetter == "letter_U");
        highlight_V.gameObject.SetActive(currLetter == "letter_V");
        highlight_W.gameObject.SetActive(currLetter == "letter_W");
        highlight_X.gameObject.SetActive(currLetter == "letter_X");
        highlight_Y.gameObject.SetActive(currLetter == "letter_Y");
        highlight_Z.gameObject.SetActive(currLetter == "letter_Z");
    }
}
