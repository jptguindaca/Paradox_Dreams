using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image selectionHighlight;

    public void Setup(InventorySlot slot, bool isSelected)
    {
        bool hasItem = !slot.IsEmpty;
        icon.enabled = hasItem;
        quantityText.enabled = hasItem && slot.Quantity > 1;
        if (hasItem)
        {
            icon.sprite = slot.Item.Icon;
            quantityText.text = slot.Quantity.ToString();
        }

        if (selectionHighlight != null)
        {
            selectionHighlight.gameObject.SetActive(isSelected);
        }
    }
}