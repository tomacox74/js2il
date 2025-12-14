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

            // Helper for optional unboxing if request
            void UnboxIfNeeded(JavascriptType jsType)
            {
                if (!unbox) return;

                // if the variable we are loading is a double already, then no unboxing is needed
                if (variable.ClrType == typeof(double)) return;
                
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
                if (variable.ClrType == typeof(double))
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
                // determine if we need to box the value before storing
                if (!valueIsBoxed && variable.ClrType != typeof(double))
                {
                    if (variable.Type == JavascriptType.Number)
                    {
                        if (bclReferences == null)
                        {
                            throw new ArgumentNullException(nameof(bclReferences), "BCL references are required for boxing operations.");
                        }

                        il.OpCode(ILOpCode.Box);
                        il.Token(bclReferences.DoubleType);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot store unboxed value into variable '{variable.Name}' of type '{variable.ClrType?.FullName}'");
                    }
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

                // Allocate a temp local for the value
                var tempSlot = variables.AllocateBlockScopeLocal($"StoreTemp_{variable.Name}");
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

            // Stack: [scope instance, value]
            // Store to field
            il.OpCode(ILOpCode.Stfld);
            il.Token(variable.FieldHandle);
        }
    }
}
