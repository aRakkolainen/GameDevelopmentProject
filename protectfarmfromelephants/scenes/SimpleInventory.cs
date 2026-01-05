using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class SimpleInventory : ItemList
{
	private List<InventoryItem> inventory_items = new List<InventoryItem>();

	private Item[] items;

	private string seed_type;

	private int inventory_size = 10;
	// Called when the node enters the scene tree for the first time.


	[Signal]
	public delegate void InventoryItemActivatedEventHandler();


	public override void _Ready()
	{
		items = new Item[inventory_size];
		LevelData level = LevelManager.Instance.GetLevelDataForActiveLevel();
		seed_type = level.GetPlantType() + "_seeds";
    }

    private void DisplayNewItems()
    {
        for (int i = 0; i < inventory_items.Count; i++)
        {
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

	public void OnInventoryItemActivated(int index)
	{
		EmitSignal(SignalName.InventoryItemActivated, index);
	}

	public void OnUpdatedPlayerInventory(int id, string item_name, int quantity, int max_quantity)
	{
		InventoryItem item = new InventoryItem(id, item_name, quantity, max_quantity);
		GD.Print("Trying to add item " + item_name + " with quantity " + quantity);
		if(inventory_items != null)
        {
			int index = inventory_items.FindIndex(item => item.GetItemName() == item_name);
			if (index == -1) {
				inventory_items.Add(item);
			} else
			{
				InventoryItem current = inventory_items[index];
				if (current.GetQuantity() <= current.GetMaxQuantity())
				{
					current.SetQuantity(quantity);
				}
			}
			Clear();
            DisplayNewItems();
			GD.Print(inventory_items.Count);
        }
	}

	public void OnFarmUpdatedSeedCount()
	{
		GD.Print("Trying to update seed counts");
		InventoryItem seeds = inventory_items.Find(item => item.GetItemName() == seed_type);
		if (seeds != null)
		{
			int currentQuantity = seeds.GetQuantity();
			if (currentQuantity > 0)
			{
				seeds.SetQuantity(currentQuantity-1);
			}
			Clear();
			DisplayNewItems();
		}
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


