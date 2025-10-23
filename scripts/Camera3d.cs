using Godot;
using System;

public partial class Camera3d : Camera3D
{
	[Export] public float Speed = 5.0f;

	public override void _Process(double delta)
	{
		Vector3 direction = Vector3.Zero;

		if (Input.IsActionPressed("move_forward"))
			direction -= Transform.Basis.Z;
		if (Input.IsActionPressed("move_back"))
			direction += Transform.Basis.Z;
		if (Input.IsActionPressed("move_left"))
			direction -= Transform.Basis.X;
		if (Input.IsActionPressed("move_right"))
			direction += Transform.Basis.X;

		direction.Y = 0;
		direction = direction.Normalized();
		GlobalTranslate(direction * Speed * (float)delta);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			if (!Input.IsMouseButtonPressed(MouseButton.Left) &&
			!Input.IsMouseButtonPressed(MouseButton.Right))
			{
				RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * 0.1f));

				var cameraRotation = Rotation;
				cameraRotation.X -= Mathf.DegToRad(mouseMotion.Relative.Y * 0.1f);
				cameraRotation.X = Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
				Rotation = cameraRotation;
			}
		}
		if (@event is InputEventKey keyEvent)
		{
			if (keyEvent.Pressed && keyEvent.Keycode == Key.Space)
			{
				LookAt(Vector3.Zero, Vector3.Up);
			}
		}
	}
}
