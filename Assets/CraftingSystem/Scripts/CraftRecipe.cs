using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CraftRecipe : ScriptableObject
{
    [Serializable]
    public struct ItemGroup
    {
        public ItemData Item;
        public int quiantity;
    }

    public ItemGroup[] RequiredItems;
    public ItemData CraftedItem;

    public int time;
}
