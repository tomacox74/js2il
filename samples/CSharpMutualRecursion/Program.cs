using System;
using System.Runtime.CompilerServices;

namespace CSharpMutualRecursion;

internal static class Program
{
	private static void Main()
	{
		// Keep this small and deterministic; we only care about the IL/metadata shape.
		var result = Mutual.A(3);
		Console.WriteLine(result);
	}
}

internal static class Mutual
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static int A(int depth)
	{
		if (depth <= 0)
		{
			return 1;
		}

		return B(depth - 1) + 10;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static int B(int depth)
	{
		if (depth <= 0)
		{
			return 2;
		}

		return A(depth - 1) + 20;
	}
}
