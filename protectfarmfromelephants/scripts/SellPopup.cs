using Godot;
using System;

public partial class SellPopup : CanvasLayer
{
	private float itemsToBeSold = 0;

	private LevelData currentLevelData; 

	[Export] TextureRect fruit_image;

	[Signal]
	public delegate void ItemAmountChangedEventHandler(float amount);

	[Signal]
	public delegate void SoldAllItemsFromInventoryEventHandler();

	[Signal]
	public delegate void SoldNumberOfItemsFromInventoryEventHandler(int amount);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		fruit_image ??= GetNode<TextureRect>("%Fruit");
		currentLevelData = LevelManager.Instance.GetLevelDataForActiveLevel();
		if (currentLevelData != null)
		{
			var texture= GetTexture(currentLevelData.GetPlantType());
			var texture2d = (Texture2D) GD.Load(texture);
			fruit_image.Texture = texture2d;
		}
	}

	private void OnSellingDeskSellPopUpOpened()
	{
		Show();
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
			EmitSignal(SignalName.SoldNumberOfItemsFromInventory, (int) itemsToBeSold);
		} else
		{
			GD.Print("You haven't selected any fruits to be sold!");
		}

	}

	public void OnSellAllButtonPressed()
	{
		GD.Print("Trying to sell all fruits from inventory..");
		EmitSignal(SignalName.SoldAllItemsFromInventory);
		Hide();

	}

	public void OnCloseButtonPressed()
	{
		Hide();
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
