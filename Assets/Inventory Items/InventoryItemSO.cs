using UnityEngine;

public enum InventoryItemType
{
    Weapon = 0,
    Potion = 1
}

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "tk-jam-2023/InventoryItemSO", order = 0)]
public class InventoryItemSO : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;
    public InventoryItemType type;

    [Header("Only weapon")]
    public int requiredBlood = 10;
    public int attackSpeed = 10;
    public int damage = 10;

    public bool IsPotion => type == InventoryItemType.Potion;
    public bool IsWeapon => type == InventoryItemType.Weapon;
}
