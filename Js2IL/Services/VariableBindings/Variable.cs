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
        private int _nextLocalIndex = 0;

        public Variable CreateLocal(string name)
        {
            var variable = new Variable { Name = name };
            variable.LocalIndex = _nextLocalIndex++;
            if (this.ContainsKey(name))
            {
                throw new InvalidOperationException($"Variable '{name}' already exists.");
            }
            this[name] = variable;
            return variable;
        }

        public Variable Get(string name)
        {
            if (this.TryGetValue(name, out var variable))
            {
                return variable;
            }
            throw new KeyNotFoundException($"Variable '{name}' not found.");
        }

        public int GetNumberOfLocals()
        {
            return _nextLocalIndex;
        }
    }
}
