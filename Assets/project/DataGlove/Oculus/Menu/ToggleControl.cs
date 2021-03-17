using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleControl : MonoBehaviour
{
    [SerializeField] Sprite uncheckedSprite;
    [SerializeField] Sprite checkedSprite;

    public bool boxChecked;
    public float toggleDebounceTime = 0.5f;
    bool debounced;

    SpriteRenderer sr;

    // Scriptable object containing public parameters that are used across multiple interactable components
    [SerializeField] ButtonParameters buttonParameters;


    // Glove References
    GloveController glove;
    HapticController haptics;
    GestureRecognition gestures;

    Vector3 mousePos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        SetSprite();

        glove = GameObject.FindGameObjectWithTag("UserHand").GetComponent<GloveController>();
        gestures = glove.GetComponent<GestureRecognition>();
        haptics = glove.GetComponent<HapticController>();
    }

    private void OnTriggerStay(Collider other)
    {
        // Checks if thumb, index or middle finger tip is within collider. If pinching, toggle state and update sprite.
        // Debounce is currently a timed cooldown. This will be udpated/ modified

        // NOTE: I tagged the thumb, index, and middle finger tips as "Finger Tips". Feel free to use a different detection method
        if (other.gameObject.CompareTag("FingerTip") && gestures.GetCurrentGesture() == "Pinch" && !debounced)
        {
            haptics.IndexOneShot();
            haptics.ThumbOneShot();
            boxChecked = !boxChecked;
            SetSprite();

            debounced = true;
            StartCoroutine(StartToggleDebounce());
        }
    }

    void SetSprite()
    {
        if (boxChecked)
        {
            sr.sprite = checkedSprite;
            sr.color = buttonParameters.highlightColor;
        }
        else
        {
            sr.sprite = uncheckedSprite;
            sr.color = buttonParameters.standbyColor;
        }
    }

    IEnumerator StartToggleDebounce()
    {
        yield return new WaitForSeconds(toggleDebounceTime);
        debounced = false;
    }
}
