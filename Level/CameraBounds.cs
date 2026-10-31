using Godot;
using System;

// Draw this rectangle over the level art in the editor; at runtime the active camera stays inside it
public partial class CameraBounds : ReferenceRect
{
	public override void _Ready()
	{
		var camera = GetViewport().GetCamera2D();
		if (camera == null)
		{
			GD.PushWarning("CameraBounds: no active Camera2D found");
			return;
		}

		var bounds = GetGlobalRect();
		camera.LimitLeft = Mathf.RoundToInt(bounds.Position.X);
		camera.LimitTop = Mathf.RoundToInt(bounds.Position.Y);
		camera.LimitRight = Mathf.RoundToInt(bounds.End.X);
		camera.LimitBottom = Mathf.RoundToInt(bounds.End.Y);
	}
}
