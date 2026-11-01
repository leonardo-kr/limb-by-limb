using Godot;
using System;

public partial class End : Node2D
{
	private ColorRect _colorRect;
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
		_colorRect = GetNode<ColorRect>("CanvasLayer/ColorRect");
		_colorRect.Modulate = new Color(0, 0, 0, 0);
	}

	public override void _Process(double delta)
	{
	}

	public void BodyEntered2D(Node2D body)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(_colorRect, "modulate", new Color(0, 0, 0, 1), 0.5f);
	}
}
