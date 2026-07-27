using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

public partial class UpgradeShop : CanvasLayer
{
	[Export] SimpleInventory _inventory;
	private List<UpgradeItem> upgrade_items = new List<UpgradeItem>();

	private ItemList visible_upgrade_items;

	private SoundEffectPlayer soundEffectPlayer;

	private LevelData level;

	private string[] itemTypes;

	private int watering_can_upgrade_purchases = 0;
	

	//private Dictionary<string, Godot.Collections.Array> upgradeOptions;

	private int numberOfTypes;

	[Signal]
	public delegate void PauseTimerEventHandler();

	[Signal]
	public delegate void ContinueTimerEventHandler();

	[Signal]
	public delegate void UpdatedMoneyTextEventHandler();

	[Signal]
	public delegate void UpdatedPassiveUpgradesListEventHandler(string passiveUpgradeName, int updateTimes);

	[Signal]
	public delegate void UpdatedItemsInStockTextEventHandler(int quantity);
	[Signal] public delegate void PlayerAddToInventoryEventHandler(int id, string name, string type, int quantity, int maxQuantity);

	[Signal] public delegate void UpdatedSeedCountEventHandler();

	[Signal] public delegate void UpdatedItemQuantityEventHandler(string item_name, int quantity, string update_type);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		Hide();
		visible_upgrade_items = GetNode<ItemList>("UpgradeItems");
		level = LevelManager.Instance.GetLevelDataForActiveLevel();
		if (level.GetLevelUpgradeItems() != null)
		{
			upgrade_items = level.GetLevelUpgradeItems();
			DisplayItemsInShop();
		}
		soundEffectPlayer = GetNode<SoundEffectPlayer>("SoundEffectPlayer");
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
				ShopItem new_item = new ShopItem(upgradeItem.GetID(), upgradeItem.GetItemName(), upgradeItem.GetDescription(), icon, upgradeItem.GetTotalInStock(), upgradeItem.GetPrice());
				StringBuilder sell_text_builder = new StringBuilder();
				sell_text_builder.Append(new_item.Description);
				sell_text_builder.Append(" ");
				sell_text_builder.Append("\n");
				sell_text_builder.Append("Available: ");
				sell_text_builder.Append(new_item.InStock);
				sell_text_builder.Append(" Price: ");
				sell_text_builder.Append(new_item.Price);
				if (upgradeItem.GetAdditionalPrice() > 0)
				{
					sell_text_builder.Append(" Extra price: ");
					sell_text_builder.Append(upgradeItem.GetAdditionalPrice());
					sell_text_builder.Append(" ");
					sell_text_builder.Append(upgradeItem.GetAdditionalPriceInfo());
				}
                visible_upgrade_items.AddItem(sell_text_builder.ToString(), new_item.Icon);
				
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
				EmitSignal(SignalName.PlayerAddToInventory, 1, level.GetPlantType() + "_" + selected_item.GetItemName(), selected_item.GetItemType(), 10, 0);
				//EmitSignal(SignalName.UpdatedSeedCount, 10, "increase");
			} else if (selected_item.GetItemName().Equals("watering_can_upgrade"))
			{
				int currentWaterTotal = LevelManager.Instance.GetWateringCanTotalLevel();
				watering_can_upgrade_purchases++;
				LevelManager.Instance.SetWateringCanTotalLevel(currentWaterTotal+5);
				EmitSignal(SignalName.UpdatedPassiveUpgradesList, selected_item.GetItemName(), watering_can_upgrade_purchases);
			} else if (selected_item.GetItemName().Equals("watering_can_puddle_upgrade"))
			{
				LevelManager.Instance.SetWateringCanPuddleUpgrade(true);
				EmitSignal(SignalName.UpdatedPassiveUpgradesList, selected_item.GetItemName(), 1);
			} else if (selected_item.GetItemName().Equals("super_fertilizer"))
			{
				/* if (selected_item.GetAdditionalPrice() > 0)
				{
					GD.Print("Super fertilizer has extra price, checking that player has collected elephant poop!");
					{
						EmitSignal(SignalName.PlayerAddToInventory, selected_item.GetID(), selected_item.GetItemName(), selected_item.GetItemType(), 1, 0);
						EmitSignal(SignalName.UpdatedItemQuantity, "elephant_poop", 1, "decrease");
					} else
					{
						GD.Print("You don't have the required extra price!");
						return;
					}

				} */
			} else if ("chili".Equals(selected_item.GetItemName()) || "sunflower".Equals(selected_item.GetItemName()))
			{
				EmitSignal(SignalName.PlayerAddToInventory, selected_item.GetID(), selected_item.GetItemName()+"_seeds", selected_item.GetItemType(), 1, 0);
			}
				else
			{
				EmitSignal(SignalName.PlayerAddToInventory, selected_item.GetID(), selected_item.GetItemName(), selected_item.GetItemType(), 1, 0);
			}
			LevelManager.Instance.MinusFromTotalMoney(selected_item.GetPrice());
			soundEffectPlayer.OnSoundEffectStarted("UI", "buy_item", 1);
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

	public string Description;

	public string type;

	public Texture2D Icon; 

	public int InStock;

	public int Price;


	public ShopItem(string id, string name, string desc, Texture2D icon, int amount, int value)
    {
        ID = id;
		Name = name;
		Description = desc;
		Icon = icon; 
		InStock = amount;
		Price = value;
    }
}

