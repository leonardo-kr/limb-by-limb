using Godot;
using System;
using YourFirstGame.Player;

public partial class LegsPlayer : BasePlayer
{
	[Export] public float CoyoteTime = 0.1f;
	[Export] public float JumpBufferTime = 0.1f;

	[Export] public new Vector2 CameraZoomLevel = new Vector2(4.5f, 4.5f);

	private float _coyoteTimer = 0.0f;
	private float _jumpBufferTimer = 0.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Init();
		UpdateTimers(delta);
		ApplyGravity(delta);

		if (Sprite.Animation == "Death") {
			return;
		}

		HandleJump();
		HandleMovement(delta);

		if (Input.IsActionPressed("die"))
		{
			Die();
		}

		MoveAndSlide();
	}

	private void UpdateTimers(double delta)
	{
		if (IsOnFloor())
		{
			_coyoteTimer = CoyoteTime;
		}
		else
		{
			_coyoteTimer = Mathf.Max(_coyoteTimer - (float)delta, 0.0f);
		}

		_jumpBufferTimer = Mathf.Max(_jumpBufferTimer - (float)delta, 0.0f);
		InvincibilityTimer = Mathf.Max(InvincibilityTimer - (float)delta, 0.0f);
	}

	private void ApplyGravity(double delta)
	{
		if (IsOnFloor())
		{
			return;
		}

		float gravityScale = GravityScale;

		if (Velocity.Y < 0)
		{
			gravityScale *= FallGravityScale;
		}

		Velocity += GetGravity() * gravityScale * (float)delta;
		Velocity = new Vector2(Velocity.X, Mathf.Min(Velocity.Y, MaxFallSpeed));
	}

	private void HandleMovement(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "look_up", "look_down");
		float movementDirection = Input.GetAxis("move_left", "move_right");

		if (direction != Vector2.Zero)
		{
			if (Math.Abs(direction.Y) >= Math.Abs(direction.X))
			{
				FacingDirection = direction.Y > 0 ? Vector2.Down : Vector2.Up;
			}
			else
			{
				FacingDirection = direction.X > 0 ? Vector2.Right : Vector2.Left;
			}
		}

		// only needed for controller, but doesnt impact keyboard
		// always move at the same speed, by setting the movement director to 1f
		if (movementDirection > 0)
		{
			Sprite.Play("Walk");
			Sprite.FlipH = false;
			movementDirection = 1f;
		}
		else if (movementDirection < 0)
		{
			Sprite.Play("Walk");
			Sprite.FlipH = true;
			movementDirection = -1f;
		}
		else
		{
			Sprite.Play("Idle");
			movementDirection = 0f;
		}

		if (KnockBackTimer > 0f)
		{
			KnockBackTimer -= (float)delta;
		}
		else
		{
			Velocity = new Vector2(movementDirection * Speed, Velocity.Y);
		}
	}

	private void HandleJump()
	{
		if (Input.IsActionJustPressed("jump"))
		{
			_jumpBufferTimer = JumpBufferTime;
		}

		if (_jumpBufferTimer > 0)
		{
			if (IsOnFloor() || _coyoteTimer > 0)
			{
				Velocity = new Vector2(Velocity.X, JumpVelocity);
				_coyoteTimer = 0;
				_jumpBufferTimer = 0;
			}
		}

		if (Input.IsActionJustReleased("jump") && Velocity.Y < 0)
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y * JumpCutMultiplier);
		}
	}
}
