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

    //suma items a la cantidad y devuelve la cantidad restante que no cabe
    public int AddItemInstance(int quantity)
    {
        int tmp = this.quantity + quantity;
        if (tmp > maxStack)
        {
            this.quantity = maxStack;
            return tmp - maxStack;
        }
        else
        {
            this.quantity = tmp;
            return 0;
        }
    }
    public void EmptyInstance()
    {
        quantity = 0;
    }
}
