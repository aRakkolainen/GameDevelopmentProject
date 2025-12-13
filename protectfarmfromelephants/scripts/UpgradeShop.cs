using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class UpgradeShop : ItemList
{
	[Export] int upgradeNumber = 6;
	[Export] Texture2D blankIcon;
	private UpgradeItem[] upgradeItems;

	private string[] itemTypes;



	

	private Dictionary<string, Godot.Collections.Array> upgradeOptions;

	private int numberOfTypes;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		Random rnd = new();
		upgradeOptions = new Dictionary<string, Godot.Collections.Array>();

		numberOfTypes = 3;
		itemTypes = new string[numberOfTypes];
		//Godot.Collections.Array defenses = new Godot.Collections.Array();

       /*  Godot.Collections.Array plantDefenses = new Godot.Collections.Array();
        plantDefenses.Add("Sunflower");
		plantDefenses.Add("Chili");

		Godot.Collections.Array distractions = new Godot.Collections.Array();
		distractions.Add("Fire");
		distractions.Add("Beatbox");
		distractions.Add("Beehive");
		
		
		upgradeOptions.Add("artificial_defense", defenses);
		upgradeOptions.Add("organic_defenses", plantDefenses);
		upgradeOptions.Add("distraction", distractions);

        upgradeItems = new UpgradeItem[upgradeNumber];

		for (int i=0; i < upgradeNumber; i++)
        {
            AddItem(" ", blankIcon);
			int randomDefenseIndex = rnd.Next(defenses.Count());
			AddItem(defenses.ElementAt(0).ToString(), fence);
        } */

		
		//ItemClicked+= OnUpgradeItemClicked;
    }

	

	public void AddUpgradeItem(UpgradeItem item)
    {
        if (item == null || item.Quantity <= 0)
        {
            return;
        }

		//bool possibleToPickup = AddStackableItem(item);
		for (int i = 0; i < upgradeNumber; i++)
		{
			if(upgradeItems[i] != null) continue;

			upgradeItems[i] = item;
			SetItemIcon(i, item.Icon);

			if (item.MaxQuantity > 1)
            {
                SetItemText(i, upgradeItems[i].Quantity.ToString());
            }

		}

		//return possibleToPickup;

    }

	private void OnUpgradeItemClicked(long index, Vector2 pos, long mouseButtonIndex)
    {
		if (mouseButtonIndex == 1)
        {
        UpgradeItem item = GetUpgradeItem((int) index);
		if(item == null)
        	{
            	GD.Print("No item available!");
				return;
            }  
		GD.Print($"You clicked {item.Name}!");
        }
    }

    private void RemoveInventoryItem(int index)
    {
        throw new NotImplementedException();
    }


    private UpgradeItem GetUpgradeItem(int index)
    {
        UpgradeItem upgrade = upgradeItems[index];
		if(upgrade == null)
        {
            return null;
        } 
		return upgrade;
    }

    public class UpgradeItem
    {
        public int ID; 

		public string Name;

		public Texture2D Icon; 
		
		public int Price;

		public int MaxQuantity;

		public int Quantity;


		public UpgradeItem(int id, string name, Texture2D icon, int price, int maxQuantity, int quantity)
        {
            ID = id;
			Name = name;
			Icon = icon; 
			Price = price;
			MaxQuantity = maxQuantity;
			Quantity = quantity;

        }
    }
}
