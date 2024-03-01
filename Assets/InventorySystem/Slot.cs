using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    Image slotImage;

    void Awake() {
        slotImage = gameObject.GetComponent<Image>();
    }

    public void SetSlotImage(Sprite sprite) {
        slotImage.sprite = sprite;
    }
}
