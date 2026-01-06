using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class SimpleInventory : ItemList
{
	private List<InventoryItem> inventory_items = new List<InventoryItem>();

	private Item[] items;

	private string seed_type;

	private LevelData level;

	private int max_inventory_size = 10;
	private int max_stack = 32;
	// Called when the node enters the scene tree for the first time.


	[Signal]
	public delegate void InventoryItemActivatedEventHandler();

	[Signal]
	public delegate void FruitsSoldEventHandler();

	[Signal]
	public delegate void InventoryItemsChangedEventHandler();


	public override void _Ready()
	{
		items = new Item[max_inventory_size];
		level = LevelManager.Instance.GetLevelDataForActiveLevel();
		seed_type = level.GetPlantType() + "_seeds";
    }

	public List<InventoryItem> GetInventoryItems()
	{
		return inventory_items;
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
			AddToInventory(item);
			Clear();
            DisplayNewItems();
        }
	}

	public void OnFarmUpdatedSeedCount()
	{
		GD.Print("Trying to update seed counts");
		SetNumberOfSeedsInInventory();
		Clear();
		DisplayNewItems();
	}

	public void OnSellPopupSoldAllItemsFromInventory()
	{
		int indexOfFruit = inventory_items.FindIndex(item => item.GetItemName() == level.GetPlantType());
		if(indexOfFruit == -1)
		{
			GD.Print("Fruit not found, cannot sell!");
		} else
		{
			InventoryItem fruit = inventory_items[indexOfFruit];
			bool quotaUpdated = LevelManager.Instance.UpdateLevelQuota(fruit.GetQuantity());
			if(quotaUpdated) {
				fruit.SetQuantity(0);
				EmitSignal(SignalName.FruitsSold);
				Clear();
				DisplayNewItems();
			}
		
		}
	}

	public void OnSellPopupSoldNumberOfItemsFromInventory(float amount)
	{
		
	}

	public int GetNumberOfSeedsInInventory()
	{
		InventoryItem seeds = inventory_items.Find(item => item.GetItemName() == seed_type);
		return seeds.GetQuantity();
	}

	public void SetNumberOfSeedsInInventory()
	{
		InventoryItem seeds = inventory_items.Find(item => item.GetItemName() == seed_type);
		if (seeds != null)
		{
			int currentQuantity = seeds.GetQuantity();
			if (currentQuantity > 0)
			{
				seeds.SetQuantity(currentQuantity-1);
			}
		}
	}

	public bool AddToInventory(InventoryItem item)
    {
		
		int index = FindIndexForItemInInventory(item);


		if(inventory_items.Count < max_inventory_size)
		{
		if(index == -1)
		{
        	inventory_items.Add(item);
			GD.Print("You collected item" + item.GetItemName() + " and total quantity is " + item.GetQuantity());
			return true;
		} else
		{
			InventoryItem currentItem = inventory_items[index];
			int currentQuantity = currentItem.GetQuantity();
			int max = currentItem.GetMaxQuantity();
			if(currentQuantity < max)
			{
				currentItem.SetQuantity(++currentQuantity);
				GD.Print("You collected item " + currentItem.GetItemName() + " and total quantity is " + currentItem.GetQuantity());
				return true;
			}
			return false;
		}
		} else
		{
			GD.Print("Inventory full, drop something!");
			return false;
			
		}
    }

	public void UpdateItemQuantity(string name, int quantity)
	{
		int index = inventory_items.FindIndex(i => i.GetItemName() == name);
		if(index == -1)
		{
			GD.Print("Item not found!");
		} else
		{
			InventoryItem current = inventory_items[index];
			if (quantity > 0 && quantity < current.GetMaxQuantity())
			{
				inventory_items[index].SetQuantity(quantity);
			}
		}
	}

	public void RemoveFromInventory(InventoryItem item)
    {
        inventory_items.Remove(item);
    }

	public int FindIndexForItemInInventory(InventoryItem item)
	{
		return inventory_items.FindIndex(i=> i.GetItemName() == item.GetItemName());
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


