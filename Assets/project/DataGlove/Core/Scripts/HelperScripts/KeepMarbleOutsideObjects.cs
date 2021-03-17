using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepMarbleOutsideObjects : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();
    private ResetObject resetObject;

    // Start is called before the first frame update
    void Start()
    {
        resetObject = GetComponent<ResetObject>();
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (GameObject obj in objects)
        {
            if (collision.gameObject.name.Equals(obj.name))
            {
                if (collision.collider.bounds.Contains(gameObject.transform.position))
                {
                    resetObject.ResetObj();
                }
            }
        }
    }

}
