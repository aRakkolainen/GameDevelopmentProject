using Godot;
using System;
using System.Data;
using System.Data.Common;

//This inventory system was created by following this YouTube tutorial: https://www.youtube.com/watch?v=OTRYHscL-lg
public partial class Inventory : ItemList
{
	// Called when the node enters the scene tree for the first time.
	[Export] int inventorySize = 5;
	[Export] Texture2D blankIcon;

	[Export] Player player;
	private Item[] items;
	public override void _Ready()
    {
        items = new Item[inventorySize];

		for (int i=0; i < inventorySize; i++)
        {
            AddItem(" ", blankIcon);
        }
		//ItemClicked+= OnInventoryItemClicked;
    }


	// public bool AddInventoryItem(Item item)
    // {
    //     if (item == null || item.Quantity <= 0)
    //     {
    //         return false;
    //     }

	// 	bool possibleToPickup = AddStackableItem(item);
	// 	for (int i = 0; i < inventorySize; i++)
	// 	{
	// 		if(items[i] != null) continue;

	// 		items[i] = item;
	// 		SetItemIcon(i, item.Icon);

	// 		if (item.MaxQuantity > 1)
    //         {
    //             SetItemText(i, items[i].Quantity.ToString());
    //         }

	// 		return true;
	// 	}

	// 	return possibleToPickup;

    // }

	// public void RemoveInventoryItem(int index){
	// 	if(index < 0 || index >= inventorySize){
	// 		return;
	// 	}
	// 	items[index] = null;
	// 	SetItemIcon(index, blankIcon);
	// 	SetItemText(index, " ");
	// }

	// public Item GetInventoryItem(int index)
    // {
    //     if(index < 0 || index >= inventorySize){
	// 		return null;
	// 	}
	// 	return items[index];
    // }

	// private void OnInventoryItemClicked(long index, Vector2 pos, long mouseButtonIndex)
    // {
	// 	if(mouseButtonIndex == 2)
    //     {
    //         Item item = GetInventoryItem((int) index);
	// 		if(item == null)
    //         {
    //             GD.Print("No item!");
	// 			return;
    //         }
	// 		RemoveInventoryItem((int) index);
	// 		GD.Print($"You dropped {item.Quantity} of {item.Name}");
    //     } else if (mouseButtonIndex == 1)
    //     {
    //     Item item = GetInventoryItem((int) index);
	// 	if(item == null)
    //     	{
    //         	GD.Print("No item!");
	// 			return;
    //         }  
	// 	GD.Print($"You clicked {item.Name} and you have a total of {item.Quantity}!");
    //     }
    // }

	// private bool AddStackableItem(Item item)
    // {
    //     bool possibleToPickup = false;
	// 	for (int i=0; i < items.Length; i++)
    //     {
    //         if(items[i] == null) continue;
			
	// 		if (items[i].ID != item.ID || items[i].Quantity >= items[i].MaxQuantity) continue;

	// 		if (items[i].Quantity + item.Quantity > items[i].MaxQuantity)
    //         {
    //             int amountToRemove = items[i].MaxQuantity - items[i].Quantity;
	// 			items[i].Quantity = items[i].MaxQuantity;

	// 			item.Quantity = item.Quantity - amountToRemove;

	// 			possibleToPickup = true;
	// 			SetItemText(i, items[i].Quantity.ToString());
	// 			continue;
    //         }

	// 		items[i].Quantity = item.Quantity + items[i].Quantity;
	// 		item.Quantity = 0;
	// 		SetItemText(i, items[i].Quantity.ToString());
	// 		return true;
    //     }

	// 	return possibleToPickup;

    // }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
}
