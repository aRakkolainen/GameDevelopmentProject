using Godot;
using System;

public partial class SellPopup : CanvasLayer
{
	private float itemsToBeSold = 0;

	private LevelData currentLevelData; 

	private Label titleLabel;

	private string itemName;

	[Export] TextureRect fruit_image;

	[Export] string sell_desk_type;

	[Signal]
	public delegate void ItemAmountChangedEventHandler(float amount);

	[Signal]
	public delegate void SoldAllItemsFromInventoryEventHandler(string itemName);

	[Signal]
	public delegate void SoldNumberOfItemsFromInventoryEventHandler(int amount, string itemName);

	[Signal]
	public delegate void PauseTimerEventHandler();

	[Signal]
	public delegate void ContinueTimerEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		fruit_image ??= GetNode<TextureRect>("%Fruit");
		titleLabel = GetNode<Label>("TitleLabel");

		currentLevelData = LevelManager.Instance.GetLevelDataForActiveLevel();
		if (currentLevelData != null)
		{
			string texture= null;
			string infoText=null;
			switch (sell_desk_type)
			{
				case "fruit":
					itemName = currentLevelData.GetPlantType();
					break;
				case "distraction plant":
				System.Collections.Generic.List<UpgradeItem> upgrades = currentLevelData.GetLevelUpgradeItems();
				 	UpgradeItem distractionPlantUpgrade = upgrades.Find(upgrade => upgrade.GetItemType().Equals("distraction_plant"));
					 if(distractionPlantUpgrade != null && distractionPlantUpgrade.GetItemName() != null)
						{
							texture= GetTexture(distractionPlantUpgrade.GetItemName());
							itemName = distractionPlantUpgrade.GetItemName();
							infoText = "How many distraction plants do you want to sell?";
					}
					break;
				
			}
			if(texture != null && infoText != null)
			{
				var texture2d = (Texture2D) GD.Load(texture);
				fruit_image.Texture = texture2d;
				titleLabel.Text = infoText;
			}

		}
	}

	private void OnSellingDeskSellPopUpOpened(string sellDeskType, string distractionPlantName)
	{
		if (currentLevelData != null)
		{
			string texture= null;
			string infoText=null;
			switch (sellDeskType)
			{
				case "fruit":
					texture= LevelManager.Instance.GetTextureByItemName(currentLevelData.GetPlantType());
					itemName = currentLevelData.GetPlantType();
					infoText = "How many fruits do you want to sell?";
					break;
				case "distraction plant":
					texture= LevelManager.Instance.GetTextureByItemName(distractionPlantName);
					itemName = distractionPlantName;
					infoText = "How many distraction plants do you want to sell?";

					break;
				
			}
			if(texture != null && infoText != null)
			{
				var texture2d = (Texture2D) GD.Load(texture);
				fruit_image.Texture = texture2d;
				titleLabel.Text = infoText;
			}

		}
		Show();
		EmitSignal(SignalName.PauseTimer);

	}

	public void OnNumberOfItemsToBeSoldValueChanged(float amount)
	{
		itemsToBeSold = amount;
	}

	public void OnSellCustomButtonPressed()
	{
		if (itemsToBeSold > 0)
		{
			GD.Print("You are trying to sell " + itemsToBeSold + " fruits");
			int fruits = (int) itemsToBeSold;
			if(itemName != "")
			{
				EmitSignal(SignalName.SoldNumberOfItemsFromInventory, fruits, itemName);
			}
			EmitSignal(SignalName.ContinueTimer);
		} else
		{
			GD.Print("You haven't selected any items to be sold!");
		}

	}

	public void OnSellAllButtonPressed()
	{
		GD.Print("Trying to sell all items from inventory..");
		EmitSignal(SignalName.SoldAllItemsFromInventory, itemName);
		Hide();
		EmitSignal(SignalName.ContinueTimer);

	}

	public void OnCloseButtonPressed()
	{
		Hide();
		EmitSignal(SignalName.ContinueTimer);
	}

	private static string GetTexture(string plant_type)
	{
		string path = "";
		switch (plant_type)
		{
			case "pineapple":
				path = Scenes.ItemTextures.pineapple;
				break;
			case"watermelon":
				path = Scenes.ItemTextures.watermelon;
				break;
			case "mango":
				path = Scenes.ItemTextures.mango;
				break;
		}
		return path;
	}

	
}
