using Godot;
using System.Threading.Tasks; 

public partial class SceneTransition : CanvasLayer
{
	// Enum for transition type
	public enum State
	{
		Fade,
		Scale
	}

	[Export] public State TransitionType { get; set; } = State.Fade;

	private AnimationPlayer _sceneTransitionAnim;
	private ColorRect _dissolveRect;

	public override void _Ready()
	{
		_sceneTransitionAnim = GetNode<AnimationPlayer>("SceneTransitionAnim");
		_dissolveRect = GetNode<ColorRect>("DissolveRect");
		_dissolveRect.Hide();
	}

	// This function can be called from any script via:
	// GetNode<SceneTransition>("/root/SceneTransition").LoadScene(targetScene);
	public async void LoadScene(PackedScene targetScene)
	{
		if (targetScene == null)
		{
			GD.PrintErr("SceneTransition: target scene is null.");
			return;
		}

		switch (TransitionType)
		{
			case State.Fade:
				await TransitionAnimation("fade", targetScene);
				break;
			case State.Scale:
				await TransitionAnimation("scale", targetScene);
				break;
		}
	}

	// Handles the transition animation
	private async Task TransitionAnimation(string animationName, PackedScene scene)
	{
		_sceneTransitionAnim.Play(animationName);
		await ToSignal(_sceneTransitionAnim, AnimationPlayer.SignalName.AnimationFinished);

		GetTree().ChangeSceneToPacked(scene);

		_sceneTransitionAnim.PlayBackwards(animationName);
	}
}
