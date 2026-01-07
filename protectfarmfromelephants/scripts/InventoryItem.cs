using Godot;
using System;

public class InventoryItem 
{
    // [Export] Inventory inventory;
    private int ID;
    private string name;
    private string type;
    private int maxQuantity;
    private int quantity;

    public InventoryItem(int id, string item_name, string item_type,  int amount, int max)
    {
        ID = id; 
        name = item_name;
        type = item_type;    
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

    public string GetItemType()
    {
        return type;
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

}
