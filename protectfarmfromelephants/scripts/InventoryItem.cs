using Godot;
using System;

public partial class InventoryItem : TextureButton
{
    [Export] Inventory inventory;
    [Export] int id;
    [Export] string name;
    [Export] Texture2D icon;
    [Export] int maxQuantity;
    [Export] int quantity;


    public override void _Ready()
    {
        Pressed += () => AddNewItem();
    }

    private void AddNewItem()
    {
        Inventory.Item item = new Inventory.Item(id, name, icon, maxQuantity, quantity);
        inventory.AddInventoryItem(item);
    }
}
