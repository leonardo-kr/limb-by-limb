using Godot;

namespace YourFirstGame.Player;

public abstract partial class BasePlayer : CharacterBody2D
{
	[Export] public int Health = 10;
	[Export] public float Speed = 150.0f;
	
	// Gravity stuff
	[Export] public float GravityScale = 1f;
	[Export] public float FallGravityScale = 1f;
	[Export] public float MaxFallSpeed = 300.0f;
	
	public CollisionShape2D Collision;
	public Vector2 FacingDirection = Vector2.Right;
	public AnimatedSprite2D Sprite;

	public float InvincibilityTimer = 0f;
	public float InvincibilityDuration = 1.5f;

	public float KnockBackTimer = 0f;
	public float KnockBackDuration = 0.2f;
	public float ReceivedKnockBackStrengthX = 300;
	public float ReceivedKnockBackStrengthY = 500;
}
