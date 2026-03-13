using System.Collections.Generic;
using Acornima.Ast;

namespace Js2IL.Utilities;

internal static class JavaScriptParameterNameExtractor
{
    public static IEnumerable<string> ExtractParameterNames(IReadOnlyList<Node> parameters)
    {
        if (parameters == null)
        {
            yield break;
        }

        int index = 0;
        foreach (var param in parameters)
        {
            if (param is Identifier id)
            {
                yield return id.Name;
            }
            else if (param is AssignmentPattern ap && ap.Left is Identifier apId)
            {
                yield return apId.Name;
            }
            else if (param is ObjectPattern or ArrayPattern)
            {
                yield return "param" + (index + 1);
            }

            index++;
        }
    }
}
