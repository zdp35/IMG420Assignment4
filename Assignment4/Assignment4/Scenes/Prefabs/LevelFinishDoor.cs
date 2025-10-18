using Godot;

public partial class LevelFinishDoor : Area2D
{
	// Define the next scene to load in the inspector
	[Export] public PackedScene NextScene { get; set; }

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (!body.IsInGroup("Player"))
			return;

		// Play the player's death_tween to simulate entering the door
		GetTree().CallGroup("Player", "death_tween");

		// Transition to next scene (autoload SceneTransition expected)
		var transition = GetNodeOrNull<SceneTransition>("/root/SceneTransition");
		if (transition != null)
		{
			transition.LoadScene(NextScene); // <-- direct C# call (case-sensitive)
		}
		else
		{
			GD.PrintErr("SceneTransition autoload not found. Loading directly.");
			GetTree().ChangeSceneToPacked(NextScene);
		}
	}
}
