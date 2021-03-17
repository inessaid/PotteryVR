using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataGlove;

struct FingerSums
{
    public float[] sums;
};

/// Data Glove module that determines if a trigger occurs.
/**
 * To use this class, place calculateAllTrigs() in the update method of another
 * script.
 */
public class Triggers {
    public enum TriggerState { None, Positive, Negative };

    int MAX_BUFFER_SIZE = 1000;
    int MAX_TIME_SINCE_TRIG = 9999;
    float resetMultiplier = 0.8f;

    /**
     A list of structs that each contain an array of the 5 finger sums
     */
    List<FingerSums> buffer = new List<FingerSums>();

    /**
     The number of updates since the last trigger on each finger
     */
    int[] timeSinceLastTrig = { 0, 0, 0, 0, 0 };

    /**
     Positive trigger threshold for each finger
     */
    int[] posThresh = { 30, 30, 30, 30, 30 };

    /**
     Positive reset trigger threshold for each finger
     */
    int[] posReset = { -15, -15, -15, -15, -15 };

    /**
     The number of indices between the latest and delayed values of the
     fingerVals deque for positive triggers on each finger
     */
    int[] posBufferSize = { 5, 5, 5, 5, 5 };

    /**
     Negative trigger threshold for each finger
     */
    int[] negThresh = { -50, -50, -50, -50, -50 };

    /**
     Negative reset trigger threshold for each finger
     */
    int[] negReset = { 15, 15, 15, 15, 15 };

    /**
     The number of indices between the latest and delayed values of the
     fingerVals deque for negative triggers on each finger
     */
    int[] negBufferSize = { 5, 5, 5, 5, 5 };

    /**
     Whether positive triggers are active for each finger
     */
    bool[] posTriggerActive = { true, true, true, true, true };

    /**
     Whether negative triggers are active for each finger
     */
    bool[] negTriggerActive = { true, true, true, true, true };

    /**
     Whether absolute threshold triggers are active for each finger
     */
    bool[] absTrigActive = { true, true, true, true, true };

    /**
     Whether an absolute trigger fires upon return to the reset threshold for each finger
     */
    bool[] absReturnActive = { true, true, true, true, true };

    /**
     Indicates that a reset is needed before a positive trigger will be generated
     */
    bool[] needsPosReset = { false, false, false, false, false };

    /**
     Indicates that a reset is needed before a negative trigger will be generated
     */
    bool[] needsNegReset = { false, false, false, false, false };

    /**
     Indicates that a reset is needed before an absolute trigger will be generated
     */
    bool[] needsAbsReset = { false, false, false, false, false };

    /**
     Absolute finger sum trigger threshold for each finger.
     */
    int[] absoluteThresh = { 250, 250, 250, 250, 250 };

    /**
     Absolute finger sum reset trigger threshold for each finger
     */
    int[] absoluteReset = { 200, 200, 200, 200, 200 };

    /**
    Absolute finger sum reset trigger threshold for each finger
     */
    int[] postDeltaAbsReset = { 60, 60, 60, 60, 60 };

    /**
     The length of lookback into the trigger history when filtering duplicate triggers.
     Set to zero for no filtering of triggers. Lower numbers will give faster response times.
     */
    int repeatFilterNum = 0;

    /**
     The current trigger on/off state for all fingers
     */
    List<TriggerState> allFingerStates = new List<TriggerState> { TriggerState.None, 
        TriggerState.None, TriggerState.None, TriggerState.None, TriggerState.None };
    
    List<TriggerState> lastTrigger = new List<TriggerState> { TriggerState.None, TriggerState.None, 
                                            TriggerState.None, TriggerState.None, TriggerState.None };

    public Triggers() {
        
    }

    /**
     * Pass the latest sensor values to this method every frame. 
     * The method returns which finger has been triggered this frame.
     */
    public List<TriggerState> calculateAllTrigs(FingerVals[] fingerVals)
    {
        NewFrame(fingerVals);

        float currentSum = buffer[buffer.Count - 1].sums[1];

        for (int i = 0; i < 5; i++)
        {
            if (timeSinceLastTrig[i] > repeatFilterNum)
            {
                TriggerState absoluteState = calcAbsoluteTrig(i);

                if (calcPosDeltaTrig(i))
                {
                    allFingerStates[i] = TriggerState.Positive;
                }
                else if (calcNegDeltaTrig(i))
                {
                    allFingerStates[i] = TriggerState.Negative;
                }
                else if (absoluteState != TriggerState.None)
                {
                    allFingerStates[i] = absoluteState;

                    if (i == 1)
                    {
                        //Debug.Log("Absolute Trig = " + absoluteState);
                    }
                } 
                else 
                {
                    allFingerStates[i] = TriggerState.None;
                }
                
                if (allFingerStates[i] != TriggerState.None)
                {
                    timeSinceLastTrig[i] = 0;
                }
            }
            else
            {
                allFingerStates[i] = TriggerState.None;
            }
            
            if (timeSinceLastTrig[i] < MAX_TIME_SINCE_TRIG)
            {
                timeSinceLastTrig[i]++;
            }

            if (allFingerStates[i] != TriggerState.None) 
            {
                lastTrigger[i] = allFingerStates[i];
            }
        }
        
        return allFingerStates;
    }

