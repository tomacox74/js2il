using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Minimal JavaScript-like Error object for the runtime.
    /// Inherits from System.Exception so it can be thrown/caught with .NET mechanics
    /// while exposing JS-style properties (name, message, stack).
    /// </summary>
    public class Error : Exception
    {
        private readonly string _constructedStack;

        // PascalCase (JS has a 'name' string property on Error instances)
        public string Name { get; protected set; }

        // JS-style aliases
        public string name => Name;
        public string message => base.Message;
        public string stack
        {
            get
            {
                // If not thrown yet, StackTrace may be null; fall back to construction stack.
                return base.StackTrace ?? _constructedStack;
            }
        }

        // Convenience .NET-style alias (optional)
        public string Stack => stack;

        public Error() : this(string.Empty) { }

        public Error(string? message) : base(message ?? string.Empty)
        {
            Name = "Error";
            _constructedStack = CaptureStack();
        }

        public Error(string? message, Exception? innerException) : base(message ?? string.Empty, innerException)
        {
            Name = "Error";
            _constructedStack = CaptureStack();
        }

        protected virtual string CaptureStack()
        {
            // Capture current .NET stack trace as a placeholder for JS stack
            // In the future, this can be mapped to JS frame formats.
            return Environment.StackTrace ?? string.Empty;
        }

        public override string ToString()
            => string.IsNullOrEmpty(Message) ? Name : $"{Name}: {Message}";
    }

    public class EvalError : Error
    {
        public EvalError() : base() { Name = "EvalError"; }
        public EvalError(string? message) : base(message) { Name = "EvalError"; }
        public EvalError(string? message, Exception? inner) : base(message, inner) { Name = "EvalError"; }
    }

    public class RangeError : Error
    {
        public RangeError() : base() { Name = "RangeError"; }
        public RangeError(string? message) : base(message) { Name = "RangeError"; }
        public RangeError(string? message, Exception? inner) : base(message, inner) { Name = "RangeError"; }
    }

    public class ReferenceError : Error
    {
        public ReferenceError() : base() { Name = "ReferenceError"; }
        public ReferenceError(string? message) : base(message) { Name = "ReferenceError"; }
        public ReferenceError(string? message, Exception? inner) : base(message, inner) { Name = "ReferenceError"; }
    }

    public class SyntaxError : Error
    {
        public SyntaxError() : base() { Name = "SyntaxError"; }
        public SyntaxError(string? message) : base(message) { Name = "SyntaxError"; }
        public SyntaxError(string? message, Exception? inner) : base(message, inner) { Name = "SyntaxError"; }
    }

    public class TypeError : Error
    {
        public TypeError() : base() { Name = "TypeError"; }
        public TypeError(string? message) : base(message) { Name = "TypeError"; }
        public TypeError(string? message, Exception? inner) : base(message, inner) { Name = "TypeError"; }
    }

    public class AggregateError : Error
    {
        // In JS AggregateError has an iterable of errors. Represent as object[] here.
        public JavaScriptRuntime.Array Errors { get; }
        public JavaScriptRuntime.Array errors => Errors; // JS-style alias

        public AggregateError() : this(System.Array.Empty<object?>(), null) { }
        public AggregateError(System.Collections.IEnumerable errors) : this(errors, null) { }
        public AggregateError(System.Collections.IEnumerable errors, string? message) : base(message)
        {
            Name = "AggregateError";
            Errors = new Array(errors);
        }

        public AggregateError(System.Collections.IEnumerable errors, string? message, Exception? inner) : base(message, inner)
        {
            Name = "AggregateError";
            Errors = new Array(errors);
        }

        public override string ToString()
        {
            var baseStr = base.ToString();
            if (Errors.length == 0) return baseStr;
            return baseStr + $" (errors: {Errors.length})";
        }
    }
}
