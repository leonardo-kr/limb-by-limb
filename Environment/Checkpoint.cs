using Godot;
using System;

public partial class Checkpoint : StaticBody2D
{
	[Export] public bool isActive = false;

	[Export] public Texture2D frontTexture = null;
	[Export] public Texture2D frontTextureActive = null;

	private Sprite2D _spriteFG = null;

	public override void _Ready()
	{
		_spriteFG = GetNode<Sprite2D>("SpriteFG");
		_spriteFG.Texture = frontTexture;
	}

	public void SetActive()
	{
		isActive = true;
		
		_spriteFG.Texture = frontTextureActive;
	}
}
