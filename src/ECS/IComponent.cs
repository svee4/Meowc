using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowc.ECS;

public interface IComponent
{
    static abstract string ComponentName { get; }
}
