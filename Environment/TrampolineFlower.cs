using Godot;
using System;
using YourFirstGame.Player;

public partial class TrampolineFlower : StaticBody2D
{
	[Export] public float BounceForce = -100.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnPlayerCheckerBodyEntered(Node2D body)
	{
		if (body.Name.ToString().Contains("Player"))
		{
			BasePlayer player = (BasePlayer)body;
			player.Velocity = new Vector2(player.Velocity.X, BounceForce);
		}
	}
}
