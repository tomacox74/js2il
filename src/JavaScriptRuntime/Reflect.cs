namespace JavaScriptRuntime
{
    [IntrinsicObject("Reflect")]
    public static class Reflect
    {
        public static bool defineProperty(object target, object? propertyKey, object? attributes)
        {
            Object.defineProperty(target, propertyKey, attributes);
            return true;
        }
    }
}
