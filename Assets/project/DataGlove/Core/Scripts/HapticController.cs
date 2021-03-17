using UnityEngine;
using DataGlove;
using System.Collections;

public struct HapticSettings
{
    public int waveform;
    public float amplitude;
    public float grainLocation;
    public float grainSize;
    public float grainFade;
    public int pitch;
}

/// Data Glove module for haptics functionality
/**
 * User-friendly functions that handle one shot,
 * loop, and silence functionality as well as functionality
 * for granular synthesis commands.
 */
public class HapticController : MonoBehaviour
{
    private static int ACTUATORS = 6;
    private DataGloveIO dataGloveIO;

    /// Use this for initialization
    void Start()
    {
        dataGloveIO = gameObject.GetComponent<GloveController>().GetDataGlove();
    }

    //-----------------------------------------------------------------
    // Haptic Wrapper Methods for DataGloveIO methods
    //-----------------------------------------------------------------

    /// triggers one shot sound playback on one of the 6 glove actuators. 
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void SendOneShotHaptic(int actuatorID, int note, float amplitude)
    {
        dataGloveIO.SendOneShotHaptic(actuatorID, note, amplitude);
    }

    /// triggers loop sound playback on one of the 6 glove actuators.
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param note  0 - 127
     * @param amplitude  0 .0f - 1.0f
     */
    public void SendLoopHaptic(int actuatorID, int note, float amplitude)
    {
        dataGloveIO.SendLoopHaptic(actuatorID, note, amplitude);
    }

    /// changes a given actuator's sound file.
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param waveform slots  0 - 14
     */
    public void SelectHapticWave(int actuatorID, int waveform)
    {
        dataGloveIO.SelectHapticWave(actuatorID, waveform);
    }

