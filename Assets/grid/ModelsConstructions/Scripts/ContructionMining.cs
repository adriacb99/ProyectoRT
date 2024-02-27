using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContructionMining : MonoBehaviour
{
    [SerializeField] private float miningInterval = 1;

    private float time = 0;
    private List<Mineral> mineralsToMine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= miningInterval) {
            time = 0;
            foreach (var min in mineralsToMine) {
                min.GetMineral();
            }
        }
    }
}
