using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Runtime
{
    public class Console
    {
        public static void Log(string message, object arg1)
        {
            arg1 = DotNet2JSConversions.ToString(arg1);
            System.Console.WriteLine(message,arg1);
        }
    }
}
