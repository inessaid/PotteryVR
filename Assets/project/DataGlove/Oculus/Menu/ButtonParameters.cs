using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonParameters", menuName = "ScriptableObjects/ButtonParamaters", order = 1)]

// public parameters for use across multiple interactable components (buttons, sliders, dials, etc.)
public class ButtonParameters : ScriptableObject
{
    public float buttonDebounceTime;

    public Color standbyColor;
    public Color highlightColor;
    public Color switchStandbyColor;

    // DoTween Values
    public float hoverTweenTime = 0.3f;
    public float hoverScaleAmt = 1.2f;
    public float hoverMoveAmt = -0.6f;

    public float pushTweenTime = 0.2f;
    public float pushScaleAmt = 0.85f;
    public float pushMoveAmt = 0.5f;

}
