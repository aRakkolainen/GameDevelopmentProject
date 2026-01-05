using Godot;
using System;

public partial class SellPopup : Window
{

	[Signal]
	public delegate void ItemAmountChangedEventHandler(float amount);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnSellingDeskSellPopUpOpened()
	{
		GD.Print("Opening sell pop up!");
		Show();
	}

/* 	private void OnCancelButtonPressed()
	{
		GD.Print("Trying to close!");
		Hide();
	} */

	public void OnNumberOfItemsToBeSoldValueChanged(float amount)
	{
		EmitSignal(SignalName.ItemAmountChanged, amount);
	}

	
}
