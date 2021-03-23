using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deform : MonoBehaviour
{
    //public Camera VRCamera;
    public GameObject ball;
    public GameObject wrist;
    public float min;

    bool isHaptic;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(ball.transform.position,transform.position);
        // Debug.Log("Distance: " + distance);

        HapticController haptics = wrist.gameObject.GetComponentInParent<HapticController>();

        if (distance < min)
        {

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            
                BSoftBody hitBody = hit.transform.GetComponentInChildren<BSoftBody>();
                if (hitBody)
                {

                    hitBody.DeformAtPoint(hit.point, hit.normal, 25f, 0.6f);
                    haptics.LoopAllActuators(0, 0.5f);

                    //Debug.Log("HitBodyNmae: " + hitBody.transform.gameObject.name);
                }
                

            }
        }

        else
        {

            //Stop haptics
            haptics.LoopAllActuators(0, 0);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);


        }




        /*   void OnTriggerStay(Collider other)
          {
             // Debug.Log("contact**");
              RaycastHit hit;
              if (Physics.Raycast(transform.position, transform.forward, out hit))
              {
                  Debug.Log("Point of contact: " + hit.point);
              }


          }*/

        /*void OnCollisionEnter(Collision collision)
        {
            Debug.Log("contact**");
            BSoftBody hitBody = hit.transform.GetComponent<BSoftBody>();
            if (hitBody)
                hitBody.DeformAtPoint(hit.point, hit.normal, 35f, 0.7f);
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }
        }*/



    }
}
