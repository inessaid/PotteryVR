using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {

    public float speed;
    public Vector3 RotationObject;
	
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
	}
}
