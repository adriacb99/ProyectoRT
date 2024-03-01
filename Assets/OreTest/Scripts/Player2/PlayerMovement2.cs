using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement2 : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] float gravity;
    [SerializeField] float movementSpeed;

    private CharacterController characterController;


    // Start is called before the first frame update
    void Start()
    {
       characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        
    }
    
}
