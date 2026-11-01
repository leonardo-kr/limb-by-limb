using Godot;
using System;

public partial class PillarMan : CharacterBody2D
{
    [Export] public float MovementSpeed = 25.0f;
    [Export] public float StartOffset = 0.0f;
    [Export] public float MaxPositionChange = 100.0f;
    private Vector2 _startPosition;
    private Vector2 _moveDirection = Vector2.Up;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _startPosition = new Vector2(Position.X, Position.Y + StartOffset);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
        if (_moveDirection == Vector2.Up)
        {
            Velocity = new Vector2(0, -MovementSpeed);
            if (Position.Y < _startPosition.Y - MaxPositionChange)
            {
                _moveDirection = Vector2.Down;
            }
        }
        else
        {
            Velocity = new Vector2(0, MovementSpeed);
            if (Position.Y >= _startPosition.Y)
            {
                _moveDirection = Vector2.Up;
            }
        }
        
        MoveAndSlide();
    }
}
