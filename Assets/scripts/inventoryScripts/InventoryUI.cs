using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private InventorySlotUI slotPrefab;

    private readonly List<InventorySlotUI> spawnedSlots = new List<InventorySlotUI>();

    private void Start()
    {
        if (inventory == null) inventory = FindFirstObjectByType<Inventory>();
        if (playerInteraction == null) playerInteraction = FindFirstObjectByType<PlayerInteraction>();

        if (inventory != null)
            inventory.OnInventoryChanged += Refresh;

        Refresh();
    }

    private void Update()
    {
        UpdateSelectionVisuals();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;
    }

    private void Refresh()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        spawnedSlots.Clear();

        if (inventory == null) return;

        int selectedIndex = playerInteraction != null ? playerInteraction.SlotSelecionado : 0;

        for (int i = 0; i < inventory.Slots.Count; i++)
        {
            InventorySlotUI ui = Instantiate(slotPrefab, slotContainer);
            bool isSelected = (i == selectedIndex);
            ui.Setup(inventory.Slots[i], isSelected);
            spawnedSlots.Add(ui);
        }
    }

    private void UpdateSelectionVisuals()
    {
        if (playerInteraction == null || inventory == null) return;

        int selectedIndex = playerInteraction.SlotSelecionado;

        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            if (i < inventory.Slots.Count)
            {
                bool isSelected = (i == selectedIndex);
                spawnedSlots[i].Setup(inventory.Slots[i], isSelected);
            }
        }
    }
}