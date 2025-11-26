using System;

// Mimic JavaScriptRuntime.Console
namespace JavaScriptRuntime
{
    public static class Console
    {
        public static object Log(object[] args)
        {
            foreach (var arg in args)
            {
                System.Console.WriteLine(DotNet2JSConversions.ToString(arg));
            }
            return null;
        }
    }
    
    public static class DotNet2JSConversions
    {
        public static string ToString(object? value)
        {
            if (value == null)
            {
                return "undefined";
            }
            if (value is double dd)
            {
                return dd.ToString();
            }
            return value.ToString() ?? "null";
        }
    }
}

namespace Functions
{
    public static class TestClass
    {
        public static object test(object[] scopes, object param1)
        {
            // This should match our generated IL exactly
            if (param1 is null)
            {
                param1 = 5.0;
            }
            
            // Create array and call console.log exactly like our generated code
            object[] array = new object[1];
            array[0] = param1;
            JavaScriptRuntime.Console.Log(array);
            
            return null;
        }
    }
}

public class Program
{
    public static void Main()
    {
        System.Console.WriteLine("Calling test():");
        Functions.TestClass.test(new object[0], null);
        
        System.Console.WriteLine("\nCalling test(10):");
        Functions.TestClass.test(new object[0], 10);
    }
}
