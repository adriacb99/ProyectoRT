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

    float initialCamPosition = 0;

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

        //Debug.Log("posicion X del raton: " + vector.x);
        Debug.Log("Click a la rueda del raton: " + wheelClick);

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
            Debug.Log("Ha entrado en el wheelclick");
            if(initialCamPosition != 0)
            {
                initialCamPosition = mouseXPos;
                Debug.Log("posicion X guardada: " + initialCamPosition);
            }

            if(mousePosition.x > initialCamPosition)
            {
                FreeLook.m_XAxis.Value = FreeLook.m_XAxis.Value + 1 / sensibility;
            }
            else
            {
                FreeLook.m_XAxis.Value = FreeLook.m_XAxis.Value - 1 / sensibility;
            }
            
        }

        //Debug.Log("Posicion vector2 del cursor: " + vector);
      


        //Debug.Log("Valor de YAxis: " + FreeLook.m_YAxis.Value);
        

        //Debug.Log("Valor del camera zoom: " + cameraZoom);
    }
}
