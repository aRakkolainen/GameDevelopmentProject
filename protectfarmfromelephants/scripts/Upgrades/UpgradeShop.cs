using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

public partial class UpgradeShop : CanvasLayer
{
	private List<UpgradeItem> upgrade_items = new List<UpgradeItem>();

	private ItemList visible_upgrade_items;

	private LevelData level;

	private string[] itemTypes;
	

	//private Dictionary<string, Godot.Collections.Array> upgradeOptions;

	private int numberOfTypes;

	[Signal]
	public delegate void PauseTimerEventHandler();

	[Signal]
	public delegate void ContinueTimerEventHandler();

	[Signal]
	public delegate void UpdatedMoneyTextEventHandler();

	[Signal]
	public delegate void UpdatedItemsInStockTextEventHandler(int quantity);
	[Signal] public delegate void PlayerAddToInventoryEventHandler(int id, string name, string type, int quantity, int maxQuantity);

	[Signal] public delegate void UpdatedSeedCountEventHandler();

	[Signal] public delegate void UpdatedItemQuantityEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		Hide();
		visible_upgrade_items = GetNode<ItemList>("UpgradeItems");
		level = LevelManager.Instance.GetLevelDataForActiveLevel();
		upgrade_items = level.GetLevelUpgradeItems();
		//visible_upgrade_items.MaxColumns = upgrade_items.Count;
		DisplayItemsInShop();
    }

	private void DisplayItemsInShop()
    {
        for (int i = 0; i < upgrade_items.Count; i++)
        {
            UpgradeItem upgradeItem = upgrade_items[i];
            if (upgradeItem != null)
            {
                var texture = LevelManager.Instance.GetTextureByItemName(upgradeItem.GetItemName());
                var icon = (Texture2D)GD.Load(texture);
				ShopItem new_item = new ShopItem(upgradeItem.GetID(), upgradeItem.GetItemName(), icon, upgradeItem.GetTotalInStock(), upgradeItem.GetPrice());
				StringBuilder builder = new StringBuilder();
				string sell_text = "Available: " + new_item.InStock + " Price: " + new_item.Price;
				builder.Append(upgradeItem.GetDescription());
				builder.Append("\n");
				builder.Append(sell_text);
                visible_upgrade_items.AddItem(builder.ToString(), new_item.Icon);
            }
        } 
    }
	public void OnUpgradeShopPressed()
	{
		EmitSignal(SignalName.PauseTimer);
		Show();
	}
	public void OnUpgradeItemPurchaseClicked(int index)
	{
		int money_available = LevelManager.Instance.GetMoneyAvailable();
		UpgradeItem selected_item = upgrade_items[index];
		if (money_available > 0 && selected_item.GetPrice() <= money_available && selected_item.GetTotalInStock() > 0)
		{
			if (selected_item.GetItemName().Equals("seeds"))
			{
				EmitSignal(SignalName.PlayerAddToInventory, 1, level.GetPlantType() + "_" + selected_item.GetItemName(), selected_item.GetItemType(), 1, 0);
				EmitSignal(SignalName.UpdatedSeedCount, 1, "increase");
			} else
			{
				EmitSignal(SignalName.PlayerAddToInventory, selected_item.GetID(), selected_item.GetItemName(), selected_item.GetItemType(), 1, 0);
			}
			LevelManager.Instance.MinusFromTotalMoney(selected_item.GetPrice());
			EmitSignal(SignalName.UpdatedMoneyText);
			selected_item.SetTotalInStock(selected_item.GetTotalInStock()-1);
			visible_upgrade_items.Clear();
			DisplayItemsInShop();
		}
		
	}

	public void OnExitButtonPressed()
	{
		EmitSignal(SignalName.ContinueTimer);
		Hide();
	}

}
public class ShopItem {

	public string ID; 

	public string Name;

	public string type;

	public Texture2D Icon; 

	public int InStock;

	public int Price;


	public ShopItem(string id, string name, Texture2D icon, int amount, int value)
    {
        ID = id;
		Name = name;
		Icon = icon; 
		InStock = amount;
		Price = value;
    }
}

