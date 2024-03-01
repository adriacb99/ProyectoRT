using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftUiManager : MonoBehaviour
{
    ItemInstance currentItem;
    public Image customCursor;

    void OnMouseDownItem(ItemInstance item) {
        if (currentItem == null) {
            currentItem = item;
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentItem.itemType.icon;
        }
    }
}
