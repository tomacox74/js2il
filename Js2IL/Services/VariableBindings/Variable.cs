using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services
{
    internal record Variable
    {
        public required string Name;

        public int? LocalIndex = null;
    }


    internal class Variables : Dictionary<string, Variable>
    {
        public Variable GetOrCreate(string name)
        {
            if (!this.TryGetValue(name, out var variable))
            {
                variable = new Variable { Name = name };
                this[name] = variable;
            }
            return variable;
        }
        public void SetLocalIndex(string name, int localIndex)
        {
            if (this.TryGetValue(name, out var variable))
            {
                variable.LocalIndex = localIndex;
            }
        }
    }
}
