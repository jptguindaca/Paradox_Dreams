using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Inventory inventory = other.GetComponent<Inventory>();
        if (inventory == null) return;

        int notAdded = inventory.AddItem(itemData, quantity);

        if (notAdded < quantity)
        {
            quantity = notAdded;
            if (quantity <= 0)
                Destroy(gameObject);
        }
    }
}
