using Godot;
using System;

namespace ProtectFarm;
public partial class ChangeDayPopup : CanvasLayer
{

	Label titleLabel;

	Label infoLabel;
	Label currentQuotaLabel;

	Button changeDayButton;
	[Export] TextureRect fruit_image;
	private LevelData currentLevelData; 

	[Signal] public delegate void ChangeDayPopupClosedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		fruit_image ??= GetNode<TextureRect>("%Fruit");
		titleLabel  = GetNode<Label>("%TitleLabel");
		infoLabel  = GetNode<Label>("%InfoLabel");
		currentQuotaLabel  = GetNode<Label>("%QuotaFilledLabel");
		changeDayButton = GetNode<Button>("%ChangeDayButton");

		currentLevelData = LevelManager.Instance.GetLevelDataForActiveLevel();
	}

	public void OnChangeDayPopUpOpened(int ended_day, int sold_quota, int expected_quota, string info_text, string button_text)
	{
		string texture = LevelManager.Instance.GetTextureByItemName(currentLevelData.GetPlantType());
		var texture2d = (Texture2D) GD.Load(texture);
		if(sold_quota < expected_quota)
		{
			infoLabel.AddThemeFontSizeOverride("larger_font", 25);
		}
		fruit_image.Texture = texture2d;
		titleLabel.Text = "Day " + ended_day + " ended!";
		infoLabel.Text = info_text;
		currentQuotaLabel.Text = sold_quota + "/" + expected_quota;
		changeDayButton.Text = button_text;

		Show();

	}

	public void OnCloseButtonPressed()
	{
		Hide();
		GetTree().Paused = false;
	}

	public void OnNewDayPressed()
	{
		Hide();
		GetTree().Paused = false;
		EmitSignal(SignalName.ChangeDayPopupClosed);
	}
}
