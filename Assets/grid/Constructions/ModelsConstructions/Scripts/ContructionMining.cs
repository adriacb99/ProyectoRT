using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;

public class ContructionMining : MonoBehaviour
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

    public static GameObject PlaceConstruction(Transform parent, GameObject obj, Vector3 position, List<GameObject> sideObjects, Quaternion q)
    {
        GameObject construction = Instantiate(obj, position, q);
        construction.transform.parent = parent;
        construction.GetComponent<ContructionMining>().SetMinerals(sideObjects);
        return construction;
    }

    public void SetMinerals(List<GameObject> list)
    {
        minerals = new List<Mineral>();
        mineralsToMine = list;
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
                        min.GetMineral();
                        stopped = !GetComponent<MinerInventory>().AddItem(new ItemInstance(item));
                    }
                }
            }
        }
        //else helix.GetComponent<Renderer>().material.SetInt("Stopped", 0);
    }

    public void ShowMenu() {
        Debug.Log("Abrir Menu");
        // canvas = GameObject.Find("Canvas");
        // canvas.transform.GetChild(0).gameObject.SetActive(true);
        // canvas.GetComponentInChildren<TextMeshProUGUI>().text = numMin.ToString();
    }
}
