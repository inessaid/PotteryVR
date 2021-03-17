using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButton : MonoBehaviour
{
    public Animator animator;
    private Panel panel;

    // Start is called before the first frame update
    void Start()
    {
        panel = GetComponentInParent<Panel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Forte_Interactor interactor = other.gameObject.GetComponentInParent<Forte_Interactor>();
        string name = other.gameObject.name;
        if (name == "hammer")
        {
            PressButton();
        }
        if (interactor != null && !interactor.isGrabbing)
        {
            PressButton();

            interactor.gameObject.GetComponent<HapticController>().PulseOnFinger(other.gameObject, 0.1f);
        }
    }

    private void PressButton()
    {
        animator.SetBool("isPressed", true);
        panel.PressButton(this);
    }

    private void OnTriggerExit(Collider other)
    {
        animator.SetBool("isPressed", false);
    }
}

