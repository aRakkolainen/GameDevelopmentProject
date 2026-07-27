using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public partial class ControlsMenu : CanvasLayer
{
	Godot.Collections.Array<StringName> allActions;

	List<StringName> key_controls;

	List<StringName> mouse_controls;


	Label MoveUp;

	Label MoveDown; 

	Label MoveLeft;

	Label MoveRight; 

	Label PlaceItem; 

	Label PickupItem;

	Label BuyUpgradeItem;

	Dictionary<StringName, string> controls;
	bool waiting_for_custom_input;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        MoveUp = GetNode<Label>("%MoveUpLabel");
        MoveDown = GetNode<Label>("%MoveDownLabel");
        MoveLeft = GetNode<Label>("%MoveLeftLabel");
        MoveRight = GetNode<Label>("%MoveRightLabel");
        PlaceItem = GetNode<Label>("%PlaceItemLabel");
        PickupItem = GetNode<Label>("%PickupItemLabel");
        BuyUpgradeItem = GetNode<Label>("%BuyUpgradeItemLabel");

        key_controls = new List<StringName>();
        mouse_controls = new List<StringName>();

        ShowControls();

        /* foreach(StringName editableControl in editable_key_controls)
		{
			List<String> inputs = new List<string>();
			foreach(InputEventKey key in InputMap.ActionGetEvents(editableControl))
			{
				if(key.AsText() != null)
				{
					inputs.Add(key.AsText());
				}
			}
			controls.Add(editableControl, inputs);

		}

		foreach(StringName editableControl in editable_mouse_controls)
		{
			List<String> inputs = new List<string>();
			foreach(InputEventMouseButton mouse in InputMap.ActionGetEvents(editableControl))
			{
				if(mouse.AsText() != null)
				{
					inputs.Add(mouse.AsText());
				}
			}
			controls.Add(editableControl, inputs);

		}



		MoveUp.Text = controls.GetValueOrDefault("move_up").ToString(); */


    }

    private void ShowControls()
    {
        allActions = InputMap.GetActions();
		Dictionary<string, List<String>> eventsAndControls = new();

        foreach (StringName action in allActions)
        {
            if (action.Equals("move_up") || action.Equals("move_right") || action.Equals("move_left") || action.Equals("move_down"))
            {
                key_controls.Add((string) action);
            }
            else if (action.Equals("mouse_right_click") || action.Equals("mouse_left_click"))
            {
                mouse_controls.Add((string) action);
            }
        }
		foreach(StringName control in key_controls)
		{
			List<String> keysAsText = new();
			foreach(InputEventKey key in InputMap.ActionGetEvents(control).Cast<InputEventKey>())
				{
					if(key.AsText() != null)
					{
						keysAsText.Add(key.AsText());
					}
			}
			eventsAndControls.Add(control, keysAsText);
		
		}

		foreach(StringName control in mouse_controls)
		{
			List<String> keysAsText = new();
			foreach(InputEvent key in InputMap.ActionGetEvents(control))
				{
					if(key.AsText() != null)
					{
						keysAsText.Add(key.AsText());
					}
			}
			eventsAndControls.Add(control, keysAsText);
			
		}

        foreach (KeyValuePair<string, List<string>> entry in eventsAndControls)
		{
			switch (entry.Key)
			{
				case "move_up":
					MoveUp.Text = string.Concat(entry.Value.ToArray());
					break;
				case "move_down":
					MoveDown.Text = string.Concat(entry.Value.ToArray());
					break;
				case "move_left":
					MoveLeft.Text = string.Concat(entry.Value.ToArray());
					break;
				case "move_right":
					MoveRight.Text = string.Concat(entry.Value.ToArray());
					break;
				case "mouse_left_click":
					PickupItem.Text = string.Concat(entry.Value.ToArray());
					BuyUpgradeItem.Text = "Double click " + string.Concat(entry.Value.ToArray());
					break;
				case "mouse_right_click":
					PlaceItem.Text = string.Concat(entry.Value.ToArray());
					break;

			}
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public static void OnMainMenuButtonPressed()
	{
		LevelManager.Instance.LoadScene(Scenes.Menus.main_menu);
	}

	public void OnExitButtonPressed()
	{
		Hide();
	}

	public void OnControlsMenuOpened()
	{
		Show();
	}

}
