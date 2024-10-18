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

    [Export]
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public bool IsDamaged => MaxHealth != CurrentHealth;

    public BreakableBlockComponent() => 
        CurrentHealth = MaxHealth;

    public void Break()
    {
        var root = Helpers.GetSceneRoot(this);
        root.QueueFree();
    }

    public void Damage(int amount = 1)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            Break();
        }
    }

    public void Reset()
    {
        CurrentHealth = MaxHealth;
    }
}