    /**
     Add the five finger values to the fingerVals list.
     
     @param fingerSums[] is an array of five finger sums of type float.
     */
    void NewFrame(FingerVals[] fingerVals)
    {
        FingerSums fingerData = new FingerSums();
        fingerData.sums = new float[5];

        for (int i = 0; i < 5; i++)
        {
            fingerData.sums[i] = (fingerVals[i].sensor1 + fingerVals[i].sensor2) * 254;
        }

        buffer.Add(fingerData);

        if (buffer.Count > MAX_BUFFER_SIZE)
        {
            buffer.RemoveAt(0);
        }
    }

    /**
     Returns whether a positve trigger is active on a given finger
     
     @return The positive trigger state
     */
    bool calcPosDeltaTrig(int fingerNum)
    {
        if (posTriggerActive[fingerNum] && buffer.Count > posBufferSize[fingerNum])
        {
            // Subtract the delayed finger sum from the current finger sum.
            // The delay time is set by the buffer size.
            float bufferedValue = buffer[buffer.Count - posBufferSize[fingerNum]].sums[fingerNum];
            float diffAmount = buffer[buffer.Count - 1].sums[fingerNum] - bufferedValue;

            // If the positive difference value exceeds the threshold and if
            // a reset is not needed for trigger output.
            if (diffAmount > posThresh[fingerNum] && !needsPosReset[fingerNum] && lastTrigger[fingerNum] != TriggerState.Positive)
            {
                needsPosReset[fingerNum] = true;

                float currentSum = buffer[buffer.Count - 1].sums[fingerNum];
                absoluteReset[fingerNum] = (int)Mathf.Round(currentSum * resetMultiplier);

                //if (fingerNum == 1)
                //{
                //    Debug.Log("Positive Index Delta Trig!");
                //}

                return true;
            }
            else if (diffAmount < posReset[fingerNum])
            {
                // The reset threshold has been crossed. Subsequent postive triggers that cross the
                // posThresh value will be allowed.
                needsPosReset[fingerNum] = false;
            }
        }

        return false;
    }

    /**
     Returns whether a negative trigger is active on a given finger
     
     @return The negative trigger state
     */
    bool calcNegDeltaTrig(int fingerNum)
    {
        if (negTriggerActive[fingerNum] && buffer.Count > negBufferSize[fingerNum])
        {
            // Subtract the delayed finger sum from the current finger sum.
            // The delay time is set by the buffer size.
            float bufferedValue = buffer[buffer.Count - negBufferSize[fingerNum]].sums[fingerNum];
            float diffAmount = buffer[buffer.Count - 1].sums[fingerNum] - bufferedValue;

            // If the negative difference value exceeds the threshold and if
            // a reset is not needed for trigger output.
            if (diffAmount < negThresh[fingerNum] && !needsNegReset[fingerNum] && lastTrigger[fingerNum] != TriggerState.Negative)
            {
                needsNegReset[fingerNum] = true;
                //if (fingerNum == 1)
                //{
                //    Debug.Log("Negative Index Delta Trig!");
                //}
                return true;
            }
            else if (diffAmount > negReset[fingerNum]) // && needsAbsReset[fingerNum])
            {
                // The reset threshold has been crossed. Subsequent postive triggers that cross the
                // negThresh value will be allowed.
                needsNegReset[fingerNum] = false;
            }
        }
        return false;
    }

    ///**
    // Returns whether an absolute trigger is active on a given finger.
    // An absolute trigger occurs when a finger sum passes the threshold.

    // @return The absolute trigger state
    // */
    TriggerState calcAbsoluteTrig(int fingerNum) {
        float currentSum = buffer[buffer.Count - 1].sums[fingerNum];
        float lastSum = buffer[buffer.Count - 2].sums[fingerNum];

        if (currentSum > absoluteThresh[fingerNum] && lastSum < absoluteThresh[fingerNum] && 
                                                        lastTrigger[fingerNum] != TriggerState.Positive) 
        {
            absoluteReset[fingerNum] = 200;

            return TriggerState.Positive;
        } 
        else if (currentSum < absoluteReset[fingerNum] && lastSum > absoluteReset[fingerNum] &&
                                                        lastTrigger[fingerNum] != TriggerState.Negative)
        {
            return TriggerState.Negative;
        }
        else
        {
            return TriggerState.None;
        }
    }

}
