using Godot;
using System;

public partial class SellingDeskManager : TextureButton
{
	[Export] Label quota_text;

	[Export] TextureRect fruit_image;


	[Signal] public delegate void SellPopUpOpenedEventHandler(string sellDeskType, string distractionPlantName);

	

	private SpinBox fruitAmountToBeSoldSpinBox;
	private int currentLevel;
	private LevelData currentLevelData; 

	[Export] private string sellDeskType;


	string display_quota_text;

	string distractionPlantName; 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentLevel = LevelManager.Instance.GetCurrentActiveLevel();
		currentLevelData = LevelManager.Instance.GetLevelData(currentLevel);
		quota_text ??= GetNode<Label>("%QuotaText");
		fruit_image ??= GetNode<TextureRect>("%Fruit");
		if (currentLevelData != null)
		{
			string texture= null;
			string quotaText=null;
			switch (sellDeskType)
			{
				case "fruit":
					texture= GetTexture(currentLevelData.GetPlantType());
					UpdateLevelQuotaText();
					break;
				case "distraction plant":
				 	System.Collections.Generic.List<UpgradeItem> upgrades = currentLevelData.GetLevelUpgradeItems();
				 	UpgradeItem distractionPlantUpgrade = upgrades.Find(upgrade => upgrade.GetItemType().Equals("distraction_plant"));
					 if(distractionPlantUpgrade != null && distractionPlantUpgrade.GetItemName() != null)
						{
							texture= GetTexture(distractionPlantUpgrade.GetItemName());
							distractionPlantName = distractionPlantUpgrade.GetItemName();
							quotaText = currentLevelData.GetLevelDistractionPlantSellValue() + " extra coins / plant";
							quota_text.Text = quotaText;
					}
					break;
				
			}
			var texture2d = (Texture2D) GD.Load(texture);
			fruit_image.Texture = texture2d;
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Checking collision with player?
		
	}

	private string GetTexture(string plant_type)
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
			case "chili":
				path = Scenes.UpgradeItemTextures.chili;
				break;
			case "sunflower":
				path = Scenes.UpgradeItemTextures.sunflower;
				break;
		}
		return path;
	}

	private void UpdateLevelQuotaText()
	{
		string text = currentLevelData.GetCurrentQuota() + "/" + currentLevelData.GetExpectedQuota();
		quota_text.Text = text;
	}

	private void OnPressed()
	{
		EmitSignal(SignalName.SellPopUpOpened, sellDeskType, distractionPlantName);
	}

	public void OnSimpleInventoryFruitsSold()
	{
		UpdateLevelQuotaText();
	}
}
