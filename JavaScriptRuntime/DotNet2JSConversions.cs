using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace JavaScriptRuntime
{
    public class DotNet2JSConversions
    {
        public static string ToString(object? value)
        {
            if (value == null)
            {
                return "null";
            }
            if (value is string strValue)
            {
                return strValue;
            }
            if (value is bool boolValue)
            {
                return boolValue ? "true" : "false";
            }

            if (value is ExpandoObject expandObject)
            {
                string propertyValues = string.Join(", ", expandObject
                    .Select(kvp =>
                    {
                        var value = kvp.Value is string ? $"'{kvp.Value}'" : ToString(kvp.Value);

                        return $"{kvp.Key}: {value}";
                     }));

                return string.Format("{{ {0} }}", propertyValues);
            }

            // Add more conversions as needed
            return value!.ToString()!;
        }
    }
}
