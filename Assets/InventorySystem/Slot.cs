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

    void Awake() {
        slotImage = gameObject.GetComponentInChildren<Image>();
    }

    public void SetSlotImage(ItemData sprite) {
        //this.quantity = quantity;
        slotItem = sprite;
        slotImage.sprite = sprite.icon;
    }
}
