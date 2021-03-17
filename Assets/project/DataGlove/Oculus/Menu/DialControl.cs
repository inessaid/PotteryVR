using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DialControl : MonoBehaviour
{
    Vector3 initScale;
    Vector3 initPos;

    // Scriptable object containing public parameters that are used across multiple interactable components
    [SerializeField] ButtonParameters buttonParameters;

    // If checked, will turn dial into a discrete dial
    [SerializeField] bool isDiscrete;
    // Set the angles at which to snap the dial
    [SerializeField] float[] discreteAngles;
    float prevAngle = 0f;

    // If not discrete, set the minimum and maximum angles at which to clamp the dial
    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;
    bool clampedMax;
    bool clampedMin;

    SpriteRenderer sr;

    float zRot = 0f;
    float offsetZRot = 0f;

    // Set the radial position of 0 degrees
    [SerializeField] float zOffset = 0f;
    float gloveZOrigin = 0f;
    float zOrigin = 0f;


    // Glove References
    GloveController glove;
    HapticController haptics;
    GestureRecognition gestures;

    bool isHovering;
    bool dragHeldDown;
    bool pinchGestureActive;


    void Awake()
    {
        // NOTE: I tagged the glove game object as "UserHand". Feel free to use another method for hand detection
        glove = GameObject.FindGameObjectWithTag("UserHand").GetComponent<GloveController>();
        gestures = glove.GetComponent<GestureRecognition>();
        haptics = glove.GetComponent<HapticController>();

        sr = GetComponent<SpriteRenderer>();
        Sprite sp = sr.sprite;

        initScale = transform.localScale;
        initPos = transform.localPosition;

    }

    void Update()
    {
        pinchGestureActive = (gestures.GetCurrentGesture() == "Fist" || gestures.GetCurrentGesture() == "Pinch" || gestures.GetCurrentGesture() == "Flat");

        // Activates dial if hand is in pinching gestures (thumb + index together). Remains active even if glove leaves the dial's collider
        if (pinchGestureActive && dragHeldDown)
        {
            zRot = (zOrigin + gloveZOrigin + glove.wrist.transform.localEulerAngles.z) % 360f;

            // Handles discrete dial functionality
            if (isDiscrete && discreteAngles.Length > 0)
            {
                offsetZRot = (zRot + zOffset) % 360f;

                float minDifference = 10000f;
                float closestAngle = 0;

                foreach (float angle in discreteAngles)
                {
                    float diff = Mathf.Abs(angle - offsetZRot);
                    if (diff < minDifference)
                    {
                        minDifference = diff;
                        closestAngle = angle;
                    }
                }

                if (closestAngle != prevAngle)
                {
                    prevAngle = closestAngle;

                    haptics.IndexOneShot();
                    haptics.ThumbOneShot();
                }

                zRot = closestAngle - zOffset;
            }

            //Debug.Log(zRot);

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRot);

            // clamps dial to min and max values
            //CheckClamping();
        }


        // release dial if pinch gesture is released
        else if (!pinchGestureActive && !isHovering)
        {
            glove.isGrabbing = false;
            dragHeldDown = false;
            ExitHover();
        }


    }

    // not clamping properly. Revisit
    void CheckClamping()
    {
        if (transform.eulerAngles.z >= maxAngle && transform.eulerAngles.z < minAngle && !clampedMin)
        {
            transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, maxAngle);
            clampedMax = true;
        }

        else if (transform.eulerAngles.z <= maxAngle && transform.eulerAngles.z > minAngle && !clampedMax)
        {
            transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, minAngle);
            clampedMin = true;
        }

        else
        {
            clampedMax = false;
            clampedMin = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if thumb, index or middle finger tip is within collider. Triggers a hover animation, mainly for visual feedback.


        // NOTE: I tagged the thumb, index, and middle finger tips as "Finger Tips". Feel free to use a different detection method
        if (other.gameObject.CompareTag("FingerTip") && !glove.isGrabbing)
        {
            transform.DOScale(initScale * buttonParameters.hoverScaleAmt, buttonParameters.hoverTweenTime);
            transform.DOLocalMoveZ(initPos.z + buttonParameters.hoverMoveAmt, buttonParameters.hoverTweenTime);
            sr.DOColor(buttonParameters.highlightColor, buttonParameters.hoverTweenTime);

            isHovering = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isHovering && pinchGestureActive && !dragHeldDown)
        {
            dragHeldDown = true;
            glove.isGrabbing = true;

            haptics.IndexOneShot();
            haptics.ThumbOneShot();


            // sets origins so that the dial does not return to initial position after every grab
            gloveZOrigin = glove.wrist.transform.localEulerAngles.z;
            zOrigin = transform.eulerAngles.z;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // registers that the glove left the collider, but does not deactivate dial until pinch gesture is released
        isHovering = false;
    }

    void ExitHover()
    {
        transform.DOScale(initScale, buttonParameters.hoverTweenTime);
        transform.DOLocalMoveZ(initPos.z, buttonParameters.hoverTweenTime);
        sr.DOColor(buttonParameters.standbyColor, buttonParameters.hoverTweenTime);
    }
}
