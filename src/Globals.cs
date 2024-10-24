using Godot;
using System;

namespace Meowc;

public static class Globals
{
    public static float Gravity => 15;

    public static int FrameRateCap => 500;

    public enum Gamemode
    {
        Creative = 1,
        Survival
    }

    public static class InputMap
    {
        public static string MoveLeft => "move_left";
        public static string MoveRight => "move_right";
        public static string MoveForward => "move_forward";
        public static string MoveBackward => "move_backward";
        public static string Jump => "jump";
        public static string Crouch => "crouch";
        public static string Sprint => "sprint";
        public static string PrimaryInput => "primary_input";
        public static string SecondaryInput => "secondary_input";
        public static string HotbarNext => "hotbar_next";
        public static string HotbarPrev => "hotbar_prev";
        public static string OpenInventory => "open_inventory";

        public static class UI
        {
	        public static string Cancel => "ui_cancel";
	        public static string PrimaryInput => "ui_primary_input";
	        public static string SecondaryInput => "ui_secondary_input";
        }
    }
}
