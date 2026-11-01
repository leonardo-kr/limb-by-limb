using Godot;

namespace YourFirstGame.Player;

public abstract partial class BasePlayer : CharacterBody2D
{
	[Export] public int Health = 10;
	[Export] public float Speed = 150.0f;
	[Export] public Vector2 RespawnLocation = new Vector2(-377, 19);
	
	// Gravity stuff
	[Export] public float GravityScale = 1f;
	[Export] public float FallGravityScale = 1f;
	[Export] public float MaxFallSpeed = 300.0f;
	
	public CollisionShape2D Collision;
	public Vector2 FacingDirection = Vector2.Right;
	public AnimatedSprite2D Sprite;
	public Area2D EnvArea;
	
	public bool IsDead = false;
	public float DeathTime = 1.0f;
	public float DeathTimer = 0.0f;

	public float InvincibilityTimer = 0f;
	public float InvincibilityDuration = 1.5f;

	public float KnockBackTimer = 0f;
	public float KnockBackDuration = 0.2f;
	public float ReceivedKnockBackStrengthX = 300;
	public float ReceivedKnockBackStrengthY = 500;

	public void Init()
	{
		EnvArea = GetNode<Area2D>("EnvArea");
	}

	public void OnEnvBodyEntered(Node2D body)
	{
		if (body.Name.ToString().Contains("Checkpoint"))
		{
			RespawnLocation = body.GlobalPosition;
		}
	}

	public async void Die()
	{
		Sprite.Hide();
		DeathTimer = DeathTime;
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
		Position = RespawnLocation;
		IsDead = false;
		Sprite.Show();
	}
}
