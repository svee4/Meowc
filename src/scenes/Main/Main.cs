using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Meowc;

public partial class Main : Node3D
{
	
	[Export]
	public int GeneratedBlocksCount { get; set; }

    private Label _debugLabel = null!;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Engine.MaxFps = Globals.FrameRateCap;
        _debugLabel = new Label();
        Helpers.GetNodeOrThrow<BoxContainer>(this, "DebugInfoContainer").AddChild(_debugLabel);
        GenerateWorld();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        var fps = Engine.GetFramesPerSecond();
        _debugLabel.Text = $"Avg: {fps:000}";
	}

	private void GenerateWorld()
	{
        var blocksNode = Helpers.GetNodeOrThrow<Node>(this, "Blocks");

        var allBlocksFilenames = Directory.GetFiles(ProjectSettings.GlobalizePath("res://Scenes/Blocks"));
        var allBlockResources = allBlocksFilenames
            .Select(GD.Load<PackedScene>)
            .ToList();

        var generatedBlockLocations = new List<(float x, float z)>();

        // create center block
        var center = allBlockResources.First().Instantiate();
        ((Node3D)center).Position = new Vector3(0, 0, 0);
        AddChild(center);
        generatedBlockLocations.Add((0f, 0f));

        for (int i = 0; i < GeneratedBlocksCount; i++)
        {
            var node = allBlockResources[Random.Shared.Next(0, allBlockResources.Count)].Instantiate();

            float newx = 0f, newz = 0f;
            do
            {
                var nextTo = generatedBlockLocations[Random.Shared.Next(0, generatedBlockLocations.Count)];
                (newx, newz) = Random.Shared.Next(0, 4) switch
                {
                    0 => (nextTo.x + 1, nextTo.z),
                    1 => (nextTo.x - 1, nextTo.z),
                    2 => (nextTo.x, nextTo.z + 1),
                    3 => (nextTo.x, nextTo.z - 1),
                    _ => throw new Exception()
                };

                GD.Print($"{i}: Got coordinates: ({newx}, {newz})");
            } while (generatedBlockLocations.Any(loc => loc.x == newx && loc.z == newz));

            GD.Print($"{i}: Final coordinates: ({newx}, {newz})");
            generatedBlockLocations.Add((newx, newz));

            ((Node3D)node).Position = new Vector3(newx, 0, newz);
            AddChild(node);
        }
    }
}
