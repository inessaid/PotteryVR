using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LampController : MonoBehaviour {

    //public Toggle globalLightToggle;
    //public Toggle lampLightToggle;
    //public Light globalLight;
    //private Material[] materials;

    public float redColorValueEmmision = 1.0f;
    public float greenColorValueEmmision = 1.0f;
    public float blueColorValueEmmision = 1.0f;

    //public List<Material> listOfMaterials;
    //public Dropdown materialDropdown;

    private Material activeMaterial;


    void Start()
    {
        /*foreach (var mat in listOfMaterials)
        {
            materialDropdown.options.Add(new Dropdown.OptionData { text = mat.name });
        }
        activeMaterial = GameObject.Find("MainLamp").GetComponent<Renderer>().materials[0];
        materials = GameObject.Find("MainLamp").GetComponent<Renderer>().sharedMaterials;

        materialDropdown.captionText.text = activeMaterial.name;*/

    }
    // Update is called once per frame
    void Update() {
        /*if (lampLightToggle.isOn == true)
        {
              ToggleLampLights(true);
        }
        else
        {
              ToggleLampLights(false);
        }

        if (globalLightToggle.isOn == true)
        {
            globalLight.enabled = true;
        }
        else
        {
            globalLight.enabled = false;
        }*/
    }

    public void AssignSelectedMaterial()
    {
        /*Material selectedMaterial = listOfMaterials[materialDropdown.value];
        GameObject.Find("MainLamp").GetComponent<Renderer>().material = selectedMaterial;*/
    }

    public void ToggleLampLights(bool enable)
    {
        foreach (Light light in transform.GetComponentsInChildren<Light>())
        {
            light.enabled = enable;
        }

        // Change hdr on Lamp material

        
        /*if (enable)
        {
            foreach (Material material in materials)
            {
                  material.SetColor("_EmissionColor", new Color(redColorValueEmmision, greenColorValueEmmision, blueColorValueEmmision, 1));
            }
        }
        else
        {
            foreach (Material material in materials)
            {
                  material.SetColor("_EmissionColor", new Color(0,0,0,0));
            }
        }*/
       
    }
}
