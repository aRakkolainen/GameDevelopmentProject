using Godot;
using System;

public partial class TimeManager : Node2D
{
	private RichTextLabel timer_text;
	private Timer timer;

	private int total_days;
	private int days_left;
	[Export] Level1 _level1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		total_days = _level1.GetTotalDays();
		timer_text = GetNode<RichTextLabel>("%TimerText");
		timer_text.Text = "Days left: " + total_days;
		timer = GetNode<Timer>("%Timer");
		StartTimer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(timer.IsStopped() && days_left > 0)
		{
			days_left = total_days--;
			timer_text.Text = "Days left: " + days_left;
			_level1.SetDayChanged(true);
			StartTimer();
		}
	}

	public void StartTimer()
	{
		timer.Start();
	}
}
