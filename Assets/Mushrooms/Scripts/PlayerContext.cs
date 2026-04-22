using System;
using UnityEngine;

public class PlayerContext : MonoBehaviour
{
    [SerializeField] float insanity;
    //audio manager for effects

    public void InsanityChange(float value)
    {
        if (insanity > 0)
        {
            insanity += value;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
