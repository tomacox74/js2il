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
    [IntrinsicObject("Error", IntrinsicCallKind.BuiltInError)]
    public class Error : Exception
    {
        private readonly string _constructedStack;

        // PascalCase (JS has a 'name' string property on Error instances)
        public string Name { get; protected set; }

        // JS-style aliases
        public string name => Name;
        public string message => base.Message;
        public object? cause { get; protected set; }
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

        public Error(object? message) : this(CoerceMessage(message)) { }

        public Error(string? message) : base(message ?? string.Empty)
        {
            Name = "Error";
            _constructedStack = CaptureStack();
            PrototypeChain.SetPrototype(this, GlobalThis.ErrorPrototypeValue);
        }

        public Error(string? message, object? cause) : this(message)
        {
            this.cause = cause;
        }

        public Error(string? message, Exception? innerException) : base(message ?? string.Empty, innerException)
        {
            Name = "Error";
            _constructedStack = CaptureStack();
            PrototypeChain.SetPrototype(this, GlobalThis.ErrorPrototypeValue);
        }

        public Error(string? message, Exception? innerException, object? cause) : this(message, innerException)
        {
            this.cause = cause;
        }

        public static void InstallCause(object? errorValue, object? options)
        {
            if (errorValue is Error error
                && options is not null
                && options is not JsNull
                && !TypeUtilities.IsPrimitive(options)
                && ObjectRuntime.HasPropertyIn("cause", options))
            {
                error.cause = ObjectRuntime.GetProperty(options, "cause");
            }
        }

        protected virtual string CaptureStack()
        {
            // Capture current .NET stack trace as a placeholder for JS stack
            // In the future, this can be mapped to JS frame formats.
            return Environment.StackTrace ?? string.Empty;
        }

        protected void InitializeIntrinsicSurface(object prototype)
        {
            PrototypeChain.SetPrototype(this, prototype);
        }

        protected static string CoerceMessage(object? message)
            => message is null ? string.Empty : DotNet2JSConversions.ToErrorMessageString(message);

        public override string ToString()
            => string.IsNullOrEmpty(Message) ? Name : $"{Name}: {Message}";
    }

    [IntrinsicObject("EvalError", IntrinsicCallKind.BuiltInError)]
    public class EvalError : Error
    {
        public EvalError() : base() { Name = "EvalError"; PrototypeChain.SetPrototype(this, GlobalThis.EvalErrorPrototypeValue); }
        public EvalError(object? message) : base(message) { Name = "EvalError"; PrototypeChain.SetPrototype(this, GlobalThis.EvalErrorPrototypeValue); }
        public EvalError(string? message) : base(message) { Name = "EvalError"; PrototypeChain.SetPrototype(this, GlobalThis.EvalErrorPrototypeValue); }
        public EvalError(string? message, Exception? inner) : base(message, inner) { Name = "EvalError"; PrototypeChain.SetPrototype(this, GlobalThis.EvalErrorPrototypeValue); }
    }

    [IntrinsicObject("RangeError", IntrinsicCallKind.BuiltInError)]
    public class RangeError : Error
    {
        public RangeError() : base() { Name = "RangeError"; PrototypeChain.SetPrototype(this, GlobalThis.RangeErrorPrototypeValue); }
        public RangeError(object? message) : base(message) { Name = "RangeError"; PrototypeChain.SetPrototype(this, GlobalThis.RangeErrorPrototypeValue); }
        public RangeError(string? message) : base(message) { Name = "RangeError"; PrototypeChain.SetPrototype(this, GlobalThis.RangeErrorPrototypeValue); }
        public RangeError(string? message, Exception? inner) : base(message, inner) { Name = "RangeError"; PrototypeChain.SetPrototype(this, GlobalThis.RangeErrorPrototypeValue); }
    }

    [IntrinsicObject("ReferenceError", IntrinsicCallKind.BuiltInError)]
    public class ReferenceError : Error
    {
        public ReferenceError() : base() { Name = "ReferenceError"; PrototypeChain.SetPrototype(this, GlobalThis.ReferenceErrorPrototypeValue); }
        public ReferenceError(object? message) : base(message) { Name = "ReferenceError"; PrototypeChain.SetPrototype(this, GlobalThis.ReferenceErrorPrototypeValue); }
        public ReferenceError(string? message) : base(message) { Name = "ReferenceError"; PrototypeChain.SetPrototype(this, GlobalThis.ReferenceErrorPrototypeValue); }
        public ReferenceError(string? message, Exception? inner) : base(message, inner) { Name = "ReferenceError"; PrototypeChain.SetPrototype(this, GlobalThis.ReferenceErrorPrototypeValue); }
    }

    [IntrinsicObject("SyntaxError", IntrinsicCallKind.BuiltInError)]
    public class SyntaxError : Error
    {
        public SyntaxError() : base() { Name = "SyntaxError"; PrototypeChain.SetPrototype(this, GlobalThis.SyntaxErrorPrototypeValue); }
        public SyntaxError(object? message) : base(message) { Name = "SyntaxError"; PrototypeChain.SetPrototype(this, GlobalThis.SyntaxErrorPrototypeValue); }
        public SyntaxError(string? message) : base(message) { Name = "SyntaxError"; PrototypeChain.SetPrototype(this, GlobalThis.SyntaxErrorPrototypeValue); }
        public SyntaxError(string? message, Exception? inner) : base(message, inner) { Name = "SyntaxError"; PrototypeChain.SetPrototype(this, GlobalThis.SyntaxErrorPrototypeValue); }
    }

    [IntrinsicObject("TypeError", IntrinsicCallKind.BuiltInError)]
    public class TypeError : Error
    {
        public TypeError() : base() { Name = "TypeError"; PrototypeChain.SetPrototype(this, GlobalThis.TypeErrorPrototypeValue); }
        public TypeError(object? message) : base(message) { Name = "TypeError"; PrototypeChain.SetPrototype(this, GlobalThis.TypeErrorPrototypeValue); }
        public TypeError(string? message) : base(message) { Name = "TypeError"; PrototypeChain.SetPrototype(this, GlobalThis.TypeErrorPrototypeValue); }
        public TypeError(string? message, Exception? inner) : base(message, inner) { Name = "TypeError"; PrototypeChain.SetPrototype(this, GlobalThis.TypeErrorPrototypeValue); }
    }

    [IntrinsicObject("URIError", IntrinsicCallKind.BuiltInError)]
    public class URIError : Error
    {
        public URIError() : base() { Name = "URIError"; PrototypeChain.SetPrototype(this, GlobalThis.URIErrorPrototypeValue); }
        public URIError(object? message) : base(message) { Name = "URIError"; PrototypeChain.SetPrototype(this, GlobalThis.URIErrorPrototypeValue); }
        public URIError(string? message) : base(message) { Name = "URIError"; PrototypeChain.SetPrototype(this, GlobalThis.URIErrorPrototypeValue); }
        public URIError(string? message, Exception? inner) : base(message, inner) { Name = "URIError"; PrototypeChain.SetPrototype(this, GlobalThis.URIErrorPrototypeValue); }
    }

    [IntrinsicObject("AggregateError", IntrinsicCallKind.BuiltInError)]
    public class AggregateError : Error
    {
        public JavaScriptRuntime.Array Errors { get; }
        public JavaScriptRuntime.Array errors => Errors; // JS-style alias

        public AggregateError() : this(System.Array.Empty<object?>(), null) { }
        public AggregateError(object? message) : this(System.Array.Empty<object?>(), CoerceMessage(message)) { }
        public AggregateError(string? message) : this(System.Array.Empty<object?>(), message) { }
        public AggregateError(System.Collections.IEnumerable errors) : this(errors, null) { }
        public AggregateError(System.Collections.IEnumerable errors, string? message) : base(message)
        {
            Name = "AggregateError";
            Errors = new Array(errors);
            InitializeIntrinsicSurface(GlobalThis.AggregateErrorPrototypeValue);
            InstallErrorsProperty();
        }

        public AggregateError(System.Collections.IEnumerable errors, string? message, Exception? inner) : base(message, inner)
        {
            Name = "AggregateError";
            Errors = new Array(errors);
            InitializeIntrinsicSurface(GlobalThis.AggregateErrorPrototypeValue);
            InstallErrorsProperty();
        }

        public static AggregateError Construct(object?[] args)
        {
            var hasMessage = args.Length > 1 && args[1] is not null;
            var message = hasMessage ? CoerceMessage(args[1]) : null;
            var iterable = args.Length > 0 ? args[0] : null;
            var iterator = ObjectRuntime.GetIterator(iterable);
            var values = new List<object?>();

            while (true)
            {
                var next = iterator.Next();
                if (next.done)
                {
                    break;
                }

                values.Add(next.value);
            }

            var error = new AggregateError(values, message);
            if (hasMessage)
            {
                error.InstallMessageProperty(message!);
            }
            else
            {
                PropertyDescriptorStore.Delete(error, "message");
            }

            return error;
        }

        public override string ToString()
        {
            var baseStr = base.ToString();
            if (Errors.length == 0) return baseStr;
            return baseStr + $" (errors: {Errors.length})";
        }

        private void InstallErrorsProperty()
        {
            PropertyDescriptorStore.DefineOrUpdate(this, "errors", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = Errors
            });
        }

        private void InstallMessageProperty(string message)
        {
            PropertyDescriptorStore.DefineOrUpdate(this, "message", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = message
            });
        }
    }
}
