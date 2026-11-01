using Godot;
using System;
using System.Collections;
using YourFirstGame.Player;

public partial class PlayerManager : Node
{

	[Export] public Vector2 PlayerStartPos = new Vector2(-377, 19);
	[Export] public PackedScene FullPlayerScene;
	[Export] public PackedScene LegPlayerScene;
	[Export] public PackedScene HeadPlayerScene;

	private Camera2D _camera = null; 

	private BasePlayer _currentPlayer = null; 

	public override void _Ready()
	{
		_currentPlayer = HeadPlayerScene.Instantiate<BasePlayer>();

		AddChild(_currentPlayer);
		_currentPlayer.GlobalPosition = PlayerStartPos;

		_camera = GetNode<Camera2D>("Camera2D");

		_camera.GlobalPosition = _currentPlayer.GlobalPosition;
		_camera.Zoom = _currentPlayer.CameraZoomLevel;
		_camera.MakeCurrent();
	}
	public override void _Process(double delta)
	{
		if (_currentPlayer != null)
		{
			_camera.GlobalPosition = _currentPlayer.GlobalPosition;
		}
	}


	public void LevelUp()
	{
		if (_currentPlayer is HeadPlayer)
		{
			CallDeferred(nameof(replacePlayer), LegPlayerScene);
		} else if (_currentPlayer is LegsPlayer)
		{
			CallDeferred(nameof(replacePlayer), FullPlayerScene);
		} else if (_currentPlayer is FullPlayer)
		{
			return;
		} else
		{
			return;
		}
	}

	private void replacePlayer(PackedScene newPlayerScene)
	{
		Vector2 oldPosition = _currentPlayer.GlobalPosition;
		Vector2 oldVelocity = _currentPlayer.Velocity;
		BasePlayer oldPlayer = _currentPlayer;

		_currentPlayer = newPlayerScene.Instantiate<BasePlayer>();
		AddChild(_currentPlayer);

		_currentPlayer.GlobalPosition = oldPosition;
		_currentPlayer.Velocity = oldVelocity;

		_camera.Zoom = _currentPlayer.CameraZoomLevel;

		oldPlayer.QueueFree();
	} 
}
