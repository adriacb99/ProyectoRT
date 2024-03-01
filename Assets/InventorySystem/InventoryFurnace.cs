using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryFurnace : Inventory
{
    [SerializeField] public ItemInstance result;

    ItemInstance resultItem;

    public bool AddResultItem(ItemInstance itemToAdd)
    {
        if (resultItem == null)
        {
            resultItem = itemToAdd;
            resultItem.quantity++;
            Debug.Log("Añadiendo stack nuevo al producto quemado en el horno");       
            result = resultItem;
            return true;
        }
        else if (resultItem.itemType == itemToAdd.itemType && resultItem.quantity < resultItem.maxStack) 
        {
            resultItem.quantity++;
            Debug.Log("Añadiendo item a stack del producto quemado en el horno");
            result = resultItem;
            return true;
        }

        Debug.Log("El producto quemado no cabe o no es del mismo tipo del que ya hay en el horno");
        return false;
    }

}
