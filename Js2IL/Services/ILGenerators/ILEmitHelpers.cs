using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics.CodeAnalysis;
using Js2IL.Services.VariableBindings;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Convenience helpers for emitting common IL instruction patterns using System.Reflection.Metadata encoders.
    /// Keep these generic and dependency-free so they can be reused across generators.
    /// </summary>
    internal static class ILEmitHelpers
    {
        // Load a managed string literal
        public static void Ldstr(this InstructionEncoder il, MetadataBuilder md, string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            il.LoadString(md.GetOrAddUserString(value));
        }

        // throw new <Error>(message)
        public static void EmitThrowError(this InstructionEncoder il, MetadataBuilder md, EntityHandle errorCtor, string message)
        {
            il.Ldstr(md, message);
            il.OpCode(ILOpCode.Newobj);
            il.Token(errorCtor);
            il.OpCode(ILOpCode.Throw);
        }

        /// <summary>
        /// Emit: new T[length] given the element type handle, and fill elements using the provided callback.
        /// The callback must leave the element value on the stack for each index.
        /// For length=0, creates an empty array via newarr (caller should use Array.Empty&lt;T&gt;() for better performance if available).
        /// </summary>
        public static void EmitNewArray(this InstructionEncoder il, int length, EntityHandle elementType, Action<InstructionEncoder,int> emitElementAt)
        {
            if (emitElementAt is null) throw new ArgumentNullException(nameof(emitElementAt));
            il.LoadConstantI4(length);
            il.OpCode(ILOpCode.Newarr);
            il.Token(elementType);

            for (int i = 0; i < length; i++)
            {
                il.OpCode(ILOpCode.Dup);
                il.LoadConstantI4(i);
                emitElementAt(il, i);
                il.OpCode(ILOpCode.Stelem_ref);
            }
        }

        /// <summary>
        /// Emit: new object[length] and fill elements using the provided callback. The callback must leave
        /// the element value on the stack for each index.
        /// </summary>
        public static void EmitNewObjectArray(this InstructionEncoder il, int length, EntityHandle objectType, Action<InstructionEncoder,int> emitElementAt)
        {
            il.EmitNewArray(length, objectType, emitElementAt);
        }

        /// <summary>
        /// Emit: new object[length] and fill elements using the provided callback. For length=0, uses Array.Empty&lt;object&gt;().
        /// The callback must leave the element value on the stack for each index.
        /// </summary>
        public static void EmitNewObjectArray(this InstructionEncoder il, int length, EntityHandle objectType, Utilities.Ecma335.MemberReferenceRegistry memberRefRegistry, Action<InstructionEncoder,int>? emitElementAt)
        {
            if (length == 0)
            {
                var arrayEmptyRef = memberRefRegistry.GetOrAddArrayEmptyObject();
                il.Call(arrayEmptyRef);
            }
            else
            {
                if (emitElementAt is null) throw new ArgumentNullException(nameof(emitElementAt));
                il.EmitNewArray(length, objectType, emitElementAt);
            }
        }

        /// <summary>
        /// Throws a NotSupportedException enriched with source file and start position if an AST node is supplied.
        /// </summary>
        /// <param name="message">Human-friendly message describing the unsupported feature.</param>
        /// <param name="node">Optional AST node for location context.</param>
    [DoesNotReturn]
    public static void ThrowNotSupported(string message, Acornima.Ast.Node? node = null)
        {
            if (node == null)
            {
                throw new NotSupportedException(message);
            }
            var loc = node.Location.Start; // Start is sufficient
            var src = string.IsNullOrEmpty(node.Location.SourceFile) ? "<unknown>" : node.Location.SourceFile;
            // New format: file:line:column: message
            throw new NotSupportedException($"{src}:{loc.Line}:{loc.Column}: {message}");
        }

        /// <summary>
        /// Returns (does not throw) a NotSupportedException enriched with location so it can be used in expressions like '?? throw'.
        /// </summary>
        public static NotSupportedException NotSupported(string message, Acornima.Ast.Node? node = null)
        {
            if (node == null)
            {
                return new NotSupportedException(message);
            }
            var loc = node.Location.Start;
            var src = string.IsNullOrEmpty(node.Location.SourceFile) ? "<unknown>" : node.Location.SourceFile;
            return new NotSupportedException($"{src}:{loc.Line}:{loc.Column}: {message}");
        }

        /// <summary>
        /// Emits IL to load a variable value onto the stack.
        /// Handles parameters, local variables (uncaptured), and fields (captured).
        /// </summary>
        /// <param name="il">The instruction encoder.</param>
        /// <param name="variable">The variable to load.</param>
        /// <param name="variables">Variables context for scope resolution.</param>
        /// <param name="bclReferences">BCL type references for unboxing.</param>
        /// <param name="unbox">Whether to unbox Number/Boolean types.</param>
        /// <param name="inClassMethod">Whether currently in a class instance method.</param>
        /// <param name="currentClassName">Current class name (for this._scopes access).</param>
        /// <param name="classRegistry">Class registry for _scopes field lookup.</param>
        public static void EmitLoadVariable(
            this InstructionEncoder il,
            Variable variable,
            Variables variables,
            BaseClassLibraryReferences bclReferences,
            bool unbox = false,
            bool inClassMethod = false,
            string? currentClassName = null,
            ClassRegistry? classRegistry = null)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            if (variables == null) throw new ArgumentNullException(nameof(variables));
            if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));

            // Helper for optional unboxing if requested
            void UnboxIfNeeded(JavascriptType jsType)
            {
                if (!unbox) return;

                // Stable primitive variables are emitted as unboxed primitives in their storage locations.
                // - For typed locals (double), ldloc already yields float64.
                // - For scope fields (captured vars), ldfld yields the field's primitive type (e.g., float64/bool).
                // Only parameters are always boxed (object) and require unboxing.
                if (!variable.IsParameter && variable.IsStableType)
                {
                    if (jsType == JavascriptType.Number)
                    {
                        // Typed local optimization: already float64 (LocalSlot >= 0 with double ClrType)
                        // Field-backed stable numbers are also stored as float64 (LocalSlot < 0)
                        if (variable.LocalSlot < 0 || variable.ClrType == typeof(double)) return;
                    }
                    else if (jsType == JavascriptType.Boolean)
                    {
                        // Field-backed stable booleans are stored as bool
                        if (variable.LocalSlot < 0) return;
                    }
                }
                
                // Unbox based on JavaScript type (Number or Boolean)
                if (jsType == JavascriptType.Number)
                {
                    il.OpCode(ILOpCode.Unbox_any);
                    il.Token(bclReferences.DoubleType);
                }
                else if (jsType == JavascriptType.Boolean)
                {
                    il.OpCode(ILOpCode.Unbox_any);
                    il.Token(bclReferences.BooleanType);
                }
            }

            // code assumes loaded value is boxed
            void BoxIfNeeded()
            {
                if (unbox) return;

                // unstable types are already boxed
                // basically a unstable type is a variable that could be assigned different types
                // value types are boxed boxed to allow this
                // so we only need to box when the variable is not already stored as a boxed value
                if (variable.IsStableType && variable.ClrType == typeof(double))
                {
                    il.OpCode(ILOpCode.Box);
                    il.Token(bclReferences.DoubleType);
                }
            }

            // Check if this is an uncaptured variable (uses local variable)
            if (variable.LocalSlot >= 0)
            {
                // Load from local variable slot
                il.LoadLocal(variable.LocalSlot);

                // unbox and box are mutuallly exclusive
                UnboxIfNeeded(variable.Type);
                BoxIfNeeded();

                return;
            }

            // Get registry for scope type lookups
            var registry = variables.GetVariableRegistry();

            // Parent scope variable (captured from ancestor scope)
            if (variable is ScopeVariable sv)
            {
                // In class instance methods, scopes are stored in this._scopes field, not in arg_0
                if (inClassMethod && !string.IsNullOrEmpty(currentClassName))
                {
                    // Load this._scopes field
                    il.OpCode(ILOpCode.Ldarg_0); // load 'this'
                    if (classRegistry != null && classRegistry.TryGetPrivateField(currentClassName, "_scopes", out var scopesField))
                    {
                        il.OpCode(ILOpCode.Ldfld);
                        il.Token(scopesField);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Class '{currentClassName}' missing _scopes field");
                    }
                }
                else
                {
                    // Regular function: arg_0 is the scopes array
                    il.LoadArgument(0); // scopes array
                }
                il.LoadConstantI4(sv.ParentScopeIndex);
                il.OpCode(ILOpCode.Ldelem_ref);
                // Cast to the concrete scope type for verifiable field access
                var tdef = registry?.GetScopeTypeHandle(variable.ScopeName) ?? default;
                if (!tdef.IsNil)
                {
                    il.OpCode(ILOpCode.Castclass);
                    il.Token(tdef);
                }
                il.OpCode(ILOpCode.Ldfld);
                il.Token(sv.FieldHandle);
                UnboxIfNeeded(sv.Type);
                return;
            }

            // Parameter variable
            if (variable.IsParameter)
            {
                // Directly load argument (already object). ParameterIndex already accounts for scopes[] at arg0
                il.LoadArgument(variable.ParameterIndex);
                UnboxIfNeeded(variable.Type);
                return;
            }

            // Scope field variable (current scope)
            var scopeLocalIndex = variables.GetScopeLocalSlot(variable.ScopeName);
            if (scopeLocalIndex.Address == -1)
            {
                throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
            }

            // Load scope instance
            if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
            {
                il.LoadArgument(scopeLocalIndex.Address);
            }
            else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
            {
                il.LoadArgument(0); // Load scope array parameter
                il.LoadConstantI4(scopeLocalIndex.Address); // Load array index
                il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
            }
            else
            {
                il.LoadLocal(scopeLocalIndex.Address);
            }

            // Cast to the concrete scope type for verifiable field access
            var tdefLocal = registry?.GetScopeTypeHandle(variable.ScopeName) ?? default;
            if (!tdefLocal.IsNil)
            {
                il.OpCode(ILOpCode.Castclass);
                il.Token(tdefLocal);
            }

            // Load field value
            il.OpCode(ILOpCode.Ldfld);
            il.Token(variable.FieldHandle);
            UnboxIfNeeded(variable.Type);
        }

        /// <summary>
        /// Emits IL to store a value into a variable.
        /// Expects the value to be on the stack.
        /// Handles local variables (uncaptured) and fields (captured).
        /// For field variables when scopeAlreadyLoaded=false, uses a temp local to reorder stack.
        /// </summary>
        /// <param name="il">The instruction encoder.</param>
        /// <param name="variable">The variable to store into.</param>
        /// <param name="variables">Variables context for scope resolution.</param>
        /// <param name="scopeAlreadyLoaded">Whether the scope instance is already on the stack below the value (for field stores).</param>
        public static void EmitStoreVariable(
            this InstructionEncoder il,
            Variable variable,
            Variables variables,
            bool scopeAlreadyLoaded = false,
            bool valueIsBoxed = true,
            BaseClassLibraryReferences? bclReferences = null)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            if (variables == null) throw new ArgumentNullException(nameof(variables));

            // Check if this is an uncaptured variable (uses local variable)
            if (variable.LocalSlot >= 0)
            {
                // Determine if we need to box the value before storing.
                // The local slot is typed as float64 only when IsStableType AND ClrType == double.
                // Otherwise, it's typed as object and requires boxing.
                bool localIsTypedAsDouble = variable.IsStableType && variable.ClrType == typeof(double);
                if (!valueIsBoxed && !localIsTypedAsDouble)
                {
                    // The value is an unboxed double that needs to be stored to an object-typed local
                    // We always need to box it, regardless of what the variable's inferred type is
                    if (bclReferences == null)
                    {
                        throw new ArgumentNullException(nameof(bclReferences), "BCL references are required for boxing operations.");
                    }

                    il.OpCode(ILOpCode.Box);
                    il.Token(bclReferences.DoubleType);
                }

                // Stack: [value]
                // Store to local variable slot
                il.StoreLocal(variable.LocalSlot);
                return;
            }

            // Cannot store to parameters directly (they're method arguments)
            if (variable.IsParameter)
            {
                throw new InvalidOperationException($"Cannot store to parameter '{variable.Name}' - parameters are read-only.");
            }

            // For field variables, we need [scope instance, value] on stack
            if (!scopeAlreadyLoaded)
            {
                // Stack: [value]
                // Need: [scope, value]
                // Strategy: stloc temp, load scope, ldloc temp, stfld
                var scopeLocalIndex = variables.GetScopeLocalSlot(variable.ScopeName);
                if (scopeLocalIndex.Address == -1)
                {
                    throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
                }

                // Allocate a temp local for the value - temp slots are typed as object, so we must box first
                var tempSlot = variables.AllocateBlockScopeLocal($"StoreTemp_{variable.Name}");
                
                // If the value is an unboxed primitive, box it before storing to the object-typed temp local
                if (!valueIsBoxed)
                {
                    if (bclReferences == null)
                    {
                        throw new ArgumentNullException(nameof(bclReferences), "BCL references required for boxing when storing to temp local.");
                    }
                    il.OpCode(ILOpCode.Box);
                    il.Token(bclReferences.DoubleType);
                    valueIsBoxed = true;  // Value is now boxed
                }
                
                il.StoreLocal(tempSlot);  // Stack: [] (value saved to temp)

                // Load scope instance
                if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
                {
                    il.LoadArgument(scopeLocalIndex.Address);
                }
                else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
                {
                    il.LoadArgument(0);
                    il.LoadConstantI4(scopeLocalIndex.Address);
                    il.OpCode(ILOpCode.Ldelem_ref);
                }
                else
                {
                    il.LoadLocal(scopeLocalIndex.Address);
                }

                // Cast to scope type if needed
                var registry = variables.GetVariableRegistry();
                var scopeType = registry?.GetScopeTypeHandle(variable.ScopeName) ?? default;
                if (!scopeType.IsNil)
                {
                    il.OpCode(ILOpCode.Castclass);
                    il.Token(scopeType);
                }

                // Load value from temp
                il.LoadLocal(tempSlot);  // Stack: [scope, value]
            }

            // If the incoming value is an unboxed primitive but this variable is not stored in a
            // typed value-type field, we must box before stfld.
            // Note: fields are only typed as value types for stable-typed variables.
            if (!valueIsBoxed)
            {
                bool fieldIsTypedAsDouble = variable.IsStableType && variable.ClrType == typeof(double);
                bool fieldIsTypedAsBool = variable.IsStableType && variable.ClrType == typeof(bool);

                if (!fieldIsTypedAsDouble && !fieldIsTypedAsBool)
                {
                    if (bclReferences == null)
                    {
                        throw new ArgumentNullException(nameof(bclReferences), "BCL references are required for boxing operations.");
                    }

                    if (variable.Type == JavascriptType.Number)
                    {
                        il.OpCode(ILOpCode.Box);
                        il.Token(bclReferences.DoubleType);
                    }
                    else if (variable.Type == JavascriptType.Boolean)
                    {
                        il.OpCode(ILOpCode.Box);
                        il.Token(bclReferences.BooleanType);
                    }
                    else if (variable.Type == JavascriptType.Unknown)
                    {
                        // Callers only pass valueIsBoxed=false when emitting a known unboxed primitive.
                        // For captured/update-expression paths the Variable.Type may not be updated yet.
                        // Default to boxing as double to preserve existing numeric semantics.
                        il.OpCode(ILOpCode.Box);
                        il.Token(bclReferences.DoubleType);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Attempted to store an unboxed value into variable '{variable.Name}' with non-primitive JS type '{variable.Type}'.");
                    }
                }
            }

            // Stack: [scope instance, value]
            // Store to field
            il.OpCode(ILOpCode.Stfld);
            il.Token(variable.FieldHandle);
        }
    }
}
