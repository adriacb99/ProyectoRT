using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    Image slotImage;
    public ItemData slotItem;
    public int quantity;

    public ItemInstance slotInstance;

    void Awake() {
        slotImage = gameObject.GetComponentInChildren<Image>();
    }

    public void SetSlotInfo(ItemInstance info) {
        slotInstance = info;
        quantity = info.quantity;
        slotItem = info.itemType;
        slotImage.sprite = info.itemType.icon;
    }

    public void UpdateSlot()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = slotInstance.quantity.ToString();
    }
}
