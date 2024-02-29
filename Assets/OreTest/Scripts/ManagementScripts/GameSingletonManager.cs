using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSingletonManager : MonoBehaviour
{
    public static GameSingletonManager Instance { get; private set; }

    [HideInInspector] public GameObject playerObject { get; private set; }

    public PlayerMining playerMiningManager { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this; 
    }

    private void Awake()
    {
        playerMiningManager = GetComponentInChildren<PlayerMining>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
