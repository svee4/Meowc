using Godot;
using Meowc;
using Meowc.Components;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public partial class Player : CharacterBody3D
{
    [Export]
    public float Speed { get; set; } = 3;

    [Export]
    public float SprintModifier { get; set; } = 1.5f;

    [Export]
    public float JumpVelocity { get; set; } = 5;

    [Export]
    public int Sensitivity { get; set; } = 25;

    [Export]
    public int Reach { get; set; } = 5;

    private readonly float Gravity = Globals.Gravity;

    private PackedScene _raycastTestScene = null!;
    private Node3D _pivot = null!;
    private Camera3D _camera = null!;
    private Label _debugInfoLabel = null!;
    private bool _primaryInputHeld;
    private bool _secondaryInputHeld;

    public override void _Ready()
    {
        _raycastTestScene = GD.Load<PackedScene>("res://scenes//RaycastTest.tscn");
        _pivot = Helpers.GetNodeOrThrow<Node3D>(this, "Pivot");
        _camera = Helpers.GetNodeOrThrow<Camera3D>(this, "Pivot/Camera3D");

        _debugInfoLabel = new Label();
        Helpers.GetNodeOrThrow<BoxContainer>(this, "../DebugInfoContainer")
            .AddChild(_debugInfoLabel);
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        HandleMouseAim(@event);
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement(delta);
        HandleInputs(delta);
    }

    private void HandleInputs(double delta)
    {
        // https://www.youtube.com/watch?v=uH6yeUUVQdE&list=PLEHvj4yeNfeF6s-UVs5Zx5TfNYmeCiYwf&index=24



        var spaceState = _camera.GetWorld3D().DirectSpaceState;
        var screenCenter = GetViewport().GetVisibleRect().Size / 2;

        var rayOrigin = _camera.ProjectRayOrigin(screenPoint: screenCenter);
        var rayEnd = rayOrigin + _camera.ProjectRayNormal(screenPoint: screenCenter) * Reach;

        var query = PhysicsRayQueryParameters3D.Create(from: rayOrigin, to: rayEnd);
        query.CollideWithBodies = true;

        var result = spaceState.IntersectRay(query);

        if (Input.IsActionPressed(Globals.InputMap.PrimaryInput))
        {
            if (!_primaryInputHeld)
            {
                // handle single click
                _primaryInputHeld = true;

                if (result.TryGetValue("position", out var position))
                {
                    var pos = (Vector3)position;
                    var dot = (Node3D)_raycastTestScene.Instantiate();
                    dot.GlobalPosition = pos;
                    GetTree().Root.AddChild(dot);
                    _ = Task.Delay(500).ContinueWith(_ => dot?.QueueFree());
                }

                if (result.TryGetValue("collider", out var collider))
                {
                    var node = ((Node)collider).GetSceneRoot();

                    if (node.TryGetComponent<BreakableBlockComponent>(out var component))
                    {
                        component.Break();
                    }
                }
            }
            else
            {
                // handle holding down
            }
        }
        else
        {
            _primaryInputHeld = false;
        }


        if (Input.IsActionPressed(Globals.InputMap.SecondaryInput))
        {
            if (!_secondaryInputHeld)
            {
                // handle single click
                _secondaryInputHeld = true;

                if (result.TryGetValue("position", out var position))
                {
                    var pos = ((Vector3)position);
                    var dot = (Node3D)_raycastTestScene.Instantiate();
                    dot.GlobalPosition = pos;
                    GetTree().Root.AddChild(dot);
                    _ = Task.Delay(500).ContinueWith(_ => dot?.QueueFree());
                }
            }
            else
            {
                // handle holding down
            }
        }
        else
        {
            _secondaryInputHeld = false;
        }

    }

    private void HandleMovement(double delta)
    {
        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y -= (float)(Gravity * delta);
        }

        if (Input.IsActionPressed("jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        var input = Input.GetVector(
            Globals.InputMap.MoveLeft,
            Globals.InputMap.MoveRight,
            Globals.InputMap.MoveForward,
            Globals.InputMap.MoveBackward);

        var direction = (_pivot.Transform.Basis * new Vector3(x: input.X, y: 0, z: input.Y)).Normalized();
        velocity.X = direction.X * Speed;
        velocity.Z = direction.Z * Speed;

        if (Input.IsActionPressed(Globals.InputMap.Sprint))
        {
            // TODO: only allow sprinting forward, in the direction the camera is looking
        }

        _debugInfoLabel.Text = $"X: {velocity.X:0.00}\nY: {velocity.Y:0.00}\nZ: {velocity.Z:0.00}";

        Velocity = velocity;
        MoveAndSlide();
    }

    private void HandleMouseAim(InputEvent @event)
    {
        // mouse aim
        // https://www.youtube.com/watch?v=v4IEPi1c0eE
        if (@event is InputEventMouseButton)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            // free mouse on ESC
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (Input.MouseMode is Input.MouseModeEnum.Captured && @event is InputEventMouseMotion ev2)
        {
            _pivot.RotateY(-ev2.Relative.X * 0.0001f * Sensitivity);
            _camera.RotateX(-ev2.Relative.Y * 0.0001f * Sensitivity);

            // clamp pivot so it doesnt go around the world
            var prevRotation = _camera.Rotation;
            _camera.Rotation = new Vector3(
                x: Math.Clamp(_camera.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)),
                y: prevRotation.Y,
                z: prevRotation.Z);
        }
    }
}
