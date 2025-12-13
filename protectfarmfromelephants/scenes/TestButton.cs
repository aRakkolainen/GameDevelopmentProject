using Godot;
using System;

public partial class TestButton : Button
{
	private Control _upgradeShop;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        //_upgradeShop = GetNode<Control>("%UpgradeShop"); 
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Pressed()
    {
        base._Pressed();
        if (_upgradeShop != null)
        {    
		 _upgradeShop.Visible = true;
        }

    }
}
