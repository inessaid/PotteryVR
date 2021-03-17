using UnityEngine;
using System.Collections;

public class Raycast : MonoBehaviour {

    public float range;
	
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, range))
            {
                if (hit.transform.tag == "Lever")
                {
                    hit.transform.gameObject.GetComponent<LeverControll>().turn();
                } else if (hit.transform.tag == "Button")
                {
                    hit.transform.gameObject.GetComponent<ButtonControl>().turn();
                } else if (hit.transform.tag == "Code")
                {
                   // hit.transform.gameObject.GetComponent<CodelockControl>().pressed();
                }
            }
        }
    }
}
