using Godot;
using System;
using YourFirstGame.Player;

public partial class HeadPlayer : BasePlayer
{
	// Physics can let the head sink a few pixels into a wall; it gets pushed back out by up to this distance before counting as crushed
	private const float MaxNudge = 6f;

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
		var wanted = WantedState();

		if (wanted != _headState && TryFit(wanted, out var shift))
		{
			SetState(wanted, shift);
		}
		else if (!FitsAtScale(Scale, 0f))
		{
			// Overlapping at the current size: push out of the wall, else squish, else the head is crushed
			if (TryNudge(out var nudge))
			{
				Position += new Vector2(nudge, 0f);
			}
			else if (_headState != HeadState.SQUISHED && TryFit(HeadState.SQUISHED, out var squishShift))
			{
				SetState(HeadState.SQUISHED, squishShift);
			}
			else
			{
				Die();
			}
		}
	}

	private HeadState WantedState()
	{
		if (Input.IsActionPressed("look_up"))
		{
			return HeadState.STRETCHED;
		}

		if (Input.IsActionPressed("look_down"))
		{
			return HeadState.SQUISHED;
		}

		// An overhang just above the head squishes it automatically
		if (isUnderCeiling())
		{
			return HeadState.SQUISHED;
		}

		return HeadState.NORMAL;
	}

	private Vector2 ScaleFor(HeadState state) => state switch
	{
		HeadState.STRETCHED => new Vector2(_baseScale / 2f, _baseScale * 1.5f),
		HeadState.SQUISHED => new Vector2(_baseScale * 1.5f, _baseScale / 2f),
		_ => new Vector2(_baseScale, _baseScale),
	};

	private Vector2 SizeFor(Vector2 scale) => ((RectangleShape2D)Collision.Shape).Size * Collision.Scale * scale;

	// Tries the new size centred, then shifted away from either side by the extra width
	private bool TryFit(HeadState state, out float shiftX, float margin = 1f)
	{
		var targetScale = ScaleFor(state);
		var growth = Mathf.Max(0f, (SizeFor(targetScale).X - SizeFor(Scale).X) / 2f);

		foreach (var candidate in new[] { 0f, growth, -growth })
		{
			if (FitsAtScale(targetScale, candidate, margin))
			{
				shiftX = candidate;
				return true;
			}
		}

		shiftX = 0f;
		return false;
	}

	// Finds the smallest sideways push that gets the current size out of any overlap
	private bool TryNudge(out float shiftX)
	{
		for (var distance = 1f; distance <= MaxNudge; distance += 1f)
		{
			foreach (var candidate in new[] { -distance, distance })
			{
				if (FitsAtScale(Scale, candidate))
				{
					shiftX = candidate;
					return true;
				}
			}
		}

		shiftX = 0f;
		return false;
	}

	private void SetState(HeadState state, float shiftX)
	{
		var oldHeight = SizeFor(Scale).Y;
		var newScale = ScaleFor(state);
		var newHeight = SizeFor(newScale).Y;

		Scale = newScale;
		// Scaling happens around the centre; keep the feet in place and step away from walls if needed
		Position += new Vector2(shiftX, -(newHeight - oldHeight) / 2f);
		_headState = state;

		if (state == HeadState.STRETCHED)
		{
			Sprite.Play("Idle");
			Velocity = new Vector2(0f, Velocity.Y);
		}

		// The rays scale with the head; stretch/shrink them so they reach the same height above the normal head in every state
		var reach = state switch
		{
			HeadState.SQUISHED => 2.5f,
			HeadState.STRETCHED => 0.5f,
			_ => 1f,
		};
		_ceilingDetectionLeft.TargetPosition = _ceilingDetectionTargetPosition * reach;
		_ceilingDetectionRight.TargetPosition = _ceilingDetectionTargetPosition * reach;
	}

	// Would the collision box at this scale overlap anything, with the feet kept at their current height?
	private bool FitsAtScale(Vector2 targetScale, float shiftX, float margin = 1f)
	{
		var currentSize = SizeFor(Scale);
		var targetSize = SizeFor(targetScale);

		// The collision box is slightly off-centre, and that offset scales with the head
		var bottom = GlobalPosition.Y + Collision.Position.Y * Scale.Y + currentSize.Y / 2f;
		var centerX = GlobalPosition.X + Collision.Position.X * targetScale.X + shiftX;
		var center = new Vector2(centerX, bottom - targetSize.Y / 2f);

		var query = new PhysicsShapeQueryParameters2D
		{
			// Shrunk by the margin and lifted slightly, so touching the floor or a wall doesn't count
			Shape = new RectangleShape2D { Size = targetSize - Vector2.One * margin },
			Transform = new Transform2D(0f, center - new Vector2(0f, margin / 2f)),
			CollisionMask = CollisionMask,
			Exclude = new Godot.Collections.Array<Rid> { GetRid() },
		};
		return GetWorld2D().DirectSpaceState.IntersectShape(query, 1).Count == 0;
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
