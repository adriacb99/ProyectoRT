using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Image sprite;
    public int quantity;
    public ItemData slotItem;
    public ItemsInventory.ItemSlot inventorySlot;

    [SerializeField] Sprite defaultSprite;
    private TextMeshProUGUI _text;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = transform.GetChild(0).GetComponent<Image>();
        //sprite.gameObject.SetActive(false);       
        _text = GetComponentInChildren<TextMeshProUGUI>();
        //_text.gameObject.SetActive(false);
    }

    public void UpdateSlotUI(Sprite sprite, int quantity, ItemData slotItem, ItemsInventory.ItemSlot inventorySlot)
    {
        this.sprite.sprite = sprite;
        this.quantity = quantity;
        this.slotItem = slotItem;
        this.inventorySlot = inventorySlot;

        _text.text = quantity.ToString();

        if (quantity == 0)
        {
            _text.gameObject.SetActive(false);
            this.sprite.gameObject.SetActive(false);
        }
        else
        {
            _text.gameObject.SetActive(true);
            this.sprite.gameObject.SetActive(true);
        }
    }

    public void UpdateSlotTextUI()
    {
        _text.text = quantity.ToString();
        this.inventorySlot.quantity = quantity;
    }

    public void ActivateImage()
    {
        sprite.gameObject.SetActive(false);
    }

    public void ClearSlot()
    {
        quantity = 0;
        //sprite.gameObject.SetActive(false);
        //sprite.sprite = defaultSprite;
        this.inventorySlot.quantity = 0;
    }
}