    /// changes all actuator sound files
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param waveform  0 - 14
     */
    public void SelectHapticWaveForAllActuators(int waveform)
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SelectHapticWave(i, waveform);
        }
    }

    /// toggles on haptic one shot
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param makeOneShot  true is one shot and false is loop
     */
    public void ToggleOneShot(int actuatorID, bool makeOneShot)
    {
        dataGloveIO.ToggleOneShot(actuatorID, makeOneShot);
    }

    /// changes a given actuator's amplitude.
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param amplitude  0.0f - 1.0f
     */
    public void SetAmplitude(int actuatorID, float amplitude)
    {
        dataGloveIO.SetHapticAmp(actuatorID, amplitude);
    }

    /// changes all actuator amplitudes.
    /**
     * @param amplitude  0.0f - 1.0f
     */
    public void SetAmplitudeForAllActuators(float amplitude)
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SetAmplitude(i, amplitude);
        }
    }

    /// changes a given actuator's pitch bend.
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param pitch  0 - 127
     */
    public void SetPitchBend(int actuatorID, int pitch)
    {
        dataGloveIO.SetPitchBend(actuatorID, pitch);
    }

    /// changes all actuator pitch bends.
    /**
     * @param pitch  0 - 127
     */
    public void SetPitchBendForAllActuators(int pitch)
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SetPitchBend(i, pitch);
        }
    }

    /// changes a given actuator's grain size.
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param size  0.0f - 1.0f
     */
    public void SetGrainSize(int actuatorID, float size)
    {
        dataGloveIO.SetGrainSize(actuatorID, size);
    }

    /// changes all actuator grain sizes.
    /**
     * @param size  0.0f - 1.0f
     */
    public void SetGrainSizeForAllActuators(float size)
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SetGrainSize(i, size);
        }
    }

    /// changes a given actuator's grain fade.
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param fade  0.0f - 1.0f
     */
    public void SetGrainFade(int actuatorID, float fade)
    {
        dataGloveIO.SetGrainFade(actuatorID, fade);
    }

    /// changes all actuator grain fades.
    /**
     * @param fade  0.0f - 1.0f
     */
    public void SetGrainFadeForAllActuators(float fade)
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SetGrainFade(i, fade);
        }
    }

    /// changes a given actuator's grain location.
    /**
     * 0 is the thumb actuator - 5 is the palm actuator.
     * @param actuatorID  0 - 5
     * @param location  0.0f - 1.0f
     */
    public void SetGrainLocation(int actuatorID, float location)
    {
        dataGloveIO.SetGrainLocation(actuatorID, location);
    }

    /// changes a given actuator's grain location.
    /**
     * @param location  0.0f - 1.0f
     */
    public void SetGrainLocationForAllActuators(float location)
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SetGrainLocation(i, location);
        }
    }

    /// silences all haptics.
    /**
     */
    public void SilenceHaptics()
    {
        dataGloveIO.SilenceHaptics();
    }

    //-----------------------------------------------------------------
    // One Shot Methods
    //      Each method comes with a pre-defined method and one with
    //      parameters
    //-----------------------------------------------------------------

    //---------------------------
    // Palm One Shot
    //---------------------------

    /// pre-defined palm one shot command.
    /**
     * sends one shot to palm actuator
     * with predefined note and amplitude
     */
    public void PalmOneShot()
    {
        SendOneShotHaptic(5, 40, 1);
    }

    /// sends one shot command to palm actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void PalmOneShot(int note, float amplitude)
    {
        SendOneShotHaptic(5, note, amplitude);
    }

    //---------------------------
    // Thumb One Shot
    //---------------------------

    /// pre-defined thumb one shot command.
    /**
     * sends one shot to thumb actuator
     * with predefined note and amplitude
     */
    public void ThumbOneShot()
    {
        SendOneShotHaptic(0, 40, 1);
    }

    /// sends one shot command to thumb actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void ThumbOneShot(int note, float amplitude)
    {
        SendOneShotHaptic(0, note, amplitude);
    }

    //---------------------------
    // Index Finger One Shot
    //---------------------------

    /// pre-defined index one shot command.
    /**
     * sends one shot to index actuator
     * with predefined note and amplitude
     */
    public void IndexOneShot()
    {
        SendOneShotHaptic(1, 40, 1);
    }

    /// sends one shot command to index actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void IndexOneShot(int note, float amplitude)
    {
        SendOneShotHaptic(1, note, amplitude);
    }

    //---------------------------
    // Middle Finger One Shot
    //---------------------------

    /// pre-defined middle one shot command.
    /**
     * sends one shot to middle actuator
     * with predefined note and amplitude
     */
    public void MiddleOneShot()
    {
        SendOneShotHaptic(2, 40, 1);
    }

    /// sends one shot command to middle actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void MiddleOneShot(int note, float amplitude)
    {
        SendOneShotHaptic(2, note, amplitude);
    }

    //---------------------------
    // Ring Finger One Shot
    //---------------------------

    /// pre-defined ring one shot command.
    /**
     * sends one shot to ring actuator
     * with predefined note and amplitude
     */
    public void RingOneShot()
    {
        SendOneShotHaptic(3, 40, 1);
    }

    /// sends one shot command to ring actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void RingOneShot(int note, float amplitude)
    {
        SendOneShotHaptic(3, note, amplitude);
    }

    //---------------------------
    // Pinky Finger One Shot
    //---------------------------

    /// pre-defined pinky one shot command.
    /**
     * sends one shot to pinky actuator
     * with predefined note and amplitude
     */
    public void PinkyOneShot()
    {
        SendOneShotHaptic(4, 40, 1);
    }

    /// sends one shot command to pinky actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void PinkyOneShot(int note, float amplitude)
    {
        SendOneShotHaptic(4, note, amplitude);
    }

    //---------------------------
    // All Fingers One Shot (no palm)
    //---------------------------

    /// pre-defined all fingers (no palm) one shot command.
    /**
     * sends one shot to all finger actuators (no palm)
     * with predefined note and amplitude
     */
    public void OneShotAllFingers()
    {
        for (int i = 0; i < ACTUATORS - 1; i++)
        {
            SendOneShotHaptic(i, 40, 1);
        }
    }

    /// sends one shot command to all finger actuators (no palm).
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void OneShotAllFingers(int note, float amplitude)
    {
        for (int i = 0; i < ACTUATORS - 1; i++)
        {
            SendOneShotHaptic(i, note, amplitude);
        }
    }

    //---------------------------
    // All Actuators One Shot (fingers + palm)
    //---------------------------

    /// pre-defined all actuator (fingers + palm) one shot command.
    /**
     * sends one shot to all actuators (fingers + palm)
     * with predefined note and amplitude
     */
    public void OneShotAllActuators()
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SendOneShotHaptic(i, 40, 1);
        }
    }

    /// sends one shot command to all actuators (fingers + palm).
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void OneShotAllActuators(int note, float amplitude)
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SendOneShotHaptic(i, note, amplitude);
        }
    }

    //-----------------------------------------------------------------
    // Loop Methods
    //      Each method comes with a pre-defined method and one with
    //      parameters
    //-----------------------------------------------------------------

    //---------------------------
    // Palm Loop
    //---------------------------

    /// pre-defined palm loop command.
    /**
     * sends loop to palm actuator
     * with predefined note and amplitude
     */
    public void PalmLoop()
    {
        SendLoopHaptic(5, 40, 1);
    }

    /// sends loop command to palm actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void PalmLoop(int note, float amplitude)
    {
        SendLoopHaptic(5, note, amplitude);
    }

    //---------------------------
    // Thumb Loop
    //---------------------------

    /// pre-defined thumb loop command.
    /**
     * sends loop to thumb actuator
     * with predefined note and amplitude
     */
    public void ThumbLoop()
    {
        SendLoopHaptic(0, 40, 1);
    }

    /// sends loop command to thumb actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void ThumbLoop(int note, float amplitude)
    {
        SendLoopHaptic(0, note, amplitude);
    }

    //---------------------------
    // Index Finger Loop
    //---------------------------

    /// pre-defined index loop command.
    /**
     * sends loop to index actuator
     * with predefined note and amplitude
     */
    public void IndexLoop()
    {
        SendLoopHaptic(1, 40, 1);
    }

    /// sends loop command to index actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void IndexLoop(int note, float amplitude)
    {
        SendLoopHaptic(1, note, amplitude);
    }

    //---------------------------
    // Middle Finger Loop
    //---------------------------

    /// pre-defined middle loop command.
    /**
     * sends loop to middle actuator
     * with predefined note and amplitude
     */
    public void MiddleLoop()
    {
        SendLoopHaptic(2, 40, 1);
    }

    /// sends loop command to middle actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void MiddleLoop(int note, float amplitude)
    {
        SendLoopHaptic(2, note, amplitude);
    }

    //---------------------------
    // Ring Finger Loop
    //---------------------------

    /// pre-defined ring loop command.
    /**
     * sends loop to ring actuator
     * with predefined note and amplitude
     */
    public void RingLoop()
    {
        SendLoopHaptic(3, 40, 1);
    }

    /// sends loop command to ring actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void RingLoop(int note, float amplitude)
    {
        SendLoopHaptic(3, note, amplitude);
    }

    //---------------------------
    // Pinky Finger Loop
    //---------------------------

    /// pre-defined pinky loop command.
    /**
     * sends loop to pinky actuator
     * with predefined note and amplitude
     */
    public void PinkyLoop()
    {
        SendLoopHaptic(4, 40, 1);
    }

    /// sends loop command to pinky actuator.
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void PinkyLoop(int note, float amplitude)
    {
        SendLoopHaptic(4, note, amplitude);
    }

    //---------------------------
    // All Fingers Loop (no palm)
    //---------------------------

    /// pre-defined all fingers (no palm) loop command.
    /**
     * sends loop to all finger actuators (no palm)
     * with predefined note and amplitude
     */
    public void LoopAllFingers()
    {
        for (int i = 0; i < ACTUATORS - 1; i++)
        {
            SendLoopHaptic(i, 40, 1);
        }
    }

    /// sends loop command to all finger actuators (no palm).
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void LoopAllFingers(int note, float amplitude)
    {
        for (int i = 0; i < ACTUATORS - 1; i++)
        {
            SendLoopHaptic(i, note, amplitude);
        }
    }

    //---------------------------
    // All Actuators Loop (fingers + palm)
    //---------------------------

    /// pre-defined all actuator (fingers + palm) loop command.
    /**
     * sends loop to all actuators (fingers + palm)
     * with predefined note and amplitude
     */
    public void LoopAllActuators()
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SendLoopHaptic(i, 40, 1);
        }
    }

    /// sends loop command to all actuators (fingers + palm).
    /**
     * @param note  0 - 127
     * @param amplitude  0.0f - 1.0f
     */
    public void LoopAllActuators(int note, float amplitude)
    {
        for (int i = 0; i < ACTUATORS; i++)
        {
            SendLoopHaptic(i, note, amplitude);
        }
    }

    //-----------------------------------------------------------------
    // Single Actuator Silence Methods
    //      Turn amplitude to 0 for specific fingers
    //-----------------------------------------------------------------

    /// sets amplitude of palm actuator to 0.
    /**
     */
    public void SilencePalm()
    {
        SetAmplitude(5, 0);
    }

    /// sets amplitude of thumb actuator to 0.
    /**
     */
    public void SilenceThumb()
    {
        SetAmplitude(0, 0);
    }

    /// sets amplitude of index actuator to 0.
    /**
     */
    public void SilenceIndex()
    {
        SetAmplitude(1, 0);
    }

    /// sets amplitude of middle actuator to 0.
    /**
     */
    public void SilenceMiddle()
    {
        SetAmplitude(2, 0);
    }

    /// sets amplitude of ring actuator to 0.
    /**
     */
    public void SilenceRing()
    {
        SetAmplitude(3, 0);
    }

    /// sets amplitude of pinky actuator to 0.
    /**
     */
    public void SilencePinky()
    {
        SetAmplitude(4, 0);
    }

    public void HapticPulse(int actuator, float secLength)
    {
        HapticSettings settings = new HapticSettings();
        settings.waveform = 2;
        settings.amplitude = 1;
        settings.grainFade = 0;
        settings.grainLocation = 0;
        settings.grainSize = 0.03f;
        settings.pitch = 0;

        StartCoroutine(Pulse(actuator, settings, secLength));
    }

    public void HapticPulseAllActuators(float secLength)
    {
        HapticSettings settings = new HapticSettings();
        settings.waveform = 2;
        settings.amplitude = 1;
        settings.grainFade = 0;
        settings.grainLocation = 0;
        settings.grainSize = 0.03f;
        settings.pitch = 0;

        for (int i = 0; i < 6; i++)
        {
            StartCoroutine(Pulse(i, settings, secLength));
        }
    }

    public void HapticPulse(int actuator, HapticSettings settings, float secLength)
    {
        StartCoroutine(Pulse(actuator, settings, secLength));
    }

    IEnumerator Pulse(int actuator, HapticSettings settings, float secLength)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                //start
                SelectHapticWave(actuator, settings.waveform);
                SetGrainLocation(actuator, settings.grainLocation);
                SetGrainSize(actuator, settings.grainSize);
                SetGrainFade(actuator, settings.grainFade);
                SendLoopHaptic(actuator, settings.pitch, settings.amplitude);
            }
            else
            {
                //end
                SetAmplitude(actuator, 0);
            }
            yield return new WaitForSeconds(secLength);
        }
    }

    // Plays a haptic pulse on a finger for .1 seconds - Usful with triggers and collisions
    public void PulseOnFinger(GameObject colliderObj, float time)
    {
        int finger = -1;

        GloveController controller = gameObject.GetComponent<GloveController>();

        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                if (colliderObj == controller.middleJoints[i])
                {
                    finger = i;
                    break;
                }
            }
            else if (colliderObj == controller.tipJoints[i])
            {
                finger = i;
                break;
            }
        }

        if (finger > -1)
        {
            HapticPulse(finger, time);
        }
    }

    // Plays a haptic pulse on a finger for .1 seconds - Usful with triggers and collisions
    public void PulseOnFingerSettings(GameObject colliderObj, HapticSettings settings, float time)
    {
        int finger = -1;

        GloveController controller = gameObject.GetComponent<GloveController>();

        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                if (colliderObj == controller.knuckleJoints[i] || colliderObj == controller.middleJoints[i])
                {
                    finger = i;
                    break;
                }
            }
            else if (colliderObj == controller.knuckleJoints[i] || colliderObj == controller.middleJoints[i] || colliderObj == controller.tipJoints[i])
            {
                finger = i;
                break;
            }
        }

        if (finger > -1)
        {
            HapticPulse(finger, settings, time);
        }
    }
}
