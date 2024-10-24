using System.Collections.Generic;

namespace Meowc.Data;

public sealed class Manifest
{
	public static IReadOnlyList<Block> Blocks { get; } = 
	[
		new GrassBlock(),
		new StoneBlock(),
	];

	public static IReadOnlyList<Tool> Tools { get; } =
	[
		new ShovelTool(),
		new PickaxeTool(),
	];
}

public abstract class Item(string name, string description, string iconPath)
{
	public string Name { get; } = name;
	public string Description { get; } = description;
	public string IconPath { get; } = iconPath;
}


public abstract class Block(string name, string description, string iconPath, string scenePath)
	: Item(name, description, iconPath)
{
	public string ScenePath { get; } = scenePath;
}

public sealed class GrassBlock() : Block("Grass block", "A block of grass", "res://Art/Sprites/grassblock.png", "res://Scenes/Blocks/GrassBlock.tscn");
public sealed class StoneBlock() : Block("Stone block", "A block of stone", "res://Art/Sprites/stoneblock.png", "res://Scenes/Blocks/StoneBlock.tscn");


public abstract class Tool(string name, string description, string iconPath) : Item(name, description, iconPath);

public sealed class ShovelTool() : Tool("Shovel", "A thing for grass", "res://Art/Sprites/StoneShovel.png");
public sealed class PickaxeTool() : Tool("Pickaxe", "A thing for stone", "res://Art/Sprites/StonePickaxe.png");
