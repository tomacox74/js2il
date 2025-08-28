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

        public static object? GetProperty(object obj, string name)
        {
            // ExpandoObject properties
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                if (dict.TryGetValue(name, out var value))
                {
                    return value;
                }
                return null; // closest to JS undefined for now
            }

            // JavaScriptRuntime.Array: expose known properties via dot (length handled elsewhere)
            if (obj is Array)
            {
                // No custom properties yet; return null as missing
                return null;
            }

            // Fallback: no dynamic properties supported
            return null;
        }
    }
}
