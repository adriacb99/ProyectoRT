using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsInventoryPlayer : ItemsInventory
{
    public SlotUI[] ui;

    private void OnValidate()
    {
        
    }

    private void Start()
    {
        ui = new SlotUI[maxSlots];
        ui = CanvasSingleton.Instance.GetCanvas(3).GetComponentsInChildren<SlotUI>();
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < ui.Length; i++)
        {
            ui[i].UpdateSlotUI(slots[i].itemType?.icon, slots[i].quantity, slots[i].itemType, slots[i]);
        }
    }

    private void Update()
    {
        UpdateInventoryUI();
    }
}
