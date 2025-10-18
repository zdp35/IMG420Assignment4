using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class Player : CharacterBody2D
{
	// --------- VARIABLES ---------- //

	[ExportGroup("Player Properties")]
	[Export] public float MoveSpeed { get; set; } = 400f;
	[Export] public float JumpForce { get; set; } = 600f;
	[Export] public float Gravity { get; set; } = 30f;
	[Export] public int MaxJumpCount { get; set; } = 2;

	private int _jumpCount = 2;

	[ExportGroup("Toggle Functions")]
	[Export] public bool DoubleJump { get; set; } = false;

	// Kept for parity with original (not otherwise used)
	private bool _isGrounded = false;

	private AnimatedSprite2D _playerSprite;
	private Node2D _spawnPoint;
	private Node _particleTrails;  // Works with either CPU/GPU particles
	private Node _deathParticles;

	// --------- BUILT-IN FUNCTIONS ---------- //

	public override void _Ready()
	{
		_playerSprite   = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_spawnPoint     = GetNode<Node2D>("%SpawnPoint");     // Unique name lookup
		_particleTrails = GetNode<Node>("ParticleTrails");
		_deathParticles = GetNode<Node>("DeathParticles");
		// Collision signal is expected to be connected in the editor to _on_collision_body_entered
	}

	public override void _Process(double delta)
	{
		Movement();
		PlayerAnimations();
		FlipPlayer();
	}

	// --------- CUSTOM FUNCTIONS ---------- //

	// <-- Player Movement Code -->
	private void Movement()
	{
		// Gravity
		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity);
			_isGrounded = false;
		}
		else
		{
			_jumpCount = MaxJumpCount;
			_isGrounded = true;
		}

		HandleJumping();

		// Move Player
		var inputAxis = Input.GetAxis("Left", "Right");
		Velocity = new Vector2((float)inputAxis * MoveSpeed, Velocity.Y);
		MoveAndSlide();
	}

	// Handles jumping functionality (double jump or single jump, can be toggled from inspector)
	private void HandleJumping()
	{
		if (Input.IsActionJustPressed("Jump"))
		{
			if (IsOnFloor() && !DoubleJump)
			{
				Jump();
			}
			else if (DoubleJump && _jumpCount > 0)
			{
				Jump();
				_jumpCount -= 1;
			}
		}
	}

	// Player jump
	private void Jump()
	{
		JumpTween();

		// AudioManager.jump_sfx.play()
		var audioManager = GetNodeOrNull<Node>("/root/AudioManager");
		audioManager?.GetNodeOrNull<AudioStreamPlayer>("jump_sfx")?.Play();

		Velocity = new Vector2(Velocity.X, -JumpForce);
	}

	// Handle Player Animations
	private void PlayerAnimations()
	{
		// particle_trails.emitting = false
		SetParticlesEmitting(_particleTrails, false);

		if (IsOnFloor())
		{
			if (Mathf.Abs(Velocity.X) > 0f)
			{
				SetParticlesEmitting(_particleTrails, true);
				_playerSprite.SpeedScale = 1.5f;    // play("Walk", 1.5)
				_playerSprite.Play("Walk");
			}
			else
			{
				_playerSprite.SpeedScale = 1.0f;
				_playerSprite.Play("Idle");
			}
		}
		else
		{
			_playerSprite.SpeedScale = 1.0f;
			_playerSprite.Play("Jump");
		}
	}

	// Flip player sprite based on X velocity
	private void FlipPlayer()
	{
		if (Velocity.X < 0f)
			_playerSprite.FlipH = true;
		else if (Velocity.X > 0f)
			_playerSprite.FlipH = false;
	}

	// Tween Animations
	private async Task DeathTween()
	{
		var tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.15);
		await ToSignal(tween, Tween.SignalName.Finished);

		if (_spawnPoint != null)
			GlobalPosition = _spawnPoint.GlobalPosition;

		await ToSignal(GetTree().CreateTimer(0.3), SceneTreeTimer.SignalName.Timeout);

		// AudioManager.respawn_sfx.play()
		var audioManager = GetNodeOrNull<Node>("/root/AudioManager");
		audioManager?.GetNodeOrNull<AudioStreamPlayer>("respawn_sfx")?.Play();

		RespawnTween();
	}

	private void RespawnTween()
	{
		var tween = CreateTween();
		tween.Stop();
		tween.Play();
		tween.TweenProperty(this, "scale", Vector2.One, 0.15);
	}

	private void JumpTween()
	{
		var tween = CreateTween();
		tween.TweenProperty(this, "scale", new Vector2(0.7f, 1.4f), 0.1);
		tween.TweenProperty(this, "scale", Vector2.One, 0.1);
	}

	// --------- SIGNALS ---------- //

	// Reset the player's position to the current level spawn point if collided with any trap
	public async void _on_collision_body_entered(Node body)
	{
		if (!body.IsInGroup("Traps"))
			return;

		// AudioManager.death_sfx.play()
		var audioManager = GetNodeOrNull<Node>("/root/AudioManager");
		audioManager?.GetNodeOrNull<AudioStreamPlayer>("death_sfx")?.Play();

		// death_particles.emitting = true
		SetParticlesEmitting(_deathParticles, true);

		await DeathTween();
	}

	// --------- HELPERS ---------- //

	// Minimal, engine-agnostic particles toggle (no GPUParticles2D/CPUParticles2D types)
	private static void SetParticlesEmitting(Node particlesNode, bool emitting)
	{
		if (particlesNode == null) return;
		particlesNode.Set("emitting", emitting);
	}

	// --------- PUBLIC API ---------- //

	// External callers (e.g., Enemy Area2D) can force a respawn.
	public void Respawn()
	{
		// Full animated reset (SFX + particles + tween) to %SpawnPoint
		_ = DeathTween();

		// If you want an instant snap instead, replace the line above with:
		// if (_spawnPoint != null) GlobalPosition = _spawnPoint.GlobalPosition;
		// Velocity = Vector2.Zero;
		// RespawnTween();
	}
}
