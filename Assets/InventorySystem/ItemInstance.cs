using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public ItemData itemType;
    public int quantity;
    public int maxStack;

    public ItemInstance(ItemData itemData)
    {
        itemType = itemData;
        quantity = 0;
        maxStack = itemData.maxStack;
    }
}
