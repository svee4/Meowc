using Godot;
using System;
using System.Diagnostics.CodeAnalysis;
using Meowc;
using Meowc.Data;

namespace Meowc.Scenes.Player;

[Tool]
public partial class InventoryTile : Control
{

	private Data.Item? _item;

	public Data.Item? Item
	{
		get => _item;
		set
		{
			_item = value;
			SetTextureFromItem();
		}
	}


	[MemberNotNullWhen(false, nameof(Item))]
	public bool IsEmptyTile => Item is null;
	
	public Action<InventoryTile>? OnClick { get; set; }
	
	public override void _Ready()
	{
		SetTextureFromItem();
	}

	private void SetTextureFromItem()
	{
		var path = Item is null
			? "res://Art/Sprites/EmptyTile.png"
			: Item.IconPath ?? "res://Art/Sprites/MissingTexture.png";
		
		var textureNode = Helpers.GetNodeOrThrow<TextureRect>(this, "Panel/MarginContainer/TextureRect");
		var texture = GD.Load<Texture2D>(path);
		textureNode.Texture = texture;
	}
	
	public override void _GuiInput(InputEvent @event)
	{
		if (Engine.IsEditorHint())
		{
			return;
		}
		
		if (@event.IsActionPressed(Globals.InputMap.UI.PrimaryInput))
		{
			GetViewport().SetInputAsHandled();
			OnClick?.Invoke(this);
		}
	}
}
