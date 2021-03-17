using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// Drag this component onto any game object in order to trigger a hand pose, highlight, 
// or pick-up event by a Forte_Interactor object
public abstract class Forte_Interactable : MonoBehaviour
{
    public Poses.Pose pose;
    public float range = 0.5f;
    public float transitionRate = 0.5f;
    public float pickUpThreshold = 0.2f;
    public bool[] fingersUsedToPickUp = { true, true, true, true, true }; // Selects which fingers are used when detecting a pick up event
    public Collider hoverCollider;
    public bool collisionHapticsOn = true;
    [HideInInspector]
    public bool isGrabbed = false;
    [HideInInspector]
    public bool grabHandRight = false;
    public int hapticWave = 2;
    public float hapticAmp = 1;
    public float hapticGrainFade = 0;
    public float hapticGrainLocation = 0;
    public float hapticGrainSize = 0.03f;
    public int hapticPitch = 0;
    [HideInInspector]
    public HapticSettings hapticSettings;
    [HideInInspector]
    public GameObject grabbingHand = null;

    private Material originalMat;
    private Material highlightMat = null;
    private Renderer objectRenderer;
    private int collisionCount = 0; // Number of collisions with grabbing hand
    private bool thumbIsColliding = false;
    private bool[] fingersColliding = { false, false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (hoverCollider == null)
        {
            if (col != null)
            {
                hoverCollider = col;
            }
            else if (col != null)
            {
                // Copy collider and make trigger
                hoverCollider = CopyHover(col);
                hoverCollider.isTrigger = true;

                if (hoverCollider == null)
                {
                    // Copy failed, make one
                    SphereCollider sphereCol = gameObject.AddComponent<SphereCollider>();
                    sphereCol.radius = range * (1f / gameObject.transform.localScale.x);
                    hoverCollider = sphereCol;
                }
            }
            else
            {
                // No Collider found, make one
                SphereCollider sphereCol = gameObject.AddComponent<SphereCollider>();
                sphereCol.radius = range * (1f / gameObject.transform.localScale.x);
                hoverCollider = sphereCol;
            }
        }

        hoverCollider.isTrigger = true;

        highlightMat = (Material)Resources.Load("Highlight", typeof(Material));
        if (highlightMat == null)
        {
            Debug.Log("Can't find highlight material!");
        }

        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }
        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInParent<Renderer>();
        }

        originalMat = new Material(objectRenderer.material);

        hapticSettings = new HapticSettings
        {
            waveform = hapticWave,
            amplitude = hapticAmp,
            grainFade = hapticGrainFade,
            grainLocation = hapticGrainLocation,
            grainSize = hapticGrainSize,
            pitch = hapticPitch
        };
    }

    // Adds a special highlight material if highlighting is on in the Forte_Interactable script
    public void Highlight(bool isHighlighted)
    {
        //if (isHighlighted)
        //{
        //    objectRenderer.material = highlightMat;
        //}
        //else
        //{
        //    objectRenderer.material = originalMat;
        //}
    }

    // Plays haptics when the object is touched
    private void OnCollisionEnter(Collision collision)
    {
        if (collisionHapticsOn)
        {
            StartPulse(collision.collider.gameObject);
        }
    }

    // Plays haptics when the object is touched if the collider is in trigger mode
    private void OnTriggerEnter(Collider other)
    {
        if (isGrabbed && other.isTrigger == false)
        {
            GrabCollisions();
        }

        GloveController controller = other.GetComponentInParent<GloveController>();
        if (controller != null)
        {
            if (!IsTipCollider(other, controller))
            {
                return;
            }

            bool[] fingersCollidingCopy = (bool[])fingersColliding.Clone();
            if (other.gameObject.name.Contains("thumb") || other.gameObject.name.Contains("Thumb") || other.gameObject.name.Contains("THUMB"))
            {
                thumbIsColliding = true;
                fingersColliding[0] = true;
                collisionCount++;
            }
            if (other.gameObject.name.Contains("index") || other.gameObject.name.Contains("Index") || other.gameObject.name.Contains("INDEX"))
            {
                fingersColliding[1] = true;
                collisionCount++;
            }
            if (other.gameObject.name.Contains("middle") || other.gameObject.name.Contains("Middle") || other.gameObject.name.Contains("MIDDLE"))
            {
                fingersColliding[2] = true;
                collisionCount++;
            }
            if (other.gameObject.name.Contains("ring") || other.gameObject.name.Contains("Ring") || other.gameObject.name.Contains("RING"))
            {
                fingersColliding[3] = true;
                collisionCount++;
            }
            if (other.gameObject.name.Contains("pinky") || other.gameObject.name.Contains("Pinky") || other.gameObject.name.Contains("PINKY"))
            {
                fingersColliding[4] = true;
                collisionCount++;
            }
            FreezeNewFingers(controller, fingersCollidingCopy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GloveController controller = other.GetComponentInParent<GloveController>();
        if (controller != null)
        {
            if (!IsTipCollider(other, controller)) { return; }

            if (other.gameObject.name.Contains("thumb") || other.gameObject.name.Contains("Thumb") || other.gameObject.name.Contains("THUMB"))
            {
                thumbIsColliding = false;
                fingersColliding[0] = false;
                collisionCount--;
            }
            if (other.gameObject.name.Contains("index") || other.gameObject.name.Contains("Index") || other.gameObject.name.Contains("INDEX"))
            {
                fingersColliding[1] = false;
                collisionCount--;
            }
            if (other.gameObject.name.Contains("middle") || other.gameObject.name.Contains("Middle") || other.gameObject.name.Contains("MIDDLE"))
            {
                fingersColliding[2] = false;
                collisionCount--;
            }
            if (other.gameObject.name.Contains("ring") || other.gameObject.name.Contains("Ring") || other.gameObject.name.Contains("RING"))
            {
                fingersColliding[3] = false;
                collisionCount--;
            }
            if (other.gameObject.name.Contains("pinky") || other.gameObject.name.Contains("Pinky") || other.gameObject.name.Contains("PINKY"))
            {
                fingersColliding[4] = false;
                collisionCount--;
            }
        }
    }

    // Plays a haptic pulse on a finger for .1 seconds
    public void StartPulse(GameObject colliderObj)
    {
        GloveController controller = colliderObj.GetComponentInParent<GloveController>();
        if (controller != null)
        {
            // We need to grab this each time because there can be two different hands in the scene
            HapticController haptics = controller.gameObject.GetComponent<HapticController>();

            if (haptics != null)
            {
                haptics.PulseOnFingerSettings(colliderObj, hapticSettings, 0.15f);
            }
        }
        else if (isGrabbed)
        {
            HapticController haptics = grabbingHand.GetComponent<HapticController>();
            haptics.HapticPulse(0, hapticSettings, 0.15f);
        }
    }

    // This is what occurs on a fingers actautor when that finger touches the interactable. Can be overridden.
    public virtual void TouchHaptic(HapticController haptics, int actuator)
    {
        haptics.HapticPulse(actuator, 0.1f);
    }

    public bool IsNotColliding
    {
        get { return collisionCount == 0; }
    }

    public int GetCollisionCount() {
        return collisionCount;
    }

    public bool GetThumbColliding() {
        return thumbIsColliding;
    }

    public bool[] GetFingersColliding() {
        return fingersColliding;
    }

    public virtual void EnterGrab(GameObject hand)
    {

    }

    public virtual void ExitGrab(GameObject hand)
    {

    }

    public void ClearCollisions()
    {
        collisionCount = 0;
        thumbIsColliding = false;

        for (int i = 0; i < fingersColliding.Length; i++)
        {
            fingersColliding[i] = false;
        }
    }

    // If no hover collider is provided, the normal object collider will be copied and made into a trigger collider
    private Collider CopyHover(Collider col)
    {
        if (col.GetType() == typeof(SphereCollider))
        {
            SphereCollider colOriginal = gameObject.GetComponent<SphereCollider>();
            SphereCollider colCopy = gameObject.AddComponent<SphereCollider>();
            colCopy.radius = colOriginal.radius;
            colCopy.center = colOriginal.center;
            return colCopy;
        }
        if (col.GetType() == typeof(BoxCollider))
        {
            BoxCollider colOriginal = gameObject.GetComponent<BoxCollider>();
            BoxCollider colCopy = gameObject.AddComponent<BoxCollider>();
            colCopy.size = colOriginal.size;
            colCopy.center = colOriginal.center;
            return colCopy;
        }
        if (col.GetType() == typeof(CapsuleCollider))
        {
            CapsuleCollider colOriginal = gameObject.GetComponent<CapsuleCollider>();
            CapsuleCollider colCopy = gameObject.AddComponent<CapsuleCollider>();
            colCopy.radius = colOriginal.radius;
            colCopy.center = colOriginal.center;
            colCopy.height = colOriginal.height;
            colCopy.direction = colOriginal.direction;
            return colCopy;
        }
        if (col.GetType() == typeof(MeshCollider))
        {
            // Warning Only convex meshes can be set to trigger
            MeshCollider colOriginal = gameObject.GetComponent<MeshCollider>();
            MeshCollider colCopy = gameObject.AddComponent<MeshCollider>();
            colCopy.sharedMesh = colOriginal.sharedMesh;
            colCopy.convex = true;
            return colCopy;
        }
        return null;
    }

    // Determine if a collider belongs to a fingertip
    private bool IsTipCollider(Collider col, GloveController controller)
    {
        GameObject fingerSection = col.gameObject;
        if (fingerSection == controller.middleJoints[0])
        {
            return true;
        }

        for (int i = 1; i < 5; i++)
        {
            if (fingerSection == controller.tipJoints[i])
            {
                return true;
            }
        }

        return false;
    }

    void FreezeNewFingers(GloveController controller, bool[] fingersLastColliding)
    {
        Poses poseModule = controller.gameObject.GetComponent<Poses>();
        DataGlove.FingerVals[] fingerVals = controller.GetSensors();

        if (poseModule.freezePose.Length < 1) { return; }

        for (int i = 0; i < 5; i++)
        {
            if (fingersColliding[i] && !fingersLastColliding[i])
            {
                // A free finger has collided with the object and should now be frozen
                poseModule.freezePose[i] = fingerVals[i];
            }
        }
        poseModule.fingersToFreeze = fingersColliding;
    }

    public virtual void GrabCollisions()
    {

    }
}
