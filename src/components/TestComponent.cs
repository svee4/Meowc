using Godot;
using Meowc.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowc.Components;

[GlobalClass]
public partial class TestComponent : ComponentBase, IComponent
{
    public static string ComponentName => nameof(TestComponent);

    public override void _EnterTree()
    {
        GD.Print("TestComponent EnterTree");
    }
}
