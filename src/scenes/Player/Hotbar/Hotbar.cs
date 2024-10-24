using Godot;
using Meowc;
using System;
using System.Linq;

namespace Meowc.Scenes.Player;

public partial class Hotbar : Panel
{

    private const string StyleBoxOverrideName = "panel"; // idk wtf this is but it has to be panel

    private int _selectedIndex;
    private Panel[] _panels = null!;
    private StyleBox _defaultHotbarPanelStyleBox = null!;
    private StyleBox _selctedHotbarPanelStyleBox = null!;

    public int SelectedIndex => _selectedIndex;

    public override void _Ready()
    {
        var hbox = Helpers.GetNodeOrThrow<HBoxContainer>(this, "HBoxContainer");
        _panels = hbox.GetChildren().Cast<Panel>().ToArray();

        _defaultHotbarPanelStyleBox = GD.Load<StyleBox>("res://scenes/default_hotbar_panel.tres");
        _selctedHotbarPanelStyleBox = GD.Load<StyleBox>("res://scenes/selected_hotbar_panel.tres");

        _panels[0].AddThemeStyleboxOverride(StyleBoxOverrideName, _selctedHotbarPanelStyleBox);

        foreach (var panel in _panels.Skip(1))
        {
            panel.AddThemeStyleboxOverride(StyleBoxOverrideName, _defaultHotbarPanelStyleBox);
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased(Globals.InputMap.HotbarNext))
        {
            SelectNextPanel();
        }
        else if (Input.IsActionJustReleased(Globals.InputMap.HotbarPrev))
        {
            SelectPrevPanel();
        }

        void SelectNextPanel()
        {
            int index = _selectedIndex;
            index++;
            if (index >= _panels.Length)
            {
                index = 0;
            }
            SelectPanel(index);
        }

        void SelectPrevPanel()
        {
            int index = _selectedIndex;
            index--;
            if (index < 0)
            {
                index = _panels.Length - 1;
            }
            SelectPanel(index);
        }
    }

    private void SelectPanel(int index)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, _panels.Length - 1);

        var prev = _panels[_selectedIndex];
        prev.RemoveThemeStyleboxOverride(StyleBoxOverrideName);
        prev.AddThemeStyleboxOverride(StyleBoxOverrideName, _defaultHotbarPanelStyleBox);

        _selectedIndex = index;

        var cur = _panels[_selectedIndex];
        cur.RemoveThemeStyleboxOverride(StyleBoxOverrideName);
        cur.AddThemeStyleboxOverride(StyleBoxOverrideName, _selctedHotbarPanelStyleBox);

    }
}
