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
            if (ui[i].isActiveAndEnabled) ui[i].UpdateSlotUI2(slots[i].itemType?.icon, slots[i].quantity, slots[i].itemType, slots[i]);
            if (slots[i].itemType != null) slots[i].maxStack = slots[i].itemType.maxStack;
        }
    }

    public void SetCraftItems(CraftRecipe recipe)
    {
        slots[0] = new ItemSlot(recipe.CraftedItem);
        for (int i = 0; i < recipe.RequiredItems.Length; i++)
        {
            slots[i + 1] = new ItemSlot(recipe.RequiredItems[i].Item);
        }
    }

    private void Update()
    {
        UpdateInventoryUI();
    }
}
