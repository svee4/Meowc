using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Meowc.Data;

namespace Meowc.Scenes.Player;

[Tool]
public partial class Inventory : Control
{
	public const int Width = 8;
	public const int Height = 6;

	private PackedScene _tileScene = null!;
	private GridContainer _tileContainer = null!;
	private HBoxContainer _hotbarContainer = null!;
	private GrabbedItemControl? _grabbedItemControl;
	
	public override void _Ready()
	{
		_tileScene = GD.Load<PackedScene>("res://Scenes/Player/Inventory/InventoryTile.tscn");
		_tileContainer = this.GetNodeOrThrow<GridContainer>("VBoxContainer/Inventory/MarginContainer/TileContainer");
		
		for (var y = 0; y < Height; y++)
		{
			for (var x = 0; x < Width; x++)
			{
				var tile = InstantiateTile();
				tile.OnClick = OnTileClick;
				_tileContainer.AddChild(tile);
			}
		}

		_tileContainer.GetChild<InventoryTile>(1).Item = new GrassBlock();

		_hotbarContainer = this.GetNodeOrThrow<HBoxContainer>("VBoxContainer/Hotbar/MarginContainer/HBoxContainer");
		for (var i = 0; i < 8; i++)
		{
			var tile = InstantiateTile();
			tile.OnClick = OnTileClick;
			_hotbarContainer.AddChild(tile);
		}

		if (Engine.IsEditorHint()) return;

		Input.SetMouseMode(Input.MouseModeEnum.Confined);
	}

	private void OnTileClick(InventoryTile tile)
	{
		if (Engine.IsEditorHint())
		{
			return;
		}
		
		if (_grabbedItemControl is not null)
		{
			PlaceDownItemToTile(tile);
		}
		else
		{
			PickUpItemFromTile(tile);
		}
	}

	private void PlaceDownItemToTile(InventoryTile tile) 
	{
		if (_grabbedItemControl is null)
		{
			throw new Exception();
		}
		
		var itemToPlaceDown = _grabbedItemControl.GrabbedItem;
		
		_grabbedItemControl.QueueFree();
		_grabbedItemControl = null;
		
		if (!tile.IsEmptyTile)
		{
			PickUpItemFromTile(tile);
		}
		
		tile.Item = itemToPlaceDown;
	}

	private void PickUpItemFromTile(InventoryTile tile)
	{
		if (_grabbedItemControl is not null)
		{
			throw new Exception();
		}
		
		if (tile.IsEmptyTile)
		{
			return;
		}
		
		_grabbedItemControl = new GrabbedItemControl(tile.Item);
		AddChild(_grabbedItemControl);

		tile.Item = null;
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (Engine.IsEditorHint()) return;

		if (_grabbedItemControl is not null && @event is InputEventMouseMotion mouseMotion)
		{
			var pos = mouseMotion.GlobalPosition;
			_grabbedItemControl.SetGlobalPosition(pos);
		}
		
		if (@event is InputEventMouseButton eventMouseButton)
		{
			// eventMouseButton.
		}
	}

	private InventoryTile InstantiateTile() => (InventoryTile)_tileScene.Instantiate();
	
	private partial class GrabbedItemControl : Control
	{
		public Item GrabbedItem { get; }

		public GrabbedItemControl(Item grabbedItem)
		{
			GrabbedItem = grabbedItem;
			
			var textureRect = new TextureRect { Texture = GD.Load<Texture2D>(grabbedItem.IconPath) };
			AddChild(textureRect);
			
			MouseFilter = MouseFilterEnum.Ignore;
			textureRect.MouseFilter = MouseFilterEnum.Ignore;
		}
	}
}
