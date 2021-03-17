using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Animator animator;
    public LampController lampController;
    public Material lampMat;
    public Material metalMat;
    public GameObject mainLamp;
    public bool manualButtonPress = false;
    Renderer meshRenderer;
    private Material[] objMaterials;
    private bool lightOn = true;
    private bool pressed = false; // Was the button just pressed
    private Vector4 onColor = new Vector4(1.7f, 1.7f, 1.7f, -10);
    private Vector4 offColor;
    private int counter = 0;
    private float timeButtonFreeze = 0;
    private float debounceTime = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        offColor = onColor * 0.2f;
        lampMat.SetColor("_EmissionColor", onColor);
        meshRenderer = mainLamp.GetComponent<MeshRenderer>();
        objMaterials = meshRenderer.materials;
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
        mainLamp.GetComponent<MeshRenderer>().materials = objMaterials;
        if (pressed)
        {
            counter++;
        }
        if (counter == 90)
        {
            counter = 0;
            pressed = !pressed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timeButtonFreeze > 0) { return; }

        GloveController controller = other.gameObject.GetComponentInParent<GloveController>();
        if (controller == null) { return; }
        
        string name = other.gameObject.name;
        if(name == "hammer")
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
        lightOn = !lightOn;
        lampController.ToggleLampLights(lightOn);

        if (lightOn)
        {
            lampMat.SetColor("_EmissionColor", onColor);
            objMaterials[1] = lampMat;
        }
        else
        {
            lampMat.SetColor("_EmissionColor", offColor);
            objMaterials[1] = metalMat;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GloveController controller = other.gameObject.GetComponentInParent<GloveController>();

        if (controller == null) { return; }
        
        string name = other.gameObject.name;
        if (controller.IsTipCollider(other) || name == "hammer")
        {
            if (!pressed)
            {
                animator.SetBool("isPressed", false);
            }
        }
    }
}
