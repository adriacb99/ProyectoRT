using System;
using System.Collections.Generic;
using UnityEngine;


public class ItemsInventory : MonoBehaviour
{
    public int maxSlots = 1;
    public ItemSlot[] slots;

    private void Awake()
    {
        slots = new ItemSlot[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            slots[i] = new ItemSlot();
        }
    }

    
    public void RetireItemToSlot(int i, int quantity)
    {
        if (slots[i].quantity - quantity < 0)
        {
            Debug.Log("Se quieren retirar mas items de los que hay, se cancela la retirada");
        }
        else slots[i].quantity -= quantity;      
    }

    public bool AddItemToSlot(ItemData slot)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemType == null)
            {
                slots[i] = new ItemSlot(slot);
                slots[i].quantity++;

                return true;
            }
            else if (slots[i].itemType == slot && slots[i].quantity < slots[i].maxStack)
            {
                slots[i].quantity++;

                return true;
            }
        }
        Debug.Log("No space in the inventory");
        return false;
    }

    [Serializable]
    public class ItemSlot
    {
        public ItemData itemType;
        public int quantity;
        public int maxStack;

        public ItemSlot()
        {
            itemType = null;
            quantity = 0;
            maxStack = 0;
        }
        public ItemSlot(ItemData itemData)
        {
            itemType = itemData;
            quantity = 0;
            maxStack = itemData.maxStack;
        }

        public ItemSlot(ItemData itemData, int quantity)
        {
            itemType = itemData;
            this.quantity = quantity;
            maxStack = itemData.maxStack;
        }

        public int AddToSlot(int quantity)
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
    }
}



