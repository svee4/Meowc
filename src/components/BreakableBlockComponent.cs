using Godot;
using Meowc.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowc.Components;

[GlobalClass]
public sealed partial class BreakableBlockComponent : ComponentBase, IComponent
{
    public static string ComponentName => nameof(BreakableBlockComponent);

    public void Break()
    {
        var root = Helpers.GetSceneRoot(this);
        root.QueueFree();
    }
}
