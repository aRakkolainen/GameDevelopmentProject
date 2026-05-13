using Godot;
using System;

public class UpgradeItem 
{
    // [Export] Inventory inventory;
    string ID;
    string name;

    string type;

    string description;
    int total_in_stock;

    int total_for_level {get; set;}

    int price;

    int additional_price {get; set;}

    string additional_price_info { get; set; }

    public UpgradeItem(string id, string item_name, string desc, string item_type, int amount, int total_level, int item_price, int additional, string price_info)
    {
        ID = id; 
        name = item_name;
        description = desc;
        type = item_type;
        total_in_stock = amount;
        total_for_level = total_level;
        price = item_price;
        additional_price = additional;
        additional_price_info = price_info;
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

    public string GetDescription()
    {
        return description;
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

    public int GetAdditionalPrice()
    {
        return additional_price;
    }

    public void SetAdditionalPrice(int num)
    {
       additional_price = num;
    }

    public string GetAdditionalPriceInfo()
    {
        return additional_price_info;
    }

    public void SetAdditionalPriceInfo(string info)
    {
       additional_price_info = info;
    }

    public int GetTotalForLevel()
    {
        return total_for_level;
    }

     public void SetTotalForLevel(int num)
    {
       total_for_level = num;
    }
}