using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Furnance : MonoBehaviour
{
    InventoryFurnace inventory;

    [SerializeField] CraftRecipe[] recipes;
    [SerializeField] int recipeIndex;

    CraftRecipe selectedRecipe;

    bool stopeed = false;
    bool canCraft = true;
    private float time = 0;

    GameObject canvas;

    private void Start()
    {
        inventory = GetComponent<InventoryFurnace>();
        selectedRecipe = recipes[0];
        canvas = CanvasSingleton.Instance.GetCanvas(1);
    }

    private void Update()
    {
        if (!stopeed)
        {
            for (int i = 0; i < selectedRecipe.RequiredItems.Length; i++){
                if (selectedRecipe.RequiredItems[i].quiantity > inventory.GetItemInstance(i).quantity) {
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
                        inventory.RetireItem(i, selectedRecipe.RequiredItems[i].quiantity);
                        inventory.AddResultItem(new ItemInstance(selectedRecipe.CraftedItem));
                    }
                    UpdateCanvas();
                }
            }
            canCraft = true;
        }
    }

    public void UpdateCanvas()
    {
        canvas.SetActive(true);
        canvas.GetComponentsInChildren<Slot>()[0].SetSlotInfo(inventory.result);
        canvas.GetComponentsInChildren<Slot>()[1].SetSlotInfo(inventory.GetItemInstance(0));
        TextMeshProUGUI[] testo = canvas.GetComponentsInChildren<TextMeshProUGUI>();
        testo[0].text = inventory.result.quantity.ToString();
        testo[1].text = inventory.items[0].quantity.ToString();
    }
}
