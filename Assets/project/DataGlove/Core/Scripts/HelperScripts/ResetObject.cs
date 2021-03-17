using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObject : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Transform startParent;
    private const float yPositionThreshold = -5f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.position;
        startRotation = gameObject.transform.rotation;
        startParent = gameObject.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetObj();
        }
        if (gameObject.transform.position.y < yPositionThreshold)
        {
            ResetObj();
        }
    }

    public void ResetObj()
    {
        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        gameObject.transform.position = startPosition;
        gameObject.transform.rotation = startRotation;
        gameObject.transform.parent = startParent;
    }

}
