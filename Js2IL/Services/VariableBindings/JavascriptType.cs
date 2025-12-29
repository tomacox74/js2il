using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services
{
    public enum JavascriptType
    {
        /**
         * Represents an unknown type, typically used for variables that whose type cannot be determined at compile time.  
         * These times need runtime checks to determine their actual type. This is detrimental to performance, so it should be avoided when possible.
         */
        Unknown,
        /**
         * A string, object or array.  Does not need to be unboxed
         */
        Object,
        /**
         * A number, which is a double in C# and JavaScript.
         */
        Number,
        /**
         * true or false
         */
        Boolean,
        /**
         * Distincly different than undefined in JavaScript.
         */
        Null,

        Function,

        String
    }
}
