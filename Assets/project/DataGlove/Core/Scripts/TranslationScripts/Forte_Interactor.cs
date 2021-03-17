using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataGlove;

// Place this module on the same game object as a GloveController to interact with Forte_Interactable objects
public class Forte_Interactor : MonoBehaviour
{
    public bool highlightingOn = true;
    public float lerpTime = 0.6f;
    private Poses poses = null;
    [HideInInspector]
    public bool isGrabbing = false;

    private GloveController gloveController = null;
    private List<Forte_Interactable> nearbyObjects = new List<Forte_Interactable>();
    private Forte_Interactable activeObject = null;
    private Transform handParent = null;
    private Quaternion handStartRot;
    private Vector3 handStartPos;
    private Vector3 handStartScale;
    private bool isFrozen = false;
    private float lerpElapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        gloveController = gameObject.GetComponent<GloveController>();
        if (gloveController == null)
        {
            Debug.Log("Error: No Glove Controller found near the Interactor module");
        }
        poses = gameObject.AddComponent<Poses>();
        handStartScale = transform.localScale;
        handStartRot = transform.localRotation;
        handStartPos = transform.localPosition;
        handParent = transform.parent;
    }

    private void Update()
    {        
        // Don't switch objects while holding an object
        if (nearbyObjects.Count > 0 && !isGrabbing)
        {
            // The new active object is the closest colliding Forte_Interactable object
            Forte_Interactable closestObj = FindClosestObj(nearbyObjects);
            if (closestObj != activeObject)
            {
                SwitchToObject(closestObj);
            }
        }

        LerpToOrigin();

        if (activeObject == null)
        {
            isGrabbing = false;
            return;
        }
        if(!activeObject.gameObject.activeSelf)
        {
            isGrabbing = false;
            nearbyObjects.Remove(activeObject);
            activeObject = null;
            return;
        }
        if (isGrabbing && activeObject.isGrabbed && gloveController.rightHandGlove != activeObject.grabHandRight)
        {
            //Stolen from other hand
            isGrabbing = false;
            nearbyObjects.Remove(activeObject);
            activeObject = null;
            return;
        }

        bool grab = CalcIsGrabbingPinch(activeObject);
        if (grab && !isGrabbing)
        {
            // Grab Event
            activeObject.EnterGrab(gameObject);
            isGrabbing = true;
            poses.freezePose = gloveController.GetSensors();
            poses.fingersToFreeze = (bool[]) activeObject.GetFingersColliding().Clone();
        }
        else if (!grab && isGrabbing)
        {
            // Release Event
            activeObject.ExitGrab(gameObject);
            isGrabbing = false;

            nearbyObjects.Remove(activeObject);
            activeObject.Highlight(false);

            activeObject = null;
        }
    }


    private void LerpToOrigin()
    {
        if (lerpElapsed < lerpTime)
        {
            lerpElapsed += Time.deltaTime;

            if (transform.localRotation != handStartRot && !isFrozen && lerpElapsed < lerpTime)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, handStartRot, lerpElapsed / lerpTime);
            }
            else if (!isFrozen && lerpElapsed >= lerpTime)
            {
                transform.localRotation = handStartRot;
            }
            if (transform.localPosition != handStartPos && !isFrozen)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, handStartPos, lerpElapsed / lerpTime);
            }
            else if (!isFrozen && lerpElapsed >= lerpTime)
            {
                transform.localPosition = handStartPos;
            }
        }
    }


    // The closest object has changed to a new object or to null if there are no near objects
    private void SwitchToObject(Forte_Interactable closestObj)
    {
        if (activeObject != null)
        {
            activeObject.Highlight(false);
        }
            
        activeObject = closestObj;

        if (highlightingOn)
        {
            activeObject.Highlight(true);
        }
    }

    // Determine the closest object in the nearby objects list
    Forte_Interactable FindClosestObj(List<Forte_Interactable> nearby)
    {
        int nearestIndex = -1;
        float nearestDistance = 1000;

        // Remove null items
        nearby.RemoveAll(item => item == null);

        for (int i = 0; i < nearby.Count; i++)
        {
            float dist = Vector3.Distance(nearby[i].transform.position, transform.position);

            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestIndex = i;
            }
            nearby[i].Highlight(false);
        }

        if (nearestIndex == -1)
        {
            Debug.Log("Error: No Forte_Interactable object found");
            return null;
        }
        if (highlightingOn)
        {
            nearby[nearestIndex].Highlight(true);
        }

        return nearby[nearestIndex];
    }

    // Returns the current active object
    public Forte_Interactable GetActiveObject() {
        return activeObject;
    }

    // Determine if the finger values are curled enough to pick up the object
    private bool CalcIsGrabbing(Forte_Interactable nearbyObj)
    {
        float activeSum = 0;
        int numActiveFingers = 0;
        FingerVals[] fingers = gloveController.GetSensors();

        if (nearbyObj == null)
        {
            return false;
        }

        // Some object use a sum from a subset of fingers, such as the pinch pose 
        // that only uses the thumb and index fingers when determining when to start a pick up event
        for (int i = 0; i < nearbyObj.fingersUsedToPickUp.Length; i++)
        {
            if (nearbyObj.fingersUsedToPickUp[i])
            {
                numActiveFingers++;

                activeSum += fingers[i].sensor1;
                activeSum += fingers[i].sensor2;
            }
        }
        activeSum = activeSum / (numActiveFingers * 2.0f);

        return (activeSum >= nearbyObj.pickUpThreshold);
    }

    // Determine if the finger values are curled enough to pick up the object
    private bool CalcIsGrabbingPinch(Forte_Interactable nearbyObj)
    {
        if (nearbyObj == null) 
        {
            //Debug.Log("Nearby Obj == Null");
            return false;
        }

        if (!isGrabbing)
        {
            if (nearbyObj.GetCollisionCount() < 2 && gloveController.rightHandGlove == true)
            {
                //Debug.Log(nearbyObj.gameObject.name + "Less than two collisions");
            }
            if (!nearbyObj.GetThumbColliding() && gloveController.rightHandGlove == true)
            {
                //Debug.Log(nearbyObj.gameObject.name + "Thumb Not Colliding");
            }
            if (nearbyObj.GetCollisionCount() < 2 || !nearbyObj.GetThumbColliding())
            {
                return false;
            }
        }

        bool fingerPinch = false;
        FingerVals[] fingers = gloveController.GetSensors();
        bool[] fingersColliding = nearbyObj.GetFingersColliding();
        float[] fingerDeltas = gloveController.GetFingerDeltas();

        // A pinch requires a thumb collision and for at least one other colliding finger to curl
        for (int i = 1; i < 5; i++)
        {
            float fingerSum = (fingers[i].sensor1 + fingers[i].sensor2) / 2;
            
            if (nearbyObj.pose == Poses.Pose.Freeze)
            {
                // Special handling for picking up objects with pinch and freeze pose
                if (fingerSum >= nearbyObj.pickUpThreshold && fingersColliding[i])
                {
                    fingerPinch = true;
                    break;
                }
            }
            else
            {
                if (fingerSum >= nearbyObj.pickUpThreshold)
                {
                    fingerPinch = true;
                    break;
                }
            }
        }
        return (fingerPinch);
    }

    private void OnTriggerStay(Collider other)
    {
        Forte_Interactable interactable = other.GetComponent<Forte_Interactable>();

        if (interactable != null)
        {
            if (other == interactable.hoverCollider && !nearbyObjects.Contains(interactable))
            {
                nearbyObjects.Add(interactable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Forte_Interactable interactable = other.GetComponent<Forte_Interactable>();

        if (interactable != null)
        {
            if (other == interactable.hoverCollider)
            {
                if (interactable == activeObject && isGrabbing)
                {
                    // Dont remove the currently grabbed object
                    return;
                }
                if (interactable == activeObject && gloveController.rightHandGlove == activeObject.grabHandRight)
                {
                    interactable.ExitGrab(gameObject);
                }

                interactable.Highlight(false);
                nearbyObjects.Remove(interactable);
                if (nearbyObjects.Count == 0)
                {
                    activeObject = null;
                }
            }
        }
    }

    public void FreezeHand(GameObject newParent)
    {
        isFrozen = true;
        transform.SetParent(newParent.transform, true);
    }

    public void UnfreezeHand()
    {
        isFrozen = false;
        lerpElapsed = 0;
        transform.SetParent(handParent, true);
        transform.localScale = handStartScale;
    }
}
