using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem; 
//using UnityEngine.InputSystem.XR; Falta input systema

public class NewBehaviourScript : MonoBehaviour
{
    //Mensaje para push

    [Header("Movement settings")]
    [SerializeField] float gravity;
    [SerializeField] float movementSpeed;

    [Header("Game camera")]
    Camera mainCamera;

    [Header("Input Action")]
  //  [SerializeField] InputActionReference move;

    private CharacterController characterController;


    Vector3 currentVelocity = Vector3.zero;
    Vector3 smoothMoveXZLocal = Vector3.zero;
    

    public GameObject target;

    float verticalVelocity = 0f;

    public GameObject planetObject;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
    //    move.action.Enable(); Falta input systema
    }
    private void OnDisable()
    {
        //  move.action.Disable(); Falta input systema
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();  
    }

    private void UpdateMovement()
    {
        //Vector2 horizontalMovement = move.action.ReadValue<Vector2>(); Falta input system
        //Vector3 moveXZ = new Vector3(horizontalMovement.x, 0f, horizontalMovement.y);
        Vector3 forward = mainCamera.transform.forward;
        forward.Normalize();
        forward.y = 0f;
        //Vector3 moveXZ = forward * horizontalMovement.y + Falta input system
        //               mainCamera.transform.right * horizontalMovement.x; Falta input system
        //currentVelocity = moveXZ * movementSpeed; Falta input system

        if (characterController.isGrounded != true)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity = 0f;
        }



        characterController.Move(Time.deltaTime * currentVelocity);
        characterController.Move((transform.position - planetObject.transform.position) * verticalVelocity * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(transform.position - planetObject.transform.position);
        //characterController.Move(Vector3.MoveTowards(this.transform.position, planetObject.transform.position, Time.deltaTime) * verticalVelocity * Time.deltaTime);

    }

    void UpdateVerticalMovement()
    {
        verticalVelocity += gravity * Time.deltaTime;
        //characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        if (characterController.isGrounded) { verticalVelocity = 0f; }
        
    }
}
