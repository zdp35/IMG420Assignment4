using Godot;

public partial class Coin : Area2D
{
	[Export] public float Amplitude { get; set; } = 4f;
	[Export] public float Frequency { get; set; } = 5f;

	private float _timePassed = 0f;
	private Vector2 _initialPosition = Vector2.Zero;

	public override void _Ready()
	{
		_initialPosition = Position;
		BodyEntered += OnBodyEntered; // replaces _on_body_entered
	}

	public override void _Process(double delta)
	{
		_timePassed += (float)delta;
		float newY = _initialPosition.Y + Amplitude * Mathf.Sin(Frequency * _timePassed);
		Position = new Vector2(Position.X, newY);
	}

	private async void OnBodyEntered(Node body)
	{
		if (!body.IsInGroup("Player"))
			return;


		GetNodeOrNull<GameManager>("/root/GameManager")?.AddScore();

		var tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.1);
		await ToSignal(tween, Tween.SignalName.Finished);
		QueueFree();
	}
}
