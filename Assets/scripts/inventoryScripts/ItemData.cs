using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int maxStackSize = 1;
    [SerializeField] private GameObject equipmentPrefab;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public int MaxStackSize => maxStackSize;
    public GameObject EquipmentPrefab => equipmentPrefab;
}