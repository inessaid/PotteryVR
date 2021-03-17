using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteLevelControl : MonoBehaviour
{
    [SerializeField] GameObject[] discreteBars;
    [SerializeField] Sprite onBar;
    [SerializeField] Sprite offBar;

    // start level
    [SerializeField] int level = 3;

    public int Level
    {
        get { return level; }
    }

    void Start()
    {
        for(int i = 0; i < level; i++)
        {
            discreteBars[i].GetComponent<SpriteRenderer>().sprite = onBar;
        }
    }

    // Called when "+" button is pressed
    public void IncreaseLevel()
    {
        level = Mathf.Clamp(level + 1, 0, 8);
        discreteBars[level - 1].GetComponent<SpriteRenderer>().sprite = onBar;

    }

    // Called when "-" button is pressed
    public void DecreaseLevel()
    {
        level = Mathf.Clamp(level - 1, 0, 7);
        discreteBars[level].GetComponent<SpriteRenderer>().sprite = offBar;

    }

    
}
