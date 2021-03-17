using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public GameObject knob1;
    public GameObject knob2;
    public GameObject knob3;
    public PanelButton button1;
    public PanelButton button2;
    public PanelButton button3;
    public Switch switch1;
    public Switch switch2;
    public Switch switch3;
    public ParticleSystem glow1;
    public ParticleSystem glow2;
    public GameObject floatingObject;

    private FloatObject floatObj;
    private float moveDistance = 0.1f;
    private Vector3 cubeStartPos;
    private float moveSpeed = 10.0f;


    // Start is called before the first frame update
    void Start()
    {
        floatObj = floatingObject.GetComponent<FloatObject>();
        cubeStartPos = floatingObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float x = knob1.transform.localRotation.eulerAngles.z;
        float y = knob2.transform.localRotation.eulerAngles.z;
        float z = knob3.transform.localRotation.eulerAngles.z;
        Vector3 newRotation = new Vector3(x, y, z);

        floatingObject.transform.rotation = Quaternion.Euler(newRotation);

        Vector3 newPos = new Vector3(cubeStartPos.x, cubeStartPos.y, cubeStartPos.z);

        if (switch1.GetIsUp())
        {
            newPos.x -= moveDistance;
        }

        if (switch2.GetIsUp())
        {
            newPos.y += moveDistance;
        }

        if (switch3.GetIsUp())
        {
            newPos.z += moveDistance;
        }

        floatingObject.transform.position = Vector3.Lerp(floatingObject.transform.position, newPos, moveSpeed * Time.deltaTime);

        //bool[] switchStates = { switch1.GetIsUp(), switch2.GetIsUp(), switch3.GetIsUp() };
        //floatObj.SetOsc(switchStates);
    }

    public void PressButton(PanelButton button)
    {
        Color glowColor;

        if (button == button1)
        {
            //b1
            glowColor = Color.blue;
        }
        else if (button == button2)
        {
            //b2
            glowColor = Color.red;
        }
        else 
        {
            //b3
            glowColor = Color.green;
        }

        var gradient1 = glow1.colorOverLifetime;
        var gradient2 = glow2.colorOverLifetime;

        Gradient newGradient1 = new Gradient();
        newGradient1.SetKeys(
            new GradientColorKey[] { new GradientColorKey(glowColor, 0.0f), new GradientColorKey(glowColor, 1.0f), new GradientColorKey(glowColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
        );

        Gradient newGradient2 = new Gradient();

        newGradient2.SetKeys(
            new GradientColorKey[] { new GradientColorKey(glowColor, 0.0f), new GradientColorKey(glowColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );

        gradient1.color = new ParticleSystem.MinMaxGradient(newGradient1);
        gradient2.color = new ParticleSystem.MinMaxGradient(newGradient2);
    }
}
