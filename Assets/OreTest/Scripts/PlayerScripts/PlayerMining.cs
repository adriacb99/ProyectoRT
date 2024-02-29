using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMining : MonoBehaviour
{
    const int maxOres = 100;

    private bool isOreInRange = false;
    private GameObject oreObject;
    private PhysicsRaycaster raycaster;
    private Collider oreCollider;
    [SerializeField] List<GameObject> ores = new List<GameObject>(maxOres);

    // Start is called before the first frame update
    void Start()
    {
    
    }

    private void Awake()
    {
       
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void mineOre(GameObject ore)
    {
        Debug.Log("Se ha clickado encima del ore");

        oreObject = ore;

        var index = ores.IndexOf(oreObject);

        if (isOreInRange && index != -1)
        {
            RemoveFromList(oreObject);
            Destroy(oreObject, 0.01f);

            Debug.Log("Objeto destruido: " + oreObject.name);   
            
        }
    }
     
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("El personaje y el ore han entrado en contacto");
        isOreInRange = true;
        oreObject = other.gameObject;

        if(oreObject.CompareTag("Ore")) //Comprueba si el objeto con el que ha colisionado es un ore a través de su tag
        {
            ores.Add(oreObject);
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("El personaje y el ore han dejado de estar en contacto");
        isOreInRange = false;
        RemoveFromList(other.gameObject);
    }

    private void RemoveFromList(GameObject objectToRemove)
    {
        ores.Remove(objectToRemove);
    }

}
