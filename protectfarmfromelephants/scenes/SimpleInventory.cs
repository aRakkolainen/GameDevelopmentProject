using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Text;

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
	public delegate void InventoryItemActivatedForUseEventHandler(int id, string item_name, string item_type, int quantity);

	[Signal]
	public delegate void FruitsSoldEventHandler();

	[Signal]
	public delegate void InventoryItemsChangedEventHandler();

	[Signal]
	public delegate void UpdatedMoneyTextEventHandler();

	[Signal]
	public delegate void UpdatedInfoTextEventHandler(string message);

	[Signal]
	public delegate void ItemRemovedFromInventoryEventHandler(string item_name);



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
                var texture = LevelManager.Instance.GetTextureByItemName(currentItem.GetItemName());
                var icon = (Texture2D)GD.Load(texture);
				GD.Print(currentItem.GetItemName());
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
			case "fence":
				texture = Scenes.UpgradeItemTextures.fence;
				break;
			case"stone_wall":
				texture = Scenes.UpgradeItemTextures.stone_wall;
				break;
			case "camp_fire":
				texture = Scenes.UpgradeItemTextures.camp_fire;
				break;
			case "noise_maker":
				texture = Scenes.UpgradeItemTextures.noise_maker;
				break;
			case "beehive":
				texture = Scenes.UpgradeItemTextures.beehive;
				break;
			case "chili":
				texture = Scenes.UpgradeItemTextures.chili;
				break;	
			case "sunflower":
				texture = Scenes.UpgradeItemTextures.sunflower;
				break;	

			case "seeds":
				if (level == null || (level != null && level.GetPlantType() == null))
				{
					break;
				} else
				{
					
				string plant_type = LevelManager.Instance.GetLevelDataForActiveLevel().GetPlantType();
				if (plant_type.Equals("pineapple"))
				{
					texture = Scenes.ItemTextures.pineapple_seeds;
				} else if (plant_type.Equals("watermelon"))
				{
					texture = Scenes.ItemTextures.watermelon_seeds;
				} else if (plant_type.Equals("mango"))
				{
					texture = Scenes.ItemTextures.mango_seeds;
				}
				}
				break;
		}

		return texture;
	}

	public void OnInventoryItemActivated(int index)
	{
		InventoryItem item = inventory_items[index];
		EmitSignal(SignalName.InventoryItemActivatedForUse, item.GetID(), item.GetItemName(), item.GetItemType(), item.GetQuantity());
	}

	public void OnItemSelected(int index){
		InventoryItem item = inventory_items[index];
		EmitSignal(SignalName.InventoryItemActivatedForUse, item.GetID(), item.GetItemName(), item.GetItemType(), item.GetQuantity());
	}

	public void OnUpdatedPlayerInventory(int id, string item_name, string item_type, int quantity, int max_quantity)
	{
		if (max_quantity == 0)
		{
			max_quantity = max_stack;
		}

		if (inventory_items != null && inventory_items.Count < 10)
		{
			GD.Print("Trying to add item " + item_name + " with quantity " + quantity);
			AddToInventory(id, item_name, item_type, quantity, max_quantity);
			Clear();
            DisplayNewItems();
			/* if(item == null)
			{
				item = new InventoryItem(id, item_name, item_type, quantity, max_quantity);
				AddToInventory(item);
			} else
			{
				if (item.GetQuantity() + quantity <= item.GetMaxQuantity()){
					item.SetQuantity(item.GetQuantity()+quantity);
					AddToInventory(item);
				} else
				{
					int remaining_quantity = item.GetMaxQuantity() - quantity;
					InventoryItem new_item = new InventoryItem(id, item_name, item_type, remaining_quantity, max_quantity);

				}
			}
			
		}
			Clear();
            DisplayNewItems(); */
	}
	}

	/* public void OnFarmUpdatedSeedCount(int quantity, string update_type)
	{
		GD.Print("Trying to update seed counts");
		SetNumberOfSeedsInInventory(quantity, update_type);
		Clear();
		DisplayNewItems();
	} */

	public void OnUpdatedItemQuantity(int id, string item_name, int quantity, string update_type)
	{
		UpdateItemQuantity(id, item_name, update_type, quantity);
		Clear();
		DisplayNewItems();
	}



	public void OnSellPopupSoldAllItemsFromInventory(string itemName)
	{
		int indexOfItem = inventory_items.FindIndex(item => item.GetItemName() == itemName);
		int itemSellValue = 0;
		if("chili".Equals(itemName) || "sunflower".Equals(itemName))
		{
			itemSellValue = level.GetLevelDistractionPlantSellValue();
		} else
		{
			itemSellValue = level.GetLevelFruitSellValue();
		}
		if(indexOfItem == -1)
		{
			GD.Print("Item not found, cannot sell!");
			EmitSignal(SignalName.UpdatedInfoText, "Item " + itemName + " not found, cannot sell!");
		} else
		{
			InventoryItem item = inventory_items[indexOfItem];
			LevelManager.Instance.AddToTotalMoney(item.GetQuantity()*itemSellValue);
			if (itemName.Equals(level.GetPlantType()))
			{
				bool quotaUpdated = LevelManager.Instance.UpdateLevelQuota(item.GetQuantity());
				if(quotaUpdated) {
					EmitSignal(SignalName.FruitsSold);
			}
			}
			RemoveFromInventory(item);
			EmitSignal(SignalName.UpdatedMoneyText);
			Clear();
			DisplayNewItems();
		
		}
	}

	public void OnSellPopupSoldNumberOfItemsFromInventory(int amount, string itemName)
	{
		int indexOfItem = inventory_items.FindIndex(item => item.GetItemName() == itemName);
		int itemSellValue = 0;
		if("chili".Equals(itemName) || "sunflower".Equals(itemName))
		{
			itemSellValue = level.GetLevelDistractionPlantSellValue();
		} else
		{
			itemSellValue = level.GetLevelFruitSellValue();
		}
		if(indexOfItem == -1)
		{
			GD.Print("Fruit not found, cannot sell!");
			EmitSignal(SignalName.UpdatedInfoText, "Fruit not found, cannot sell!");
		} else
		{
			InventoryItem item = inventory_items[indexOfItem];
			if (amount <= item.GetQuantity())
			{
				if (itemName.Equals(level.GetPlantType()))
			{
				bool quotaUpdated = LevelManager.Instance.UpdateLevelQuota(amount);
				if(quotaUpdated) {
					EmitSignal(SignalName.FruitsSold);
				}
				}
				LevelManager.Instance.AddToTotalMoney(amount*itemSellValue);
				EmitSignal(SignalName.UpdatedMoneyText);
				item.SetQuantity(item.GetQuantity()-amount);
				if(item.GetQuantity() == 0)
				{
					RemoveFromInventory(item);
				}
				Clear();
				DisplayNewItems();
			} else
			{
				EmitSignal(SignalName.UpdatedInfoText, "You are trying to sell more than you have in your inventory!");
			}
		
		}
		
	}

	public int GetNumberOfSeedsInInventory(int id)
	{
		InventoryItem seeds = inventory_items.Find(item => item.GetID() == id && item.GetItemName() == seed_type);
		if (seeds != null)
		{
			return seeds.GetQuantity();
		} else
		{
			return 0;
		}
	}

	/* public void SetNumberOfSeedsInInventory(int quantity, string type)
	{
		InventoryItem seeds = inventory_items.Find(item => item.GetItemName() == seed_type);
		if (seeds != null)
		{
			int currentQuantity = seeds.GetQuantity();
			if (currentQuantity >= 0)
			{
				UpdateItemQuantity(seeds.GetItemName(), type, quantity);
			}
		}
	} */

	public void AddToInventory( int id, string item_name, string item_type, int quantity, int max_quantity)
    {
		
		int index = FindIndexForItemInInventory(item_name);
		int items_with_same_name = 0;
		if(FindAllItemsForNameInInventory(item_name) != null)
		{
			items_with_same_name = FindAllItemsForNameInInventory(item_name).Count;
		}

		if(inventory_items.Count < max_inventory_size)
		{
		if(index == -1)
		{
			InventoryItem item = new InventoryItem(inventory_items.Count+1, item_name, item_type, quantity, max_quantity);
        	inventory_items.Add(item);
			GD.Print("You collected new item " + item.GetItemName() + " and total quantity is " + item.GetQuantity());
		} else
            {
                if (items_with_same_name > 0)
                {
                    if (items_with_same_name == 1)
                    {
						InventoryItem currentItem = inventory_items[index];
						HandleIsOverMaxStackSize(quantity, currentItem);
                    }
                    else
                    {
						List<InventoryItem> current_items = FindAllItemsForNameInInventory(item_name);
						foreach(InventoryItem item in current_items)
						{
							if(item.GetQuantity() == max_quantity)
							{
								continue;
							} else
							{
								HandleIsOverMaxStackSize(quantity, item);
							}
						}
                    }
                }
            }
        } else
		{
			GD.Print("Inventory full, drop something!");
			
		}
    }

    private bool HandleIsOverMaxStackSize(int quantity, InventoryItem currentItem)
    {
        int currentQuantity = currentItem.GetQuantity();
        int max = currentItem.GetMaxQuantity();
		bool IsOverMaxSize = false;
        if (currentQuantity < max && currentQuantity + quantity <= max)
        {
            currentItem.SetQuantity(currentQuantity + quantity);
            GD.Print("You collected existing item " + currentItem.GetItemName() + " and total quantity is " + currentItem.GetQuantity());
        }
        else if (currentQuantity + quantity >= max)
        {
            int over_max_stack = currentQuantity + quantity - max;
            int remainder = quantity - over_max_stack;
            currentItem.SetQuantity(currentQuantity + remainder);
            InventoryItem new_item = new(inventory_items.Count + 1, currentItem.GetItemName(), currentItem.GetItemType(), over_max_stack, max_stack);
            inventory_items.Add(new_item);
            IsOverMaxSize = true;
    	}
		return IsOverMaxSize;
	}

    public void UpdateItemQuantity(int id, string name, string update_type, int quantity)
	{
		int index = inventory_items.FindIndex(i => i.GetID() == id && i.GetItemName() == name);
		if(index == -1)
		{
			GD.Print("Item not found!");
		} else
		{
			InventoryItem current = inventory_items[index];
			int current_quantity = current.GetQuantity();

			if(index > inventory_items.Count)
			{
				return;
			}

			if (update_type.Equals("increase"))
				{
					if (quantity >= 0 && quantity + current_quantity < current.GetMaxQuantity())
				{
					current.SetQuantity(current_quantity + quantity);
				}
					
				} else if (update_type.Equals("decrease"))
			{
				if (quantity >= 0 && current_quantity - quantity >= 0 && current_quantity - quantity < current.GetMaxQuantity())
				{
					int new_quantity = current_quantity - quantity;
					if (new_quantity == 0)
					{
						EmitSignal(SignalName.ItemRemovedFromInventory, current.GetItemName());
						RemoveFromInventory(current);
					} else
					{
						current.SetQuantity(current_quantity - quantity);
					}
				}
			} else if (update_type.Equals("custom") && quantity >= 0 && quantity < current.GetMaxQuantity())
			{
				current.SetQuantity(quantity);
			}
			
		}
	}

	public int GetItemQuantityInInvetory(int id, string name)
	{
		InventoryItem item = inventory_items.Find(item => item.GetID() == id && item.GetItemName() == name);
		if (item != null)
		{
			return item.GetQuantity();
		} else
		{
			return 0;
		}
	}

	

	public void RemoveFromInventory(InventoryItem item)
    {
        inventory_items.Remove(item);
    }

	public int FindIndexForItemInInventory(string item_name)
	{
		return inventory_items.FindIndex(i=> i.GetItemName() == item_name);
	}

	public List<InventoryItem> FindAllItemsForNameInInventory(string item_name)
	{
		return inventory_items.FindAll(i=> i.GetItemName() == item_name);
	}

    public int GetMaxStack()
    {
        return max_stack;
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


