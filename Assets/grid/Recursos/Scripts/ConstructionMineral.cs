using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionMineral : Construction
{
    [SerializeField] int quantity = 1;
    [SerializeField] ItemData itemData;

    public override ItemData GetItemFromConstruction()
    {
        return null;
    }

    public void GetMineral(ItemsInventory inventoryScript) {
        Debug.Log("GETMINERAL");
        quantity--;
        if (quantity <= 0) Destroy(gameObject);

        inventoryScript.AddItemToSlot(itemData);

    }
}
