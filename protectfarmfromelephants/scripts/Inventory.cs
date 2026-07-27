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
