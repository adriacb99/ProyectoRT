using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlanetGravity planet;
    
    private Transform playerTransform;
    private Rigidbody playerRb;

    [SerializeField] Vector3 movementVector;
    [SerializeField] float characterSpeed;

    PlayerControls playerControls;
    

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
        playerRb = GetComponent<Rigidbody>();

        playerControls = new PlayerControls();
    }

    // Update is called once per frame
    void Update()
    {
        movementVector = playerControls.Player.Move.ReadValue<Vector3>();


        planet.AttractPlayer(gameObject);

        
    }

    void FixedUpdate()
    {
        moveCharacter(movementVector); 
    }

    void moveCharacter(Vector3 direction)
    {
        // We multiply the 'speed' variable to the Rigidbody's velocity...
        // and also multiply 'Time.fixedDeltaTime' to keep the movement consistant on all devices
        playerRb.velocity = direction * characterSpeed * Time.fixedDeltaTime;
    }
}
