using System;
using Acornima;

var js = "function test(a = 10, b, c = a * 2) {}";
var ast = JavaScriptParser.ParseScript(js);
var func = ast.Body[0] as Acornima.Ast.FunctionDeclaration;
foreach (var p in func.Params) {
    Console.WriteLine($"Type: {p.GetType().Name}");
    if (p is Acornima.Ast.AssignmentPattern ap) {
        Console.WriteLine($"  Left: {ap.Left.GetType().Name}");
        Console.WriteLine($"  Right: {ap.Right.GetType().Name}");
    }
}
