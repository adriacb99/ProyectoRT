using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    float cameraZoom;
    float wheelClick;
    Vector2 mousePosition;

    float initialCamPosition;

    [SerializeField ]float sensibility = 1;

    PlayerControls playerControls;

    CinemachineFreeLook FreeLook;

    private void Awake()
    {
        playerControls = new PlayerControls();
        FreeLook = GetComponent<CinemachineFreeLook>();
    }

    private void OnEnable()
    {
        playerControls.Camera.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {
        cameraZoom = playerControls.Camera.CameraZoom.ReadValue<float>();
        wheelClick = playerControls.Camera.CameraRotation.ReadValue<float>();
        mousePosition = playerControls.Camera.MouseMovementPosition.ReadValue<Vector2>();
        Vector2 vector = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
        float mouseXPos = vector.x;

        if(cameraZoom < 0)
        {
            FreeLook.m_YAxis.Value = FreeLook.m_YAxis.Value - 1 / sensibility;
        }
        else if(cameraZoom > 0)
        {
            FreeLook.m_YAxis.Value = FreeLook.m_YAxis.Value + 1 / sensibility;
        }
        else if(wheelClick == 1)
        {
            FreeLook.m_XAxis.m_MaxSpeed = 100; 
        }
        if(wheelClick == 0)
        {
            FreeLook.m_XAxis.m_MaxSpeed = 0;
        }
        Debug.Log("Posicion X del vector del mouse: " + vector.x);
    }
}
