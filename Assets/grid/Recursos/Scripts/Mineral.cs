using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : MonoBehaviour
{
    [SerializeField] int quantity = 1;
    [SerializeField] ItemData itemData;

    public void GetMineral(ItemsInventory inventoryScript) {
        Debug.Log("GETMINERAL");
        quantity--;
        if (quantity <= 0) Destroy(gameObject);

        inventoryScript.AddItemToSlot(itemData);

    }
}
