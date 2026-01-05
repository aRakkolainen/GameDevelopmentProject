using Godot;
using System;

public class InventoryItem 
{
    // [Export] Inventory inventory;
    int ID;
    string name;
    int maxQuantity;
    int quantity;

    public InventoryItem(int id, string item_name,  int amount, int max)
    {
        ID = id; 
        name = item_name;
        quantity = amount;
        maxQuantity = max;
    }


    public int GetID()
    {
        return ID;
    }

    public string GetItemName()
    {
        return name;
    }

    public int GetMaxQuantity()
    {
        return maxQuantity;
    }

    public int GetQuantity()
    {
        return quantity;
    }

    public void SetQuantity(int num)
    {
        quantity = num;
    }


    // public override void _Ready()
    // {
    //     Pressed += () => AddNewItem();
    // }

    // private void AddNewItem()
    // {
    //     Inventory.Item item = new Inventory.Item(id, name, icon, maxQuantity, quantity);
    //     inventory.AddInventoryItem(item);
    // }
}
