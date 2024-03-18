using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    State state;

    enum State
    {
        WaitingForItem,
        Moving,
        WaitingForDestination,
        ReturningToStart

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) { 
            case State.WaitingForItem:               
                break;
            case State.Moving:
                break; 
            case State.WaitingForDestination:
                break; 
            case State.ReturningToStart:
                break;
            }
    }
}
