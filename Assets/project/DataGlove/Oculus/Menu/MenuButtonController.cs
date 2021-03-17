using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    public bool manualButtonPress = false;
   
    private bool render;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        render = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (manualButtonPress)
        {
            manualButtonPress = false;

            PressButton();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GloveController controller = other.gameObject.GetComponentInParent<GloveController>();

        if (controller != null)
        {
            PressButton();

            HapticController haptics = controller.gameObject.GetComponent<HapticController>();

            if (haptics != null)
            {
                haptics.HapticPulse(1, 0.1f);
            }
        }
    }

    private void PressButton()
    {
        render = !render;
        canvas.gameObject.SetActive(render);
    }

    private void OnTriggerExit(Collider other)
    {
        
    }


}
