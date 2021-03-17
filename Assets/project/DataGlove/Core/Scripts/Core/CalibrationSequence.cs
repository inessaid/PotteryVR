using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataGlove;

public class CalibrationSequence : MonoBehaviour
{
    public int maxCalibrationTime = 5;
    public RawImage flat;
    public RawImage fist;
    public RawImage thumb;
    public Text prompt;
    public Text countdown;

    private const int lastFrame = 2;
    private const string flatPrompt = "Hold hands flat, with palms down in front of you.";
    private const string fistPrompt = "Make a fist.";
    private const string thumbPrompt = "Hold hands flat with thumbs in.";
    private string[] prompts = { flatPrompt, fistPrompt, thumbPrompt };
    private RawImage[] images = new RawImage[3];
    private Forte_GloveManager gloveManager;

    // Start is called before the first frame update
    void Start()
    {
        gloveManager = Forte_GloveManager.Instance;

        images[0] = flat;
        images[1] = fist;
        images[2] = thumb;
    }

    // Updates to next calibration prompt
    // Destroys object when calibration sequence is complete 
    private IEnumerator Sequence()
    {
        bool sequenceRunning = true;
        prompt.gameObject.SetActive(true);
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

            CalibrateFrame(frame, gloveManager.leftGlove);
            CalibrateFrame(frame, gloveManager.rightGlove);

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
    }

    public void StartCalibration()
    {
        if (gloveManager.leftGlove != null || gloveManager.rightGlove != null)
        {
            StartCoroutine(Sequence());
        }
    }

    private void SetFrame(int newFrame)
    {
        if (newFrame < 0 || newFrame > 2)
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

    private void CalibrateFrame(int newFrame, GloveController glove)
    {
        if (glove == null)
        {
            return;
        }

        DataGloveIO gloveIO = glove.GetDataGlove();

        int saveSlot = glove.saveSlot;
        switch (newFrame)
        {
            case 0:
                gloveIO.CalibrateFlat();
                gloveIO.SetIMUHome();
                break;
            case 1:
                gloveIO.CalibrateFingersIn();
                break;
            case 2:
                gloveIO.CalibrateThumbIn();
                break;
        }
        gloveIO.SaveCalibration(saveSlot);

    }
}
