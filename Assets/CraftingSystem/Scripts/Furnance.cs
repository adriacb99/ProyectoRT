using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnance : MonoBehaviour
{
    InventoryFurnace inventory;

    [SerializeField] CraftRecipe[] recipes;
    [SerializeField] int recipeIndex;

    CraftRecipe selectedRecipe;

    bool stopeed = false;
    bool canCraft = true;
    private float time = 0;

    private void Start()
    {
        inventory = GetComponent<InventoryFurnace>();
        selectedRecipe = recipes[0];
    }

    private void Update()
    {
        if (!stopeed)
        {
            // time += Time.deltaTime;
            // if (time >= selectedRecipe.time)
            // {
            //     bool canCraft = true;
            //     for (int i = 0; i < selectedRecipe.RequiredItems.length; i++){
            //         if (selectedRecipe.RequiredItems[i].quantity > inventory.GetItemInstance(i).quantity  && canCraft) {
            //             canCraft = false;
            //         }
            //     }
            //     if (canCraft)
            //         Debug.Log("Crafting")
            // }
            for (int i = 0; i < selectedRecipe.RequiredItems.Length; i++){
                if (selectedRecipe.RequiredItems[i].quiantity > inventory.GetItemInstance(i).quantity  && canCraft) {
                    canCraft = false;
                }
            }
            if (canCraft) {
                time += Time.deltaTime;
                if (time >= selectedRecipe.time)
                {
                    Debug.Log("Crafting");
                    time = 0;
                    canCraft = true;
                    for (int i = 0; i < selectedRecipe.RequiredItems.Length; i++){
                        inventory.RetireItem(i, selectedRecipe.RequiredItems[i].quiantity);
                        inventory.AddResultItem(new ItemInstance(selectedRecipe.CraftedItem));
                    }
                }
            }
        }
    }
}
