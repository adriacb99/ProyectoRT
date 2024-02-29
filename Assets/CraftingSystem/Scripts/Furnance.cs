using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnance : MonoBehaviour
{
    Inventory inventory;

    [SerializeField] CraftRecipe[] recipes;
    [SerializeField] int recipeIndex;

    CraftRecipe selectedRecipe;

    bool stopeed = false;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        if (!stopeed)
        {

        }
    }
}
