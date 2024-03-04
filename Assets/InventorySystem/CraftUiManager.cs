using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftUiManager : MonoBehaviour
{
    private ItemData currentItem;
    public Image customCursor;
    public TextMeshProUGUI quantity;
    private int intQuantity;

    public void OnMouseDownItem(Slot item) {
        if (currentItem == null && item.quantity != 0) {
            currentItem = item.slotItem;
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentItem.icon;
            intQuantity = item.quantity;
            quantity.text = intQuantity.ToString();
            item.slotInstance.EmptyInstance();     
            item.UpdateSlot();
        }
        else
        {
            if (item.slotInstance.itemType == currentItem) {
                int num = item.slotInstance.AddItemInstance(intQuantity);               
                if (num > 0) {
                    intQuantity = num;
                    quantity.text = intQuantity.ToString();
                    item.UpdateSlot();
                }
                else {
                    intQuantity = 0;
                    quantity.text = intQuantity.ToString();
                    item.UpdateSlot();
                    currentItem = null;
                    customCursor.gameObject.SetActive(false);
                }
            }
        }
    }
}
