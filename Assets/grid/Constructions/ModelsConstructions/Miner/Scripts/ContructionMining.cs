using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;

public class ContructionMining : Construction
{
    [SerializeField] private float miningInterval = 1;
    [SerializeField] private GameObject helix;
    [SerializeField] private int ConsumoEnergia = 0;
    
    [Header("Canvas")]
    [SerializeField] public GameObject canvas;


    private float time = 0;
    private List<GameObject> mineralsToMine;
    private List<Mineral> minerals;

    [SerializeField] public ItemData item;

    bool stopped = true;

    public void SetMinerals(List<GameObject> lista)
    {
        minerals = new List<Mineral>();
        mineralsToMine = lista;
        foreach (GameObject obj in mineralsToMine)
        {
            if (obj != null && obj.CompareTag("Mineral")) { minerals.Add(obj.GetComponent<Mineral>()); } 
        }
        stopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped)
        {
            time += Time.deltaTime;
            if (time >= miningInterval)
            {
                time = 0;
                foreach (var min in minerals)
                {
                    if (!stopped)
                    {
                        //min.GetMineral();  *para arreglar mas tarde*
                        stopped = !GetComponent<Inventory>().AddItem(new ItemInstance(item));
                    }
                }
            }
        }
        //else helix.GetComponent<Renderer>().material.SetInt("Stopped", 0);
    }

    public void ShowMenu() {
        //Debug.Log("Abrir Menu");
        // canvas = GameObject.Find("Canvas");
        // canvas.transform.GetChild(0).gameObject.SetActive(true);
        //canvas.GetComponentInChildren<TextMeshProUGUI>().text = numMin.ToString();
    }
}
