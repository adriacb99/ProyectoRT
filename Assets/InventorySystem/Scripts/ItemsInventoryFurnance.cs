using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsInventoryFurnance : ItemsInventory
{
    public SlotUI[] ui;

    [SerializeField] public ItemInstance result;

    private void Start()
    {
        ui = new SlotUI[maxSlots];
        ui = CanvasSingleton.Instance.GetCanvas(4).GetComponentsInChildren<SlotUI>();
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < ui.Length; i++)
        {
            ui[i].UpdateSlotUI2(slots[i].itemType?.icon, slots[i].quantity, slots[i].itemType, slots[i]);
            if (slots[i].itemType != null) slots[i].maxStack = slots[i].itemType.maxStack;
        }
    }

    private void Update()
    {
        UpdateInventoryUI();
    }
}
