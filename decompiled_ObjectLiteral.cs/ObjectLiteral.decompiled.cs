using System.Dynamic;
using System.Reflection;
using JavaScriptRuntime;
using Scopes;

[assembly: AssemblyVersion("1.0.0.0")]
namespace Scopes;

public class ObjectLiteral
{
	public object x;
}
public class Program
{
	public static void Main()
	{
		object obj = new ObjectLiteral();
		((ObjectLiteral)obj).x = new ExpandoObject
		{
			["name"] = "Alice",
			["age"] = 31.0
		};
		Console.Log("x is", ((ObjectLiteral)obj).x);
	}
}
