using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataGlove;

/// Adds a haptic flourish in the simulator scene
/**
 * This script controls the haptic flourish during level transitions
 * The sound loops and the volume fades out.
 */
public class SequencerHaptics : MonoBehaviour
{
    // data glove
    private DataGloveIO[] dataGloves;
    private float elapsedTimer = 0;
    private float sequenceRate = 0.5f;
    private int currentNote = -1;
    private int[] noteSequence = { 50 }; //{ 55, 55, 55, 55, 55, 55, 60, 60, 60, 60, 60, 60 };     // Add any amount of values into this array to create steps, -1 is a silent step.
    private int sequenceIndex = 0;
    private int currentActuator = 0;
    private int actuatorIndex = 0;
    List<GameObject> currentCollisions;

    // Use this for initialization
    void Start()
    {
        dataGloves = FindObjectsOfType<DataGloveIO>();
        currentCollisions = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < dataGloves.Length; i++)
        {
            if (other.gameObject == dataGloves[i].gameObject)
            {
                currentCollisions.Add(other.gameObject);
                PlayNoteBothGloves(0, 50, 1);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < dataGloves.Length; i++)
        {
            if (other.gameObject == dataGloves[i].gameObject)
            {
                currentCollisions.Remove(other.gameObject);

                if (currentCollisions.Count == 0)
                {
                    SilenceNotes();
                }
            }
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    //if (elapsedTimer > sequenceRate && currentCollisions.Count > 0)
    //    //{
    //    //    elapsedTimer = 0;

    //    //    PlayNoteBothGloves(0, 40, 1);
    //    //}
    //}


    private void PlayNoteBothGloves(int actuator, int note, float amp)
    {
        InitializeSynth();

        for (int j = 0; j < dataGloves.Length; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                dataGloves[j].SendLoopHaptic(i, note, amp);
            }
        }
    }

    private void SilenceNotes()
    {
        for (int j = 0; j < dataGloves.Length; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                dataGloves[j].SetHapticAmp(i, 0);
            }
        }
    }

    private void InitializeSynth()
    {
        for (int j = 0; j < dataGloves.Length; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                dataGloves[j].SelectHapticWave(i, 13);
                dataGloves[j].SetGrainFade(i, 0);
                dataGloves[j].SetGrainSize(i, 1);
                dataGloves[j].SetGrainLocation(i, 0);
            }
        }
    }

    // Gets called every frame
    private void TriggerNextStep()
    {
        //currentNote = noteSequence[0];

        //SilenceNotes();
        // The -1 value is a silent step
        //if (currentNote != -1) { 
        PlayNoteBothGloves(0, 40, 1);
        //}

        //sequenceIndex++;
        //actuatorIndex++;

        //if (sequenceIndex > noteSequence.Length - 1)
        //{
        //    sequenceIndex = 0;
        //}

        //if (actuatorIndex > 5)
        //{
        //    actuatorIndex = 0;
        //}
    }
}