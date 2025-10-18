using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 300f;
	[Export] public float Gravity { get; set; } = 980f;

	private int _dir = 1;                    // 1 = right, -1 = left
	private Sprite2D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");

		// Detect player overlap via Area2D
		var area = GetNode<Area2D>("Area2D");
		area.BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Basic ground movement
		var v = Velocity;
		v.Y += Gravity * (float)delta;       // stay grounded on slopes
		v.X = Speed * _dir;
		Velocity = v;

		MoveAndSlide();

		// If we collided laterally this frame, flip direction
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var col = GetSlideCollision(i);
			// Large X-normal ⇒ hit a wall (left/right)
			if (Mathf.Abs(col.GetNormal().X) > 0.7f)
			{
				Flip();
				break;
			}
		}
	}

	private void Flip()
	{
		_dir *= -1;
		if (_sprite != null) _sprite.FlipH = (_dir < 0);
	}

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			// Expect the player to implement a Respawn() method
			body.Call("Respawn");
		}
	}
}
