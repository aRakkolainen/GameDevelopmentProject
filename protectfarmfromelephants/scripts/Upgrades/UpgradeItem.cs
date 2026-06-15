using Godot;
using System;

public class UpgradeItem 
{
    // [Export] Inventory inventory;
    string ID;
    string Name;

    string Type;

    string Description;
    int TotalInStock;

    int TotalForLevel {get; set;}

    int Price;
    

    int AdditionalPrice {get; set;}

    string AdditionalPriceInfo { get; set; }

    public UpgradeItem(string id, string item_name, string desc, string item_type, int amount, int total_level, int item_price)
    {
        ID = id; 
        Name = item_name;
        Description = desc;
        Type = item_type;
        TotalInStock = amount;
        TotalForLevel = total_level;
        Price = item_price;
    }


    public string GetID()
    {
        return ID;
    }

    public string GetItemName()
    {
        return Name;
    }

    public string GetItemType()
    {
        return Type;
    }

    public string GetDescription()
    {
        return Description;
    }

    public int GetTotalInStock()
    {
        return TotalInStock;
    }


    public int GetPrice()
    {
        return Price;
    }


    public int GetAdditionalPrice()
    {
        return AdditionalPrice;
    }


    public void SetPrice(int num)
    {
       Price = num;
    }
    public void SetTotalInStock(int num)
    {
        TotalInStock = num;
    }
    public void SetAdditionalPrice(int num)
    {
       AdditionalPrice = num;
    }

    public string GetAdditionalPriceInfo()
    {
        return AdditionalPriceInfo;
    }

    public void SetAdditionalPriceInfo(string info)
    {
       AdditionalPriceInfo = info;
    }

    public int GetTotalForLevel()
    {
        return TotalForLevel;
    }

     public void SetTotalForLevel(int num)
    {
       TotalForLevel = num;
    }

}