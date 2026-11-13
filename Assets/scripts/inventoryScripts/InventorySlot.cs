using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData item;
    [SerializeField] private int quantity;

    public ItemData Item => item;
    public int Quantity => quantity;
    public bool IsEmpty => item == null || quantity <= 0;

    public int FreeSpace => IsEmpty ? (item != null ? item.MaxStackSize : 0) : (item.MaxStackSize - quantity);

    public void SetItem(ItemData newItem, int amount)
    {
        item = newItem;
        quantity = amount;
    }

    public int AddToStack(int amount)
    {
        int space = FreeSpace;
        int added = Mathf.Min(space, amount);
        quantity += added;
        return amount - added;
    }

    public void RemoveFromStack(int amount)
    {
        quantity -= amount;
        if (quantity <= 0) Clear();
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    
}