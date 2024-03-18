using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour
{
    public delegate void OnTick();
    public static event OnTick onTick;

    [SerializeField] float tickTime = 0.1f;

    private float time;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= tickTime)
        {
            time = 0;
            if (onTick != null) onTick();
        }
    }
}
