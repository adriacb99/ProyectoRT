using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxItems = 50; 
    public List<ItemInstance> items = new();

    public bool AddItem(ItemInstance itemToAdd)
    {
        // Finds an empty slot if there is one
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd;
                items[i].quantity++;
                Debug.Log("Añadiendo stack nuevo");
                return true;
            }
            else if (items[i].itemType == itemToAdd.itemType && items[i].quantity < items[i].maxStack) 
            {
                items[i].quantity++;
                Debug.Log("Añadiendo item a stack");
                return true;
            }
        }

        // Adds a new item if the inventory has space
        if (items.Count < maxItems)
        {
            items.Add(itemToAdd);
            return true;
        }

        Debug.Log("No space in the inventory");
        return false;
    }
}
