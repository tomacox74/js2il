using System;

namespace Js2IL.Services.ILGenerators
{
    record ExpressionResult
    {
        public JavascriptType JsType { get; init; }
        public Type? ClrType { get; init; }
        public bool IsBoxed { get; init; }
    }
}
