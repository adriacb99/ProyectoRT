using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSingleton : MonoBehaviour
{
    public static CanvasSingleton Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject GetCanvas(int index) {
        int i = 0;
        foreach (RectTransform child in transform) {
            if (i == index) return child.gameObject;
            i++;
        }
        return null;
    }
}
