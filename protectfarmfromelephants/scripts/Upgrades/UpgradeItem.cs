using Godot;
using System;

public class UpgradeItem 
{
    // [Export] Inventory inventory;
    string ID;
    string name;

    string type;
    int total_in_stock;

    int price;

    public UpgradeItem(string id, string item_name, string item_type, int amount, int item_price)
    {
        ID = id; 
        name = item_name;
        type = item_type;
        total_in_stock = amount;
        price = item_price;
    }


    public string GetID()
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

    public int GetTotalInStock()
    {
        return total_in_stock;
    }

    public void SetTotalInStock(int num)
    {
        total_in_stock = num;
    }

    public int GetPrice()
    {
        return price;
    }

    public void SetPrice(int num)
    {
       price = num;
    }

}