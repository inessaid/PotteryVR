using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    public Switch switchController;

    private void OnTriggerEnter(Collider other)
    {
        switchController.Flick(this, other.gameObject);
    }
}
