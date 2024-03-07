using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement2 : MonoBehaviour
{
    PlayerControls controls;
    Vector3 moveVector;

    private int grounded = 1;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    public GameObject planetObject;

    float verticalVelocity = 0f;

    private void Awake()
    {
        controls = new PlayerControls();

        controller = GetComponent<CharacterController>();

        
    }

    void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }




    // Update is called once per frame
    void Update()
    {
        //moveVector = controls.Player.Move.ReadValue<Vector2>();

        moveVector = controls.Player.Move.ReadValue<Vector3>();


        //Debug.Log(controller.isGrounded);
        if (controller.isGrounded)
            { verticalVelocity = 0f;
            grounded = 0;
        }
    
        Debug.Log(grounded);

        verticalVelocity += gravityValue * Time.deltaTime;
        
        playerVelocity.y = verticalVelocity;

        Vector3 composedMove = (moveVector * Time.deltaTime) + (-verticalVelocity * Time.deltaTime)  * (planetObject.transform.position - transform.position);
        controller.Move(composedMove);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, this.transform.position  - planetObject.transform.position);



















        /*
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        */



    }



}
