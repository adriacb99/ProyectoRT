using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OreSingleton : MonoBehaviour
{
    
    [HideInInspector] public GameObject ore;
    //[SerializeField] ItemData item;

    private void Awake()
    {
        
    }

    public void debug()
    {
        ore = this.gameObject;

        Debug.Log(ore.name);
    }

    public void CallPlayerToMineOre()
    {
        Debug.Log("has clickado la mineral");
        GameSingletonManager.Instance.playerMiningManager.mineOre(this.gameObject);
    }

    
}
