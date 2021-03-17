using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataGlove;

/// allows for the picking up, dropping, and throwing of game objects.
/**
 * This script gets dynamically instantiated by the Forte_Interactable script --
 * It allows for the picking up, dropping, and throwing of game objects
 */
public class PickUpObjects : Forte_Interactable
{

    public bool grabHapticsOn = false;
    public bool heldObjectHapticOn = true;
    private List<Collider> colliders = new List<Collider>();
    private float colliderTimer = 0;
    private float COL_DISABLE_TIME = 0.4f;

    private void Update()
    {
        if (isGrabbed && IsNotColliding)
        {
            ExitGrab(null);
        }

        // Re-enable the colliders after the disable period
        if (colliders.Count > 0)
        {
            colliderTimer += Time.deltaTime;

            if (colliderTimer > COL_DISABLE_TIME)
            {
                colliderTimer = 0;

                for (int i = 0; i < colliders.Count; i++)
                {
                    colliders[i].enabled = true;
                }

                colliders.Clear();
            }
        }
    }

    /// A grab is attempted. 
    /**
     * If a valid item is near enough the the item is 
     * picked up.
     */
    public override void EnterGrab(GameObject hand)
    {
        grabbingHand = hand;
        transform.parent = hand.transform;

        GetComponent<Rigidbody>().isKinematic = true;

        if (grabHapticsOn)
        {
            for (int i = 0; i < 6; i++)
            {
                hand.GetComponent<HapticController>().HapticPulse(i, hapticSettings, 0.1f);
            }
        }

        isGrabbed = true;

        grabHandRight = hand.GetComponent<GloveController>().rightHandGlove;

    }

    /// When a grab ends, the item must drop or be thrown.
    /**
     */
    public override void ExitGrab(GameObject hand)
    {
        isGrabbed = false;

        GetComponent<Rigidbody>().isKinematic = false;

        transform.parent = null;

        if (grabbingHand == null)
        {
            return;
        }

        DisableColliders();

        // Throw the object
        Vector3 throwVelocity = grabbingHand.GetComponent<GloveController>().GetPalmVelocity();

        // Determine if the object is being thrown straight up.
        if (Mathf.Abs(throwVelocity.x) + Mathf.Abs(throwVelocity.z) < throwVelocity.y)
        {
            throwVelocity.x = 0;
            throwVelocity.z = 0;
        }

        grabbingHand = null;

        if (throwVelocity.magnitude < 0.4)
        {
            return;
        }

        GetComponent<Rigidbody>().velocity = throwVelocity;// * 2;

    }

    public override void GrabCollisions()
    {
        if (heldObjectHapticOn)
        {
            HapticController haptics = grabbingHand.GetComponent<HapticController>();
            haptics.HapticPulse(0, hapticSettings, 0.15f);
        }
    }

    /// Limits the max throw velocity so that objects don't get thrown too far.
    /**
     */
    Vector3 LimitThrow(Vector3 velocity, float maxValue)
    {
        Vector3 newThrow;

        newThrow.x = Mathf.Clamp(velocity.x, -maxValue, maxValue);
        newThrow.y = Mathf.Clamp(velocity.y, -maxValue, maxValue);
        newThrow.z = Mathf.Clamp(velocity.z, -maxValue, maxValue);

        return newThrow;
    }

    // Disable the grab hand's colliders after a grab so that the fingers don't block the object
    private void DisableColliders()
    {
        GloveController controller = grabbingHand.GetComponent<GloveController>();

        if (!controller)
        {
            return;
        }

        colliders.Clear();

        for (int i = 0; i < 5; i++)
        {
            Collider knuckleCol = controller.knuckleJoints[i].GetComponent<Collider>();
            Collider middleCol = controller.middleJoints[i].GetComponent<Collider>();

            knuckleCol.enabled = false;
            colliders.Add(knuckleCol);

            middleCol.enabled = false;
            colliders.Add(middleCol);

            if (i != 0)
            {
                Collider tipCol = controller.tipJoints[i].GetComponent<Collider>();
                tipCol.enabled = false;
                colliders.Add(tipCol);
            }
        }

        ClearCollisions();
    }
}
