using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class SimpleInventory : ItemList
{
	private List<InventoryItem> inventory_items;

	private Item[] items;

	private int inventory_size = 10;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		items = new Item[inventory_size];
		inventory_items = LevelManager.Instance.GetPlayerInventory();
		if(inventory_items != null && inventory_items.Count > 0)
        {
            DisplayNewItems();

        }
    }

    private void DisplayNewItems()
    {
        for (int i = 0; i < inventory_items.Count; i++)
        {
            GD.Print(inventory_items[i].GetItemName());
            InventoryItem currentItem = inventory_items[i];
            if (currentItem != null)
            {
                var texture = GetTextureByItemName(currentItem.GetItemName());
                var icon = (Texture2D)GD.Load(texture);
                Item new_item = new Item(currentItem.GetID(), currentItem.GetItemName(), icon, currentItem.GetMaxQuantity(), currentItem.GetQuantity());
                AddItem(new_item.Quantity.ToString(), new_item.Icon);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (inventory_items.Count != LevelManager.Instance.GetPlayerInventory().Count)
		{
			GD.Print("Player inventory has updated!");
			inventory_items = LevelManager.Instance.GetPlayerInventory();
			DisplayNewItems();

		}
		// inventory_items = LevelManager.Instance.GetPlayerInventory();
		// if(inventory_items != null && inventory_items.Count > 0)
		// {
		// 	GD.Print(inventory_items);
			
		// }
	}

	private string GetTextureByItemName(string name)
	{
		string texture= "";
		switch (name)
		{
			case "watering_can":
				texture = Scenes.ItemTextures.watering_can;
				break;
			case "pineapple_seeds":
				texture = Scenes.ItemTextures.pineapple_seeds;
				break;
			case "watermelon_seeds":
				texture = Scenes.ItemTextures.watermelon_seeds;
				break;
			case "mango_seeds":
				texture = Scenes.ItemTextures.mango_seeds;
				break;
			case "pineapple":
				texture = Scenes.ItemTextures.pineapple;
				break;
			case "watermelon":
				texture = Scenes.ItemTextures.watermelon;
				break;
			case "mango":
				texture = Scenes.ItemTextures.mango;
				break;
		}

		return texture;
	}
}

public class Item
    {
        public int ID; 

		public string Name;

		public Texture2D Icon; 

		public int MaxQuantity;

		public int Quantity;


		public Item(int id, string name, Texture2D icon, int maxQuantity, int quantity)
        {
            ID = id;
			Name = name;
			Icon = icon; 
			MaxQuantity = maxQuantity;
			Quantity = quantity;
        }
    }
