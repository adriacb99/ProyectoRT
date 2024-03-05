using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private ItemData currentItem;
    public Image customCursor;
    public TextMeshProUGUI quantity;
    private int intQuantity;

    public void OnMouseDownItem(SlotUI item) {
        if (currentItem == null && item.quantity != 0) {
            currentItem = item.slotItem;
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentItem.icon;
            intQuantity = item.quantity;
            quantity.text = intQuantity.ToString();
            item.ClearSlot();     
            item.UpdateSlotTextUI();
        }
        else
        {
            if (item.slotItem == currentItem) {
                int num = item.inventorySlot.AddToSlot(intQuantity);               
                if (num > 0) {
                    intQuantity = num;
                    quantity.text = intQuantity.ToString();
                }
                else {
                    intQuantity = 0;
                    quantity.text = intQuantity.ToString();
                    currentItem = null;
                    customCursor.gameObject.SetActive(false);
                }
            }
        }
        Debug.Log("Item Clicked");
    }
}
