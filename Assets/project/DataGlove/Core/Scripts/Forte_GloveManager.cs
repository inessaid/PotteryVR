using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forte_GloveManager : MonoBehaviour
{
    private static Forte_GloveManager _instance;
    [HideInInspector]
    public GloveController leftGlove = null;
    [HideInInspector]
    public GloveController rightGlove = null;

    public static Forte_GloveManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }
}
