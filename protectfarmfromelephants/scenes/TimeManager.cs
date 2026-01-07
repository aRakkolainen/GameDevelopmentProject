using Godot;
using System;
//Copilot discussion used to troubleshoot and implement the signaling for timer timeout.
public partial class TimeManager : Timer
{
	[Signal] public delegate void TimerFinishedEventHandler();

	[Signal] public delegate void UpdatedTimerTextEventHandler();

	private RichTextLabel timer_text;
	[Export] private Timer timer;

	[Export] private int total_days;
	private int days_left;

	private bool dayChanged;
	
	private int currentDay = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		Timeout += () => EmitSignal(SignalName.TimerFinished);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateTimerText(int days_left)
	{
		EmitSignal(SignalName.UpdatedTimerText, days_left); 
	}
	public void StartTimer(int days_left)
	{
		SetDaysLeft(days_left);
		UpdateTimerText(days_left);
		Start();
	}

	public void PauseTimer()
	{
		Stop();
	}

	public bool GetDayChanged()
	{
		return dayChanged;
	}

	public void SetDayChanged(bool changed)
	{
		dayChanged = changed;
	}

	public int GetCurrentDay()
	{
		return currentDay;
	}

	public void SetCurrentDay(int day)
	{
		currentDay = day;
	}

	public int GetDaysLeft()
	{
		return days_left;
	}

	public void SetDaysLeft(int days)
	{
		days_left = days;
	}
}
