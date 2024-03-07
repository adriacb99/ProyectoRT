using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Furnance : MonoBehaviour
{
    ItemsInventoryFurnance inventory;

    [SerializeField] CraftRecipe[] recipes;
    [SerializeField] int recipeIndex;

    CraftRecipe selectedRecipe;

    bool stopeed = false;
    bool canCraft = true;
    private float time = 0;

    GameObject canvas;

    private void Start()
    {
        inventory = GetComponent<ItemsInventoryFurnance>();
        selectedRecipe = recipes[0];
        canvas = CanvasSingleton.Instance.GetCanvas(1);

        //inventory.slots[0].itemType.icon = selectedRecipe.CraftedItem.icon;
        inventory.SetCraftItems(selectedRecipe);
    }

    private void Update()
    {
        if (!stopeed)
        {
            for (int i = 0; i < selectedRecipe.RequiredItems.Length; i++){
                if (selectedRecipe.RequiredItems[i].quiantity > inventory.slots[i+1].quantity) {
                    canCraft = false;
                }
            }
            if (canCraft) {
                time += Time.deltaTime;
                if (time >= selectedRecipe.time)
                {
                    Debug.Log("Crafting");
                    time = 0;
                    for (int i = 0; i < selectedRecipe.RequiredItems.Length; i++){
                        bool a = inventory.AddItemToSlot(selectedRecipe.CraftedItem);
                        if (a) inventory.RetireItemToSlot(i + 1, selectedRecipe.RequiredItems[i].quiantity);
                    }
                }
            }
            canCraft = true;
        }
    }
}
