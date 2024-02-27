using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prueba : MonoBehaviour
{
    [SerializeField] AtmosphereSettings atm;
    [SerializeField] Material atmMaterial;
    // Start is called before the first frame update
    void Start()
    {
        atm.SetProperties(atmMaterial, 5);
    }

    // Update is called once per frame
    void Update()
    {
        atm.SetProperties(atmMaterial, 5);
    }
}
