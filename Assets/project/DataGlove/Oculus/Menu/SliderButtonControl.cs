using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SliderButtonControl : MonoBehaviour
{
    public Transform fingerTipPos = null;
    public bool inContact;

    [SerializeField] ButtonParameters buttonParameters;

    SpriteRenderer sr;
    GloveController glove;

    Vector3 initScale;
    Vector3 initPos;

    private void Awake()
    {
        initScale = transform.localScale;
        initPos = transform.localPosition;

        sr = GetComponent<SpriteRenderer>();
        glove = GameObject.FindGameObjectWithTag("UserHand").GetComponent<GloveController>();

    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if thumb, index or middle finger tip is within collider. Triggers a hover animation, mainly for visual feedback.


        // NOTE: I tagged the thumb, index, and middle finger tips as "Finger Tips". Feel free to use a different detection method
        if (other.gameObject.CompareTag("FingerTip") && !glove.isGrabbing){
            transform.DOScale(initScale * buttonParameters.hoverScaleAmt, buttonParameters.hoverTweenTime);
            //transform.DOLocalMoveZ(initPos.z + buttonParameters.hoverMoveAmt, buttonParameters.hoverTweenTime);
            sr.DOColor(buttonParameters.highlightColor, buttonParameters.hoverTweenTime);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("FingerTip") && !inContact){
            inContact = true;
            fingerTipPos = other.transform;
        }
    }


    public void ExitHighlight()
    {
        transform.DOScale(initScale, buttonParameters.hoverTweenTime);
        //transform.DOLocalMoveZ(initPos.z, buttonParameters.hoverTweenTime);
        //sr.DOColor(buttonParameters.standbyColor, buttonParameters.hoverTweenTime);
    }
}
