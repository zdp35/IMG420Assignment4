using Godot;

public partial class GameManager : Node2D
{
	// Global score variable
	public int Score { get; private set; } = 0;

	// Adds 1 to score variable
	public void AddScore()
	{
		Score += 1;
	}

	// Loads next level
	public void LoadNextLevel(PackedScene nextScene)
	{
		if (nextScene == null)
		{
			GD.PrintErr("GameManager: Tried to load a null scene!");
			return;
		}

		GetTree().ChangeSceneToPacked(nextScene);
	}
}
