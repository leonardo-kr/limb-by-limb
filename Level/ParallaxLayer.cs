using Godot;

// Moves a background/foreground layer relative to the camera.
// Factor 1 = moves with the world, 0 = stays fixed on screen, >1 = moves faster (foreground).
public partial class ParallaxLayer : Node2D
{
	[Export] public Vector2 Factor = new(0.5f, 0.5f);

	// World position of the camera at which the layer sits exactly as the artist composed it
	[Export] public Vector2 AlignAt = Vector2.Zero;

	private Vector2 _origin;

	public override void _Ready()
	{
		_origin = Position;
		// Run after the camera has moved this frame, otherwise the layer lags one frame behind
		ProcessPriority = 1000;
	}

	public override void _Process(double delta)
	{
		var camera = GetViewport().GetCamera2D();
		if (camera == null)
		{
			return;
		}

		var cameraOffset = camera.GetScreenCenterPosition() - AlignAt;
		Position = _origin + cameraOffset * (Vector2.One - Factor);
	}
}
