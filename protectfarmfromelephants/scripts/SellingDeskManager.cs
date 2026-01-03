using Godot;
using System;

public partial class SellingDeskManager : Node2D
{
	[Export] Label quota_text;

	[Export] TextureRect fruit_image;

	[Export] Area2D desk;

	string currentLevel;
	LevelData currentLevelData; 

	string display_quota_text;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentLevel = LevelManager.Instance.GetCurrentActiveLevel();
		currentLevelData = LevelManager.Instance.GetLevelData(currentLevel);
		quota_text ??= GetNode<Label>("%QuotaText");
		fruit_image ??= GetNode<TextureRect>("%Fruit");
		desk ??= GetNode<Area2D>("Area2D");
		if (currentLevelData != null)
		{
			UpdateLevelQuotaText();
			var texture= GetTexture(currentLevelData.GetPlantType());
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
		}
		return path;
	}

	private void UpdateLevelQuotaText()
	{
		string text = currentLevelData.GetCurrentQuota() + "/" + currentLevelData.GetExpectedQuota();
		quota_text.Text = text;
	}
}
