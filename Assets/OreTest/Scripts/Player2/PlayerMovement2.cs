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


    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -3f;

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
        Debug.Log(moveVector);

        if (controller.isGrounded != true)
        {
            verticalVelocity += gravityValue * Time.deltaTime;
        }
        else
        {
            verticalVelocity = 0f;
        }

        controller.Move((transform.position - planetObject.transform.position) * verticalVelocity * Time.deltaTime);
        controller.Move(moveVector * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(transform.position - planetObject.transform.position);

        /*
        playerVelocity.y = verticalVelocity;
        controller.Move(playerVelocity * Time.deltaTime);
        controller.Move(moveVector * Time.deltaTime);*/


















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
