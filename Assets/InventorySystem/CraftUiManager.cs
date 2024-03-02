using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftUiManager : MonoBehaviour
{
    private ItemData currentItem;
    public Image customCursor;
    public string quantity;

    private void Awake()
    {
        //quantity = GetComponentInChildren<TextMeshProUGUI>().text;
    }

    public void OnMouseDownItem(Slot item) {
        if (currentItem == null) {
            currentItem = item.slotItem;
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentItem.icon;
            //quantity = item.quantity;
        }
    }
}
