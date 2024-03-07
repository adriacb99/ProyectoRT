using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlanetGravity planet;
    
    private Transform playerTransform;
    private Rigidbody playerRb;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
        playerRb = GetComponent<Rigidbody>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        planet.AttractPlayer(gameObject);
    }
}
