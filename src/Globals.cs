using Godot;
using System;

namespace Meowc;

public static class Globals
{
    public static float Gravity => 15;

    public static int FrameRateCap => 500;

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
    }
}
