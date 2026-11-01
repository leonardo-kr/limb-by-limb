using Godot;
using System;
using Godot.Collections;
using YourFirstGame.Player;

public partial class HeadPlayer : BasePlayer
{
	private HeadState _headState = HeadState.NORMAL;
	private float _baseScale = 0.1f;
	private RayCast2D _ceilingDetectionLeft;
	private RayCast2D _ceilingDetectionRight;
	private Vector2 _ceilingDetectionTargetPosition;
	
	public override void _Ready()
	{
		Init();
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
		_ceilingDetectionLeft = GetNode<RayCast2D>("CeilingDetectionLeft");
		_ceilingDetectionRight = GetNode<RayCast2D>("CeilingDetectionRight");
		_ceilingDetectionTargetPosition = _ceilingDetectionLeft.TargetPosition;
	}
	
	public override void _PhysicsProcess(double delta)
	{

		ApplyGravity(delta);

		if (Sprite.Animation == "Death") {
			return;
		}

		HandleMovement(delta);
		HandleInput();
		CheckDeath();

		if (Input.IsActionPressed("die"))
		{
			Die();
			return;
		}

		MoveAndSlide();
	}

	private void UpdateTimers(double delta)
	{
		InvincibilityTimer = Mathf.Max(InvincibilityTimer - (float) delta, 0.0f);
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
		
		Velocity += GetGravity() * gravityScale * (float) delta;
		Velocity = new Vector2(Velocity.X, Mathf.Min(Velocity.Y, MaxFallSpeed));
	}

	private void HandleMovement(double delta)
	{
		if (_headState == HeadState.STRETCHED)
		{
			return;
		}
		
		Vector2 direction = Input.GetVector("move_left", "move_right", "look_up", "look_down");
		float movementDirection = Input.GetAxis("move_left", "move_right");

		if (direction != Vector2.Zero)
		{
		
			if (Math.Abs(direction.Y) >= Math.Abs(direction.X))
			{
				FacingDirection = direction.Y > 0 ? Vector2.Down : Vector2.Up;
			} else
			{
				FacingDirection = direction.X > 0 ? Vector2.Right : Vector2.Left;
			}
		}

		// only needed for controller, but doesnt impact keyboard
		// always move at the same speed, by setting the movement director to 1f 
		if (movementDirection > 0)
		{
			Sprite.FlipH = false;
			movementDirection = 1f;
			Sprite.Play("roll");
		} else if (movementDirection < 0) {
			Sprite.FlipH = true;
			movementDirection = -1f;
			Sprite.Play("roll");
		} else
		{
			movementDirection = 0f;
			Sprite.Play("Idle");
		}
		
		if (KnockBackTimer > 0f)
		{
			KnockBackTimer -= (float)delta;
		} else
		{	
			float speedModifier = _headState == HeadState.NORMAL ? 1.0f : 0.5f;
			Velocity = new Vector2(movementDirection * Speed * speedModifier, Velocity.Y);
		}
	}

	private void HandleInput()
	{
		if (isUnderCeiling())
		{
			Squish();
		}
		else if (Input.IsActionPressed("look_up"))
		{
			Stretch();
		} else if (Input.IsActionPressed("look_down"))
		{
			Squish();
		}
		else
		{
			Scale = new Vector2(_baseScale, _baseScale);
			_headState = HeadState.NORMAL;
			_ceilingDetectionLeft.TargetPosition = _ceilingDetectionTargetPosition;
		}
	}

	private void Squish()
	{
		Scale = new Vector2(_baseScale * 1.5f, _baseScale / 2.0f);
		_headState = HeadState.SQUISHED;
		_ceilingDetectionLeft.TargetPosition = _ceilingDetectionTargetPosition * 2.5f;
	}

	private void Stretch()
	{
		Scale = new Vector2(_baseScale / 2.0f, _baseScale * 1.5f);
		_headState = HeadState.STRETCHED;
		Sprite.Play("Idle");
		Velocity = Vector2.Zero;
		_ceilingDetectionLeft.TargetPosition = _ceilingDetectionTargetPosition * 0.5f;
	}

	private void CheckDeath()
	{
		if (!IsDead && (_ceilingDetectionLeft.IsColliding() || _ceilingDetectionRight.IsColliding()))
		{
			PhysicsDirectSpaceState2D state = GetWorld2D().DirectSpaceState;
			PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(Position, Position + Vector2.Up);
			Dictionary result = state.IntersectRay(query);
			if (result.Count > 0)
			{
				IsDead = true;
				Die();
			}
		}
	}

	bool isUnderCeiling()
	{
		return _ceilingDetectionLeft.IsColliding() || _ceilingDetectionRight.IsColliding();
	}
}

public enum HeadState
{
	NORMAL,
	STRETCHED,
	SQUISHED,
}
