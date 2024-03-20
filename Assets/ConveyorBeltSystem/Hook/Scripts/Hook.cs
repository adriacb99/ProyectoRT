using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Hook : Construction
{
    [SerializeField] int rotationTicks;
    private int ticks;

    public LayerMask layerMask;

    private Planet.Tile tileOut;
    private Planet.Tile tileIn;
    private Quaternion rotationOut;
    private Quaternion rotationIn;
    //private Planet planet;

    [SerializeField] private GameObject pivot;

    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] BeltItemsManager beltItemsManager;

    private State state;
    enum State
    {
        WaitingForItem,
        Moving,
        WaitingForDestination,
        ReturningToStart,
        SetTileOut,
        SetTileIn
    }

    enum OutMode
    {
        ItemConstruction,
        ItemBelt
    }

    // Start is called before the first frame update
    void Start()
    {
        state = State.SetTileOut;
        TimeTickSystem.onTick += UpdateHook;
    }

    // Update is called once per frame
    void UpdateHook()
    {
        switch (state) {
            case State.WaitingForItem:
                ItemData item = tileOut.tileObject.GetItemFromConstruction();
                if (tileOut.tileObject != null && item != null)
                {
                    Debug.Log("Item cogido");
                    state = State.Moving;
                }
                state = State.Moving;
                break;
            case State.Moving:
                ticks++;
                pivot.transform.rotation = Quaternion.Lerp(rotationOut, rotationIn, ticks / (float)rotationTicks);
                if (ticks >= rotationTicks)
                {
                    ticks = 0;
                    state = State.WaitingForDestination;
                }
                break; 
            case State.WaitingForDestination:
                state = State.ReturningToStart;
                break; 
            case State.ReturningToStart:
                ticks++;
                pivot.transform.rotation = Quaternion.Lerp(rotationIn, rotationOut, ticks / (float)rotationTicks);
                if (ticks >= rotationTicks)
                {
                    ticks = 0;
                    state = State.WaitingForItem;
                }
                break;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.SetTileOut:
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Planet.Tile tile = hit.collider.GetComponent<Planet>().GetGrid().GetValue(hit.triangleIndex);
                        transform.position = tile.GetPosition();
                        rotationOut = Quaternion.LookRotation((tile.GetPosition() - transform.parent.position).normalized, hit.normal);
                        pivot.transform.rotation = rotationOut;
                        tileOut = tile;

                        Debug.Log("Tile Out: " + tile);
                        state = State.SetTileIn;
                    }
                }

                break;
            case State.SetTileIn:

                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 1000f, layerMask))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Planet.Tile tile = hit.collider.GetComponent<Planet>().GetGrid().GetValue(hit.triangleIndex);
                        rotationIn = Quaternion.LookRotation((tile.GetPosition() - transform.parent.position).normalized, hit.normal);
                        tileIn = tile;

                        Debug.Log("Tile In: " + tile);
                        state = State.WaitingForItem;
                    }
                }

                break;
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Box tocando");
        if (state == State.WaitingForItem && other.gameObject.CompareTag("Box") && beltItemsManager != null && tileOut.tileObject != null)
        {
            Debug.Log("Box taken");
            beltItemsManager.takeItemFromBelt(0);
            state = State.Moving;
        }
    }

    public override ItemData GetItemFromConstruction()
    {
        return null;
    }
}
