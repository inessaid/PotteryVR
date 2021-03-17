using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Manages UI components in the HapticsTester scene
public class HapticTester : MonoBehaviour {

    private static int ACTUATORS = 6;
    private GlovePair pair;
    private List<HapticController> haptics;
    private Slider[] sliders;
    private Dropdown hapticDropdown;
    private Dropdown waveformDropdown;
    private Text[] granularValues = new Text[5];
    private int currentActuator = 0;

    /// Use this for initialization
    void Start() {
        pair = FindObjectOfType<GlovePair>();
        haptics = pair.GetHaptics();
        sliders = GameObject.Find("Sliders").GetComponentsInChildren<Slider>();
        hapticDropdown = GameObject.Find("HapticDropdown").GetComponent<Dropdown>();
        waveformDropdown = GameObject.Find("WaveformDropdown").GetComponent<Dropdown>();

        // ordered array of values
        granularValues[0] = GameObject.Find("wav_loc_value").GetComponent<Text>();
        granularValues[1] = GameObject.Find("wav_vol_value").GetComponent<Text>();
        granularValues[2] = GameObject.Find("pitch_bend_value").GetComponent<Text>();
        granularValues[3] = GameObject.Find("grain_size_value").GetComponent<Text>();
        granularValues[4] = GameObject.Find("grain_fade_value").GetComponent<Text>();

        List<string> waveStorage = new List<string>();
        for(int i=0; i<16; i++)
        {
            waveStorage.Add(i.ToString());
        }
        waveformDropdown.AddOptions(waveStorage);
    }

    /// updates all text values w/ slider values
    private void Update()
    {
        // update Text with slider values
        for (int i=0; i<sliders.Length; i++)
        {
            granularValues[i].text = sliders[i].value.ToString();
        }
    }

    /// Reset wave location
    public void WaveLocation()
    {
        for (int i = 0; i < haptics.Count; i++)
        {
            if (currentActuator == 18)
            {
                haptics[i].SetGrainLocationForAllActuators(sliders[0].value);
                continue;
            }
            haptics[i].SetGrainLocation(currentActuator, sliders[0].value);
        }
    }

    /// Reset amplitude
    public void WaveVolume()
    {
        for (int i = 0; i < haptics.Count; i++)
        {
            if (currentActuator == 18)
            {
                haptics[i].SetAmplitudeForAllActuators(sliders[1].value);
                continue;
            }
            haptics[i].SetAmplitude(currentActuator, sliders[1].value);
        }
    }

    /// Reset pitch bend (bit crush)
    public void PitchBend()
    {
        for (int i = 0; i < haptics.Count; i++)
        {
            if (currentActuator == 18)
            {
                haptics[i].SetPitchBendForAllActuators((int)sliders[2].value);
                continue;
            }
            haptics[i].SetPitchBend(currentActuator, (int)sliders[2].value);
        }
    }

    /// Reset grain size
    public void GrainSize()
    {
        for (int i = 0; i < haptics.Count; i++)
        {
            if (currentActuator == 18)
            {
                haptics[i].SetGrainSizeForAllActuators(sliders[3].value);
                continue;
            }
            haptics[i].SetGrainSize(currentActuator, sliders[3].value);
        }
    }

    /// Reset grain fade
    public void GrainFade()
    {
        for (int i = 0; i < haptics.Count; i++)
        {
            if (currentActuator == 18)
            {
                haptics[i].SetGrainFadeForAllActuators(sliders[4].value);
                continue;
            }
            haptics[i].SetGrainFade(currentActuator, sliders[4].value);
        }
    }

    /// Dropdown menu uses this to change the haptic function command
    public void ChangeHapticCommand()
    {
        for (int i = 0; i < haptics.Count; i++)
        {
            switch (hapticDropdown.value)
            {
                case 0:
                    currentActuator = 0;
                    haptics[i].SilenceHaptics();
                    haptics[i].ThumbOneShot();
                    break;
                case 1:
                    currentActuator = 0;
                    haptics[i].SilenceHaptics();
                    haptics[i].ThumbLoop();
                    break;
                case 2:
                    currentActuator = 1;
                    haptics[i].SilenceHaptics();
                    haptics[i].IndexOneShot();
                    break;
                case 3:
                    currentActuator = 1;
                    haptics[i].SilenceHaptics();
                    haptics[i].IndexLoop();
                    break;
                case 4:
                    currentActuator = 2;
                    haptics[i].SilenceHaptics();
                    haptics[i].MiddleOneShot();
                    break;
                case 5:
                    currentActuator = 2;
                    haptics[i].SilenceHaptics();
                    haptics[i].MiddleLoop();
                    break;
                case 6:
                    currentActuator = 3;
                    haptics[i].SilenceHaptics();
                    haptics[i].RingOneShot();
                    break;
                case 7:
                    currentActuator = 3;
                    haptics[i].SilenceHaptics();
                    haptics[i].RingLoop();
                    break;
                case 8:
                    currentActuator = 4;
                    haptics[i].SilenceHaptics();
                    haptics[i].PinkyOneShot();
                    break;
                case 9:
                    currentActuator = 4;
                    haptics[i].SilenceHaptics();
                    haptics[i].PinkyLoop();
                    break;
                case 10:
                    currentActuator = 5;
                    haptics[i].SilenceHaptics();
                    haptics[i].PalmOneShot();
                    break;
                case 11:
                    currentActuator = 5;
                    haptics[i].SilenceHaptics();
                    haptics[i].PalmLoop();
                    break;
                case 12:
                    currentActuator = 18;
                    haptics[i].SilenceHaptics();
                    haptics[i].OneShotAllFingers();
                    break;
                case 13:
                    currentActuator = 18;
                    haptics[i].SilenceHaptics();
                    haptics[i].LoopAllFingers();
                    break;
                case 14:
                    currentActuator = 18;
                    haptics[i].SilenceHaptics();
                    haptics[i].OneShotAllActuators();
                    break;
                case 15:
                    currentActuator = 18;
                    haptics[i].SilenceHaptics();
                    haptics[i].LoopAllActuators();
                    break;
                default:
                    break;
            }
        }
        SetGranularValues();
    }

    /// Sets new waveform
    public void ChangeWaveform()
    {
        for(int i=0; i<haptics.Count; i++)
        {
            haptics[i].SelectHapticWaveForAllActuators(waveformDropdown.value);
        }
        SetGranularValues();
    }

    /// Sets the 5 granular values
    private void SetGranularValues()
    {
        WaveLocation();
        WaveVolume();
        PitchBend();
        GrainSize();
        GrainFade();
    }

    /// Button uses this to refire a one shot command
    public void RefireOneShot()
    {
        // even numbers are one shot haptic commands
        if (hapticDropdown.value % 2 == 0)
        {
            ChangeHapticCommand();
        }
    }

    public void Silence()
    {
        for(int i=0; i < haptics.Count; i++)
        {
            haptics[i].SilenceHaptics();
        }
    }

}
