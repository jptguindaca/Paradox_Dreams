using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IInventory
{
    [SerializeField, Min(1)] private int capacity = 20;

    private readonly List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;

    public int Capacity => capacity;
    public IReadOnlyList<InventorySlot> Slots => slots;

    private void Awake()
    {
        for (int i = 0; i < capacity; i++)
            slots.Add(new InventorySlot());
    }

    public int GetItemCount(ItemData item)
    {
        int total = 0;
        foreach (var slot in slots)
            if (!slot.IsEmpty && slot.Item == item)
                total += slot.Quantity;
        return total;
    }

    public int AddItem(ItemData item, int quantity)
    {
        int remaining = quantity;

        foreach (var slot in slots)
        {
            if (remaining <= 0) break;
            if (slot.IsEmpty || slot.Item != item) continue;
            remaining = slot.AddToStack(remaining);
        }

        foreach (var slot in slots)
        {
            if (remaining <= 0) break;
            if (!slot.IsEmpty) continue;
            slot.SetItem(item, 0);
            remaining = slot.AddToStack(remaining);
        }

        bool changed = remaining != quantity;
        if (changed) NotifyChange();

        return remaining;
    }

    public bool RemoveItem(ItemData item, int quantity)
    {
        if (GetItemCount(item) < quantity) return false;

        int toRemove = quantity;
        foreach (var slot in slots)
        {
            if (toRemove <= 0) break;
            if (slot.IsEmpty || slot.Item != item) continue;

            int take = Mathf.Min(slot.Quantity, toRemove);
            slot.RemoveFromStack(take);
            toRemove -= take;
        }

        NotifyChange();
        return true;
    }

    public bool RemoveFromSlot(int index, int quantity)
    {
        if (index < 0 || index >= slots.Count) return false;
        
        InventorySlot slot = slots[index];
        if (slot.IsEmpty) return false;

        slot.RemoveFromStack(quantity);
        NotifyChange();
        return true;
    }

    private void NotifyChange() => OnInventoryChanged?.Invoke();
}