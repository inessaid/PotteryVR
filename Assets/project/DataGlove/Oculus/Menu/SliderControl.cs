using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderControl : MonoBehaviour
{
    [SerializeField] GameObject sliderButton;
    [SerializeField] GameObject sliderFillBar;
    [SerializeField] ButtonParameters buttonParameters;

    SliderButtonControl sliderButtonControl;
    SpriteRenderer sr;

    float barLength;
    float minPosition;
    float maxPosition;


    // Glove References
    GloveController glove;
    HapticController haptics;
    GestureRecognition gestures;

    bool dragHeldDown = false;
    bool pinchGestureActive = false;

    public float percentVal;

    void Awake()
    {
        glove = GameObject.FindGameObjectWithTag("UserHand").GetComponent<GloveController>();
        gestures = glove.GetComponent<GestureRecognition>();
        haptics = glove.GetComponent<HapticController>();

        sliderButtonControl = sliderButton.GetComponent<SliderButtonControl>();

        sr = GetComponent<SpriteRenderer>();
        Sprite sp = sr.sprite;
        barLength = sp.rect.width / sp.pixelsPerUnit;

        minPosition = transform.position.x;
        maxPosition = minPosition + barLength;
        
    }

    void Update()
    {
        pinchGestureActive = (gestures.GetCurrentGesture() == "Pinch");

        if (sliderButtonControl.inContact && pinchGestureActive && !glove.isGrabbing)
        {
            sr.color = buttonParameters.highlightColor;
            dragHeldDown = true;

            haptics.IndexOneShot();
            haptics.ThumbOneShot();

            glove.isGrabbing = true;
        }


        if (dragHeldDown && pinchGestureActive)
        {
            float xPos = Mathf.Clamp(sliderButtonControl.fingerTipPos.position.x, transform.position.x + barLength * transform.lossyScale.x, transform.position.x);

            sliderButton.transform.position = new Vector3(xPos, sliderButton.transform.position.y, sliderButton.transform.position.z);

            percentVal = -(transform.position.x - xPos) / (barLength * transform.lossyScale.x);

            sliderFillBar.transform.localScale = new Vector3(percentVal, sliderFillBar.transform.localScale.y, 1f);
        }


        if (!pinchGestureActive)
        {
            dragHeldDown = false;
            sliderButtonControl.inContact = false;
            sr.color = buttonParameters.standbyColor;

            glove.isGrabbing = false;

            sliderButtonControl.ExitHighlight();
        }
    }
}
