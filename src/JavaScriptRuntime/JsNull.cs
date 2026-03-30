namespace JavaScriptRuntime
{
    /// <summary>
    /// Represents the JavaScript 'null' value distinctly from CLR null (used for JS 'undefined').
    /// This will be boxed when stored as an object.
    /// </summary>
    public enum JsNull
    {
        Null = 0
    }
}
