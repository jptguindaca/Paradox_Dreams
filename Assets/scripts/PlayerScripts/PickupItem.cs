using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;

    public ItemData ItemData => itemData;
    public int Quantity => quantity;

    public void SetQuantity(int novaQuantidade)
    {
        quantity = novaQuantidade;
    }
}