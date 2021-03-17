using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataGlove;

public class PinchDetection : MonoBehaviour
{
    public int maxCalibrationTime = 5;
    public RawImage background;
    public RawImage pinchIndex;
    public RawImage pinchMiddle;
    public RawImage pinchRing;
    public RawImage pinchPinky;
    public Text prompt;
    public Text countdown;
    public float minValue = .6f;
    private const int lastFrame = 3;
    private const string pinchIndexPrompt  = "Touch your index finger and thumb together while keeping your other fingers flat.";
    private const string pinchMiddlePrompt = "Touch your middle finger and thumb together while keeping your other fingers flat.";
    private const string pinchRingPrompt   = "Touch your ring finger and thumb together while keeping your other fingers flat.";
    private const string pinchPinkyPrompt  = "Touch your pinky finger and thumb together while keeping your other fingers flat.";
    private string[] prompts = { pinchIndexPrompt, pinchMiddlePrompt, pinchRingPrompt, pinchPinkyPrompt };
    private RawImage[] images = new RawImage[4];
    private GloveController[] gloves = new GloveController[] { null, null };
    private Forte_GloveManager gloveManager;
    private bool foundLeft = false;
    private bool foundRight = false;

    private float[] distance = new float[4];
    private bool calibrated = false;
    private float rate = 10f;
    

    // Start is called before the first frame update
    void Start()
    {
        gloveManager = Forte_GloveManager.Instance;
        images[0] = pinchIndex;
        images[1] = pinchMiddle;
        images[2] = pinchRing;
        images[3] = pinchPinky;
    }

    void Update()
    {
        if (!foundLeft && gloveManager.leftGlove != null)
        {
            gloves[0] = gloveManager.leftGlove;
            foundLeft = true;
        }

        if (!foundRight && gloveManager.rightGlove != null)
        {
            gloves[1] = gloveManager.rightGlove;
            foundRight = true;
        }

        if (calibrated)
        {
            for (int i = 0; i < gloves.Length; i++)
            {
                if (gloves[i] == null)
                {
                    continue;
                }

                distance = gloves[i].GetPinchDistance();

                EnterPinch(distance, gloves[i]);

                if (gloves[i].minPinch < minValue)
                {
                    Poses poses = gloves[i].GetComponent<Poses>();

                    for (int j =0; j < 4; j++)
                    {
                        if (gloves[i].inPinch[j])
                        { // Already in pinch
                          // If it's the same pinch pose we call update pose
                            gloves[i].stayPinching = true;
                            switch (j)
                            {
                                case (0):
                                    gloves[i].UpdatePose(poses.PinchPose(), rate);
                                    break;
                                case (1):
                                    gloves[i].UpdatePose(poses.PinchMiddlePose(), rate);
                                    break;
                                case (2):
                                    gloves[i].UpdatePose(poses.PinchRingPose(), rate);
                                    break;
                                case (3):
                                    gloves[i].UpdatePose(poses.PinchPinkyPose(), rate);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (!gloves[i].stayPinching)
                    {
                        int index = gloves[i].minIndex;

                        switch (index)
                        { // If a new pinch pose is detected we call enter pose
                            case (0):
                                gloves[i].EnterPose(poses.PinchPose(), rate);
                                UpdateArray(index, gloves[i]);
                                break;
                            case (1):
                                gloves[i].EnterPose(poses.PinchMiddlePose(), rate);
                                UpdateArray(index, gloves[i]);
                                break;
                            case (2):
                                gloves[i].EnterPose(poses.PinchRingPose(), rate);
                                UpdateArray(index, gloves[i]);
                                break;
                            case (3):
                                gloves[i].EnterPose(poses.PinchPinkyPose(), rate);
                                UpdateArray(index, gloves[i]);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                { // If there is no pinch pose call exit pose
                    gloves[i].stayPinching = false;
                    for (int j = 0; j < 4; j++)
                    {
                        if (gloves[i].inPinch[j])
                        {
                            gloves[i].ExitPose(rate);
                            UpdateArray(5, gloves[i]);
                            continue;
                        }
                    }
                }
            }
        }
    }
    public void HomeGloves()
    {
        for (int i = 0; i < gloves.Length; i++)
        {
            gloves[i].GetDataGlove().SetIMUHome();
        }
    }

    // Updates to next calibration prompt
    // Destroys object when calibration sequence is complete 
    private IEnumerator Sequence()
    {
        bool sequenceRunning = true;
        prompt.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
        countdown.gameObject.SetActive(true);
        int frame = 0;
        SetFrame(frame);
        int counter = maxCalibrationTime;
        countdown.text = counter.ToString();

        while (sequenceRunning)
        {
            yield return new WaitForSeconds(1.0f);
            counter--;
            countdown.text = counter.ToString();

            CalibrateFrame(frame);

            if (counter == 0)
            {
                if (frame == lastFrame)
                {
                    sequenceRunning = false;

                    images[frame].gameObject.SetActive(false);
                    prompt.gameObject.SetActive(false);
                    countdown.gameObject.SetActive(false);

                    break;
                }
                counter = maxCalibrationTime;
                countdown.text = counter.ToString();

                frame++;
                SetFrame(frame);
            }
        }
        background.gameObject.SetActive(false);
        calibrated = true;
    }

    public void StartCalibration()
    {
        calibrated = false;
        if (gloves.Length > 0)
        {
            StartCoroutine(Sequence());
        }
    }

    private void SetFrame(int newFrame)
    {
        if (newFrame < 0 || newFrame > lastFrame)
        {
            return;
        }

        images[newFrame].gameObject.SetActive(true);
        prompt.text = prompts[newFrame];

        if (newFrame > 0)
        {
            images[newFrame - 1].gameObject.SetActive(false);
        }

    }

    private void CalibrateFrame(int newFrame)
    {
        for (int i = 0; i < gloves.Length; i++)
        {
            GloveController glove = gloves[i];
            if (glove == null) { continue; }

            switch (newFrame)
            {
                case 0:
                    glove.SetIndexPinch();
                    break;
                case 1:
                    glove.SetMiddlePinch();
                    break;
                case 2:
                    glove.SetRingPinch();
                    break;
                case 3:
                    glove.SetPinkyPinch();
                    break;
            }
        }
    }

    // Updates the variables min and index to track which pinch the user is closest to
    private void EnterPinch(float[] distance, GloveController glove)
    {
        float tempMin = 1f;
        int tempIndex = -1;
        for(int i = 0; i < 4; i++)
        {
            if( distance[i] < tempMin)
            {
                tempMin = distance[i];
                tempIndex = i;
            }
        }
        glove.minPinch = tempMin;
        glove.minIndex = tempIndex;
    }

    // Updates the array 'inPinch' so that the index of the current pinch is
    // true and the rest are false.
    private void UpdateArray(int index, GloveController glove)
    {
        for(int i = 0; i < 4; i++)
        {
            if(i == index)
            {
                glove.inPinch[i] = true;
            }
            else
            {
                glove.inPinch[i] = false;
            }
        }
    }
}
