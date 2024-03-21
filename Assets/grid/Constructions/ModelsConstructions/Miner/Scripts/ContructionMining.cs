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
    [SerializeField] GameObject dustPS;

    [Header("Canvas")]
    [SerializeField] public GameObject canvas;


    private float time = 0;
    private List<Construction> mineralsToMine;
    private List<ConstructionMineral> minerals;

    [SerializeField] public ItemData item;

    bool stopped = true;

    private void Start()
    {
        SetMinerals(tile.GetSideTilesObjects());
    }

    public void SetMinerals(List<Construction> lista)
    {
        minerals = new List<ConstructionMineral>();
        mineralsToMine = lista;
        foreach (Construction obj in mineralsToMine)
        {
            if (obj != null && obj.CompareTag("Mineral")) { 
                minerals.Add(obj.GetComponent<ConstructionMineral>());
                Instantiate(dustPS, obj.tile.GetPosition(), obj.transform.rotation);
            } 
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
                        min.GetMineral(GetComponent<ItemsInventory>());
                    }
                }
            }
        }
    }

    public void ShowMenu() {
        //Debug.Log("Abrir Menu");
        // canvas = GameObject.Find("Canvas");
        // canvas.transform.GetChild(0).gameObject.SetActive(true);
        //canvas.GetComponentInChildren<TextMeshProUGUI>().text = numMin.ToString();
    }

    public override ItemData GetItemFromConstruction()
    {
        return GetComponent<ItemsInventory>().RetireItemToSlot(0, 1);
    }
}
