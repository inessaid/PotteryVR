using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ButtonControlUI : MonoBehaviour
{
    bool isHovering;
    bool isPushing;

    SpriteRenderer spriteRend;

    // Scriptable object containing public parameters that are used across multiple interactable components
    [SerializeField] ButtonParameters buttonParameters;

    Vector3 initScale;
    Vector3 initPos;

    // Glove References
    GloveController glove;
    HapticController haptics;
    GestureRecognition gestures;
    string currGesture;
    bool pushGestureActive;


    // ACTIONS - set an event that is triggered when this action is true
    public UnityEvent buttonClick;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();

        initScale = transform.localScale;
        initPos = transform.localPosition;
        //initColor = Parameters.standbyColor;

        glove = GameObject.FindGameObjectWithTag("UserHand").GetComponent<GloveController>();
        gestures = glove.GetComponent<GestureRecognition>();
        haptics = glove.GetComponent<HapticController>();

    }

    private void OnEnable()
    {
        spriteRend.color = buttonParameters.standbyColor;
        transform.localPosition = initPos;
        isPushing = false;
    }

    void Update()
    {
        CheckGesture();
    }

    void CheckGesture()
    {
        currGesture = gestures.GetCurrentGesture();
        pushGestureActive = (currGesture == "Pointing" || currGesture == "Peace" || currGesture == "Pinch");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if thumb, index or middle finger tip is within collider. Triggers a hover animation, mainly for visual feedback.


        // NOTE: I tagged the thumb, index, and middle finger tips as "Finger Tips". Feel free to use a different detection method
        if (other.gameObject.CompareTag("FingerTip") && !isPushing)
        {
            transform.DOScale(initScale * buttonParameters.hoverScaleAmt, buttonParameters.hoverTweenTime);
            transform.DOLocalMoveZ(initPos.z + buttonParameters.hoverMoveAmt, buttonParameters.hoverTweenTime);
            spriteRend.DOColor(buttonParameters.highlightColor, buttonParameters.hoverTweenTime);
            isHovering = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("FingerTip") && !isPushing && isHovering && pushGestureActive)
        {

            if (currGesture == "Pinch")
            {
                haptics.IndexOneShot();
                haptics.ThumbOneShot();
            }

            else
            {
                haptics.IndexOneShot();
                haptics.MiddleOneShot();
            }

            isHovering = false;
            isPushing = true;
            transform.DOScale(initScale * buttonParameters.pushScaleAmt, buttonParameters.hoverTweenTime);
            transform.DOLocalMoveZ(initPos.z + buttonParameters.pushMoveAmt, buttonParameters.pushTweenTime);

            // Current debounce method. It's a cooldown timer. This will be improved/ modified
            StartCoroutine(TurnOffPush());


            // If there is one or more functions to trigger when the button is pressed, handle those here
            if (buttonClick.GetPersistentEventCount() > 0)
            {
                buttonClick.Invoke();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPushing)
        {
            transform.DOScale(initScale, buttonParameters.hoverTweenTime);
            transform.DOLocalMoveZ(initPos.z, buttonParameters.hoverTweenTime);
            spriteRend.DOColor(buttonParameters.standbyColor, buttonParameters.hoverTweenTime);
            isHovering = false;
        }
    }

    IEnumerator TurnOffPush()
    {
        yield return new WaitForSeconds(buttonParameters.buttonDebounceTime);
        transform.DOLocalMoveZ(initPos.z, buttonParameters.pushTweenTime);
        isPushing = false;

    }

    // Action debug
    public void PrintThing()
    {
        Debug.Log(transform.name + " was pressed.");
    }

}

