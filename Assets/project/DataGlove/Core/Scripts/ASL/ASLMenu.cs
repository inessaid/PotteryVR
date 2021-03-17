using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASLMenu : MonoBehaviour
{
    public Animator animator;
    public GameObject canvas;
    public bool manualButtonPress = false;
    private bool render;
    private bool pressed = false;
    private int counter = 0;
    private float timeButtonFreeze = 0;
    private float debounceTime = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        render = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeButtonFreeze > 0)
        {
            timeButtonFreeze -= Time.deltaTime;
        }
        if (manualButtonPress)
        {
            manualButtonPress = false;
            PressButton();
        }
        if (pressed)
        {
            counter++;
        }
        if(counter == 50)
        {
            counter = 0;
            pressed = !pressed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timeButtonFreeze > 0)
        {
            return;
        }

        GloveController controller = other.gameObject.GetComponentInParent<GloveController>();
        if (controller == null)
        {
            return;
        }

        string name = other.gameObject.name;
        if (name == "hammer")
        {
            PressButton();
            timeButtonFreeze = debounceTime;

            animator.SetBool("isPressed", true);
        }

        if (controller.IsTipCollider(other))
        {
            if (!pressed)
            {
                PressButton();
                timeButtonFreeze = debounceTime;

                animator.SetBool("isPressed", true);

                ASLtext text = canvas.GetComponentInChildren<ASLtext>();

                HapticController haptics = controller.gameObject.GetComponent<HapticController>();
                int fingerIndex = -1;
                string lowercaseChild = other.gameObject.name.ToLower();

                if (lowercaseChild.Contains("thumb"))
                {
                    fingerIndex = 0;
                }
                else if (lowercaseChild.Contains("index"))
                {
                    fingerIndex = 1;
                }
                else if (lowercaseChild.Contains("middle"))
                {
                    fingerIndex = 2;
                }
                else if (lowercaseChild.Contains("ring"))
                {
                    fingerIndex = 3;
                }
                else if (lowercaseChild.Contains("pinky"))
                {
                    fingerIndex = 4;
                }
                if (haptics != null)
                {
                    haptics.HapticPulse(fingerIndex, 0.1f);
                }
            }
        }
    }

    private void PressButton()
    {
        if (!pressed)
        {
            render = !render;
            canvas.SetActive(render);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GloveController controller = other.gameObject.GetComponentInParent<GloveController>();
        if (controller == null)
        {
            return;
        }

        string name = other.gameObject.name;
        if (controller.IsTipCollider(other) || name == "hammer")
        {
            if (!pressed)
            {
                animator.SetBool("isPressed", false);
            }
        }
    }

    public bool IsRendering(){
        return render;
    }
}

