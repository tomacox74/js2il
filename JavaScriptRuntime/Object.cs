using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaScriptRuntime
{
    public class Object
    {
        public static object GetItem(object obj, double index)
        {
            var intIndex = Convert.ToInt32(index);

            if (obj is Array array)
            {
                return array[intIndex];
            }
            else
            {
                // todo: add generic object index access support
                throw new Exception("Object does not support index access. Only arrays are supported for index access.");
            }
        }

        public static void Test1()
        {
            System.Console.WriteLine("Object Test1 called");
        }

        public static void Test2()
        {
            object test1 = new Action(Object.Test1);

            ((Action)test1)();
        }
    }
}
