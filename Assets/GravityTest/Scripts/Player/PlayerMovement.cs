using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlanetGravity planet;
    
    private Transform playerTransform;
    private Rigidbody playerRb;

    
    [SerializeField] float characterSpeed;

    PlayerControls playerControls;

    Vector3 movementVectorX;
    Vector3 movementVectorY;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
        playerRb = GetComponent<Rigidbody>();

        
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        movementVectorX = playerControls.Player.Move.ReadValue<Vector2>().y * transform.forward;
        movementVectorY = playerControls.Player.Move.ReadValue<Vector2>().x * transform.right;

        Debug.Log(movementVectorX);

        //playerRb.velocity = movementVector * characterSpeed * Time.fixedDeltaTime;

        planet.AttractPlayer(gameObject);


    }

    void FixedUpdate()
    {
        moveCharacter(movementVectorX + movementVectorY);
    }

    void moveCharacter(Vector3 direction)
    {
        // We multiply the 'speed' variable to the Rigidbody's velocity...
        // and also multiply 'Time.fixedDeltaTime' to keep the movement consistant on all devices
        playerRb.velocity = direction.normalized * characterSpeed * Time.fixedDeltaTime;
        
        
    }
}
