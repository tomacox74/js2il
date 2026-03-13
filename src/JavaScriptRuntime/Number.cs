namespace JavaScriptRuntime
{
    /// <summary>
    /// Minimal Number intrinsic surface needed by tests/runtime.
    /// </summary>
    [IntrinsicObject("Number")]
    public static class Number
    {
        /// <summary>
        /// ECMAScript: Number.isNaN(x) returns true only when x is a Number and is NaN (no coercion).
        /// </summary>
        public static bool isNaN(object? value)
        {
            return value switch
            {
                double d => double.IsNaN(d),
                float f => float.IsNaN(f),
                _ => false
            };
        }
    }
}
