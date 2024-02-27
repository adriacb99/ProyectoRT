using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [SerializeField] float RotationSpeed = 0;
    // Start is called before the first frame updateç

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rot = RotationSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, rot, 0), Space.Self);
    }
}
