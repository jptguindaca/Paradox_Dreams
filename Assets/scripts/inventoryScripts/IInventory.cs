using System;

public interface IInventory
{
    int Capacity { get; }
    int GetItemCount(ItemData item);
    int AddItem(ItemData item, int quantity);
    bool RemoveItem(ItemData item, int quantity);
    event Action OnInventoryChanged;
}
