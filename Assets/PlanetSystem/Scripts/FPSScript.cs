using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSScript : MonoBehaviour
{
    /// <summary>
    /// Delta time
    /// </summary>
    float deltaTime = 0.0f;

    /// <summary>
    /// It will be used for printing out fps text on screen
    /// </summary>
    public TextMeshProUGUI text;

    void Start()
    {
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        text.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    }
}
