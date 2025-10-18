using Godot;

public partial class GameUI : Control   // ← match this to your file name
{
	private TextureRect _scoreTexture;
	private Label _scoreLabel;

	public override void _Ready()
	{
		// Same as @onready var score_texture = %Score/ScoreTexture
		_scoreTexture = GetNode<TextureRect>("%Score/ScoreTexture");
		_scoreLabel = GetNode<Label>("%Score/ScoreLabel");
	}

	public override void _Process(double delta)
	{
		// Update label text from GameManager.score
		var gameManager = GetNodeOrNull<GameManager>("/root/GameManager");
		if (gameManager != null)
		{
			_scoreLabel.Text = $"x {gameManager.Score}";
		}
	}
}
