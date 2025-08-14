using Acornima;
using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates Intermediate Language (IL) code from a JavaScript Abstract Syntax Tree (AST) for a method
    /// </summary>
    internal class ILMethodGenerator : IMethodExpressionEmitter
    {
        private readonly Variables _variables;
        private readonly BaseClassLibraryReferences _bclReferences;
        private readonly MetadataBuilder _metadataBuilder;
        private readonly InstructionEncoder _il;
        private readonly BinaryOperators _binaryOperators;
        private readonly IMethodExpressionEmitter _expressionEmitter;
        private readonly Runtime _runtime;
        private readonly MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private MethodDefinitionHandle _firstMethod = default;

        private readonly Dispatch.DispatchTableGenerator _dispatchTableGenerator;
    // Tracks the name of the variable currently being initialized, to name arrow-function scopes consistently
    // with SymbolTableBuilder (e.g., ArrowFunction_<targetName>) when emitting an ArrowFunctionExpression on the RHS.
    private string? _currentAssignmentTarget;

        /*
         * Temporary exposure of private members until refactoring gets cleaner
         * need to determine what the difference is between generating the main method and generating any generic method
         */
        public Variables Variables => _variables;
        public BaseClassLibraryReferences BclReferences => _bclReferences;
        public MetadataBuilder MetadataBuilder => _metadataBuilder;
        public InstructionEncoder IL => _il;

        public MethodDefinitionHandle FirstMethod => _firstMethod;

        public ILMethodGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            var methodIl = new BlobBuilder();
            _il = new InstructionEncoder(methodIl, new ControlFlowBuilder());
            this._runtime = new Runtime(metadataBuilder, _il);
            _binaryOperators = new BinaryOperators(metadataBuilder, _il, variables, bclReferences, _runtime);

            // temporary as we set the table for further refactoring
            this._expressionEmitter = this;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _dispatchTableGenerator = dispatchTableGenerator ?? throw new ArgumentNullException(nameof(dispatchTableGenerator));
        }

        public void DeclareVariable(VariableDeclaration variableDeclaraion)
        {
            // TODO need to handle multiple
            var variableAST = variableDeclaraion.Declarations.FirstOrDefault()!;
            var variableName = (variableAST.Id as Acornima.Ast.Identifier)!.Name;

            // resolve the variable via Variables
            var variable = _variables.FindVariable(variableName) ?? throw new InvalidOperationException($"Variable '{variableName}' not found.");

            // If this is a lexical (block) scoped variable (let/const) inside a block shadowing an outer declaration,
            // the variable.ScopeName will be the block scope (Block_LxCy). We must load that scope instance local.
            bool isBlockScope = variable.ScopeName.StartsWith("Block_L", StringComparison.Ordinal);

            // now we need to generate the expession portion
            if (variableAST.Init != null)
            {
                // New approach: Store to scope field
                var scopeLocalIndex = _variables.GetScopeLocalSlot(variable.ScopeName);
                if (scopeLocalIndex.Address == -1)
                {
                    throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
                }
                
                // Load scope instance first for stfld
                if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
                {
                    _il.LoadArgument(scopeLocalIndex.Address);
                }
                else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
                {
                    _il.LoadArgument(0); // Load scope array parameter
                    _il.LoadConstantI4(scopeLocalIndex.Address); // Load array index
                    _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
                }
                else
                {
                    _il.LoadLocal(scopeLocalIndex.Address);
                }
                
                // Generate the expression - this puts the value on the stack
                var prevAssignmentTarget = _currentAssignmentTarget;
                _currentAssignmentTarget = variableName;
                try
                {
                    variable.Type = this._expressionEmitter.Emit(variableAST.Init, new TypeCoercion());
                }
                finally
                {
                    _currentAssignmentTarget = prevAssignmentTarget;
                }
                if (variable.Type == JavascriptType.Number)
                {
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.DoubleType);
                }
                
                // Now stack: [scope_instance] [value] - perfect for stfld
                _il.OpCode(ILOpCode.Stfld);
                _il.Token(variable.FieldHandle);
            }
        }

        public void GenerateStatements(NodeList<Statement> statements)
        {
            // Iterate through each statement in the block
            foreach (var statement in statements.Where(s => s is not FunctionDeclaration))
            {
                GenerateStatement(statement);
            }
        }

        public void InitializeLocalFunctionVariables(IEnumerable<FunctionDeclaration> functionDeclarations)
        {
            // Iterate through each function declaration in the block
            foreach (var functionDeclaration in functionDeclarations)
            {
                InitializeLocalFunctionVariable(functionDeclaration);
            }
        }

        public void InitializeLocalFunctionVariable(FunctionDeclaration functionDeclaration)
        {
            var functionName = (functionDeclaration.Id as Acornima.Ast.Identifier)!.Name;
            var functionVariable = _variables.FindVariable(functionName) ?? throw new InvalidOperationException($"Variable '{functionName}' not found.");

            var dispatchDelegateField = _dispatchTableGenerator.GetFieldDefinitionHandle(functionName);

            // now we assign a local variable to the function delegate
            // in the general case the local feels wasteful but there is scenarons where it could be assigned a different value
            
            // Store using scope field
            var scopeLocalIndex = _variables.GetScopeLocalSlot(functionVariable.ScopeName);
            if (scopeLocalIndex.Address == -1)
            {
                throw new InvalidOperationException($"Scope '{functionVariable.ScopeName}' not found in local slots");
            }
            
            // Load scope instance, then load delegate value, then store to field
            if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
            {
                _il.LoadArgument(scopeLocalIndex.Address);
            }
            else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
            {
                _il.LoadArgument(0); // Load scope array parameter
                _il.LoadConstantI4(scopeLocalIndex.Address); // Load array index
                _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
            }
            else
            {
                _il.LoadLocal(scopeLocalIndex.Address);
            }
            _il.OpCode(ILOpCode.Ldsfld);
            _il.Token(dispatchDelegateField);
            _il.OpCode(ILOpCode.Stfld);
            _il.Token(functionVariable.FieldHandle);
        }


        public void GenerateStatement(Statement statement)
        {
            switch (statement)
            {
                case VariableDeclaration variableDeclaration:
                    DeclareVariable(variableDeclaration);
                    break;
                case ExpressionStatement expressionStatement:
                    GenerateExpressionStatement(expressionStatement);
                    break;
                case ForStatement forStatement:
                    GenerateForStatement(forStatement);
                    break;
                case IfStatement ifStatement:
                    GenerateIfStatement(ifStatement);
                    break;
                case BlockStatement blockStatement:
                    // Handle BlockStatement with its own lexical scope if it declared let/const
                    GenerateBlock(blockStatement);
                    break;
                case ReturnStatement returnStatement:
                    GenerateReturnStatement(returnStatement);
                    break;
                case EmptyStatement:
                    // Empty statements (like standalone semicolons) do nothing
                    break;
                default:
                    throw new NotSupportedException($"Unsupported statement type: {statement.Type}");
            }
        }

        /// <summary>
        /// Generates code for a BlockStatement, creating a new scope object if the block declares any let/const bindings.
        /// This enables correct shadowing behavior for block scoped variables.
        /// </summary>
        private void GenerateBlock(BlockStatement blockStatement)
        {
            // Heuristic: if the symbol table created a block scope we would have a distinct Scope in the registry.
            // Since we don't have direct Scope reference here, we re-scan the statements for VariableDeclarations
            // containing 'let' or 'const'. The parser represented them already; BindingKind stored in registry determines field attributes.
            bool hasLexical = blockStatement.Body.Any(s =>
                s is VariableDeclaration vd && vd.Kind is VariableDeclarationKind.Let or VariableDeclarationKind.Const);

            // If no lexical declarations, just emit statements directly.
            if (!hasLexical)
            {
                GenerateStatements(blockStatement.Body);
                return;
            }

            // Create a synthetic scope name matching SymbolTableBuilder convention so registry lookups succeed.
            // We rely on the same naming pattern used during symbol table build.
            var scopeName = $"Block_L{blockStatement.Location.Start.Line}C{blockStatement.Location.Start.Column}";

            var registry = _variables.GetVariableRegistry();
            int? blockLocalIndex = null;
            if (registry != null)
            {
                var scopeTypeHandle = registry.GetScopeTypeHandle(scopeName);
                if (!scopeTypeHandle.IsNil)
                {
                    // Create constructor reference
                    var ctorSigBuilder = new BlobBuilder();
                    new BlobEncoder(ctorSigBuilder)
                        .MethodSignature(isInstanceMethod: true)
                        .Parameters(0, rt => rt.Void(), p => { });
                    var ctorRef = _metadataBuilder.AddMemberReference(
                        scopeTypeHandle,
                        _metadataBuilder.GetOrAddString(".ctor"),
                        _metadataBuilder.GetOrAddBlob(ctorSigBuilder));

                    // newobj + store to a temp local (extend locals if necessary)
                    // Allocate a new logical local slot index at end: existing locals count + created ones
                    // Allocate a new local slot dedicated to this block scope
                    blockLocalIndex = _variables.AllocateBlockScopeLocal(scopeName);
                    _il.OpCode(ILOpCode.Newobj);
                    _il.Token(ctorRef);
                    _il.StoreLocal(blockLocalIndex.Value);
                    // Track lexical scope so variable resolution prefers it
                    _variables.PushLexicalScope(scopeName);
                }
            }

            // Emit inner statements (variables inside will resolve to the block scope fields via registry name match)
            GenerateStatements(blockStatement.Body);

            if (blockLocalIndex.HasValue)
            {
                _variables.PopLexicalScope(scopeName);
            }
        }

        private void GenerateReturnStatement(ReturnStatement returnStatement)
        {
        if (returnStatement.Argument != null)
            {
                // Special-case: returning a function identifier -> bind closure scopes
                if (returnStatement.Argument is Identifier fid)
                {
            // Only treat as function if it corresponds to a known function declaration
            var funcDecl = _dispatchTableGenerator.GetFunctionDeclaration(fid.Name);
            var fnVar = funcDecl != null ? _variables.FindVariable(fid.Name) : null;
            if (fnVar != null && funcDecl != null)
                    {
                        // Load the function delegate from its scope field
                        var scopeSlot = _variables.GetScopeLocalSlot(fnVar.ScopeName);
                        if (scopeSlot.Address == -1)
                            throw new InvalidOperationException($"Scope '{fnVar.ScopeName}' not found in local slots");
                        if (scopeSlot.Location == ObjectReferenceLocation.Parameter)
                        {
                            _il.LoadArgument(scopeSlot.Address);
                        }
                        else if (scopeSlot.Location == ObjectReferenceLocation.ScopeArray)
                        {
                            _il.LoadArgument(0);
                            _il.LoadConstantI4(scopeSlot.Address);
                            _il.OpCode(ILOpCode.Ldelem_ref);
                        }
                        else
                        {
                            _il.LoadLocal(scopeSlot.Address);
                        }
                        _il.OpCode(ILOpCode.Ldfld);
                        _il.Token(fnVar.FieldHandle); // stack: target delegate (object)

                        // Build scopes[] to bind: for closures we include global (if any) and the parent local
                        var neededScopeNames = GetScopesForClosureBinding(fnVar).ToList();
                        _il.LoadConstantI4(neededScopeNames.Count);
                        _il.OpCode(ILOpCode.Newarr);
                        _il.Token(_bclReferences.ObjectType);
                        for (int i = 0; i < neededScopeNames.Count; i++)
                        {
                            var sn = neededScopeNames[i];
                            var refSlot = _variables.GetScopeLocalSlot(sn);
                            _il.OpCode(ILOpCode.Dup);
                            _il.LoadConstantI4(i);
                            if (refSlot.Location == ObjectReferenceLocation.Local)
                            {
                                _il.LoadLocal(refSlot.Address);
                            }
                            else if (refSlot.Location == ObjectReferenceLocation.Parameter)
                            {
                                _il.LoadArgument(refSlot.Address);
                            }
                            else if (refSlot.Location == ObjectReferenceLocation.ScopeArray)
                            {
                                _il.LoadArgument(0);
                                _il.LoadConstantI4(refSlot.Address);
                                _il.OpCode(ILOpCode.Ldelem_ref);
                            }
                            _il.OpCode(ILOpCode.Stelem_ref);
                        }

                        // Closure.Bind(object, object[])
                        // Ensure delegate is treated as object for the bind call
                        // (ldfld already leaves it as the specific Func<...>; call will accept object)
                        _runtime.InvokeClosureBindObject();
                    }
                    else
                    {
                        // Fallback to normal emit
                        var type = _expressionEmitter.Emit(returnStatement.Argument, new TypeCoercion());
                        if (type == JavascriptType.Number)
                        {
                            _il.OpCode(ILOpCode.Box);
                            _il.Token(_bclReferences.DoubleType);
                        }
                    }
                }
                else
                {
                    var type = _expressionEmitter.Emit(returnStatement.Argument, new TypeCoercion());
                    if (type == JavascriptType.Number)
                    {
                        _il.OpCode(ILOpCode.Box);
                        _il.Token(_bclReferences.DoubleType);
                    }
                }
            }
            else
            {
                _il.OpCode(ILOpCode.Ldnull);
            }
            _il.OpCode(ILOpCode.Ret);
        }

        /// <summary>
        /// Determines which scopes to capture when binding a function value for closure.
        /// In a function context, capture [global(if present), caller local]. In Main, capture only the leaf scope.
        /// </summary>
        private IEnumerable<string> GetScopesForClosureBinding(Variable functionVariable)
        {
            var names = _variables.GetAllScopeNames().ToList();
            var slots = names.Select(n => new { Name = n, Slot = _variables.GetScopeLocalSlot(n) }).ToList();

            bool inFunctionContext = slots.Any(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray);
            if (!inFunctionContext)
            {
                yield return _variables.GetLeafScopeName();
                yield break;
            }

            // Global first if available (scopes[0])
            var global = slots.FirstOrDefault(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray && e.Slot.Address == 0);
            if (global != null)
            {
                yield return global.Name;
            }

            // Then the caller/local scope hosting the callee delegate
            if (!string.IsNullOrEmpty(functionVariable.ScopeName))
            {
                var parentLocal = slots.FirstOrDefault(e => e.Name == functionVariable.ScopeName && e.Slot.Location == ObjectReferenceLocation.Local);
                if (parentLocal != null)
                {
                    yield return parentLocal.Name;
                }
            }
        }

    public void GenerateExpressionStatement(Acornima.Ast.ExpressionStatement expressionStatement)
        { 
            switch (expressionStatement.Expression)
            {
                case Acornima.Ast.CallExpression callExpression:
                    // Handle CallExpression
            GenerateCallExpression(callExpression, CallSiteContext.Statement, discardResult: true);
                    break;
                case Acornima.Ast.AssignmentExpression assignmentExpression:
                    // Handle AssignmentExpression  
                    _expressionEmitter.Emit(assignmentExpression, new TypeCoercion());
                    break;
                case Acornima.Ast.BinaryExpression binaryExpression:
                    // Handle BinaryExpression
                    _binaryOperators.Generate(binaryExpression);
                    break;
                case Acornima.Ast.UpdateExpression updateExpression:
                    // Handle UpdateExpression
                    GenerateUpdateExpression(updateExpression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type in statement: {expressionStatement.Expression.Type}");
            }
        }

        public void GenerateForStatement(Acornima.Ast.ForStatement forStatement)
        {

            // first lets encode the initalizer
            if (forStatement.Init is Acornima.Ast.VariableDeclaration variableDeclaration)
            {
                DeclareVariable(variableDeclaration);
            }
            else
            {
                throw new NotSupportedException($"Unsupported for statement initializer type: {forStatement.Init?.Type}");
            }

            
            // the labels used in the loop flow control
            var loopStartLabel = _il.DefineLabel();
            var loopEndLabel = _il.DefineLabel();
            var loopBodyLabel = _il.DefineLabel();

            _il.MarkLabel(loopStartLabel);

            //the test condition in the for loop
            if (forStatement.Test != null)
            {
                this._expressionEmitter.Emit(forStatement.Test, new TypeCoercion(), branching: new ConditionalBranching
                {
                    BranchOnTrue = loopBodyLabel,
                    BranchOnFalse = loopEndLabel
                });
            }

            // now the body
            _il.MarkLabel(loopBodyLabel);

            GenerateStatement(forStatement.Body);

            if (forStatement.Update != null)
            {
                _expressionEmitter.Emit(forStatement.Update, new TypeCoercion());
            }

            // branch back to the start of the loop
            _il.Branch(ILOpCode.Br, loopStartLabel);

            // here is the end
            _il.MarkLabel(loopEndLabel);
        }

        private void GenerateIfStatement(IfStatement ifStatement)
        {
            var consequentLabel = _il.DefineLabel();
            var elseLabel = _il.DefineLabel();
            var endLabel = _il.DefineLabel();


            // Actually, we want: if (test) { consequent } else { alternate }
            // So: if test is false, jump to elseLabel, otherwise fall through to consequent
            // Fix: BranchOnTrue = consequentLabel, BranchOnFalse = elseLabel            
            
            _expressionEmitter.Emit(ifStatement.Test, new TypeCoercion(), new ConditionalBranching
            {
                BranchOnTrue = consequentLabel,
                BranchOnFalse = elseLabel
            });

            // Consequent (if block)\
            _il.MarkLabel(consequentLabel);
            GenerateStatement(ifStatement.Consequent);
            _il.Branch(ILOpCode.Br, endLabel);

            // Else/Alternate
            _il.MarkLabel(elseLabel);
            if (ifStatement.Alternate != null)
            {
                GenerateStatement(ifStatement.Alternate);
            }

            _il.MarkLabel(endLabel);
        }

        private void GenerateObjectExpresion(ObjectExpression objectExpression)
        {
            // first we need to creat a new instance of the expando object
            _il.OpCode(ILOpCode.Newobj);
            _il.Token(_bclReferences.Expando_Ctor_Ref);

            // we create a new object instance for the object expression
            // the generic solution is to use the ExpandoObject
            // will apply optimizations in the future when we can calculate the object schema
            foreach (var property in objectExpression.Properties)
            {
                if (property is not ObjectProperty objectProperty)
                {
                    throw new NotSupportedException($"Unsupported object property type: {property.Type}");
                }

                if (objectProperty.Key is not Identifier propertyKey)
                {
                    throw new NotSupportedException($"Unsupported object property key type: {objectProperty.Key.Type}");
                }

                if (objectProperty.Value is not Expression propertyValue)
                {
                    throw new NotSupportedException($"Unsupported object property value type: {objectProperty.Value.Type}");
                }

                // Duplicate the ExpandoObject reference on the stack
                _il.OpCode(ILOpCode.Dup); 

                // in a perfect world we could support any expression for the property name
                // but not feature rich enough to support that yet
                //_expressionEmitter.Emit(objectProperty.Key);
                _il.LoadString(_metadataBuilder.GetOrAddUserString(propertyKey.Name));

                // Load the value of the property
                var valueType = _expressionEmitter.Emit(propertyValue, new TypeCoercion());
                if (valueType == JavascriptType.Number)
                {
                    // If the value is a number, we need to box it to an object
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.DoubleType);
                }

                // call set_Item on the ExpandoObject to set the property value
                _il.OpCode(ILOpCode.Callvirt);
                _il.Token(_bclReferences.IDictionary_SetItem_Ref);
            }

            // After all properties are set, the ExpandoObject is on the stack
            // this is the expected behavior.  The consumer for this expression output chooses what to do with it.
        }

        private void GenerateArrayExpression(ArrayExpression arrayExpression)
        {
            // Use JavaScriptRuntime.Array (List<object>-backed) to preserve JS semantics
            // 1) push capacity
            _il.LoadConstantI4(arrayExpression.Elements.Count);
            // 2) invoke runtime array ctor (produces JavaScriptRuntime.Array instance boxed as object)
            _runtime.InvokeArrayCtor();

            // For each element: duplicate array ref, load element (boxed), call Add
            for (int i = 0; i < arrayExpression.Elements.Count; i++)
            {
                var element = arrayExpression.Elements[i];
                _il.OpCode(ILOpCode.Dup); // array instance
                var elemType = _expressionEmitter.Emit(element!, new TypeCoercion());
                if (elemType == JavascriptType.Number)
                {
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.DoubleType);
                }
                _il.OpCode(ILOpCode.Callvirt);
                _il.Token(_bclReferences.Array_Add_Ref);
            }
        }

        private void GenerateUpdateExpression(Acornima.Ast.UpdateExpression updateExpression)
        {
            if ((updateExpression.Operator != Acornima.Operator.Increment && updateExpression.Operator != Acornima.Operator.Decrement) || updateExpression.Prefix)
            {
                throw new NotSupportedException($"Unsupported update expression operator: {updateExpression.Operator} or prefix: {updateExpression.Prefix}");
            }
            // Handle postfix increment (x++) and decrement (x--)
            var variableName = (updateExpression.Argument as Acornima.Ast.Identifier)!.Name;
            var variable = _variables[variableName];
            
            // Handle scope field variables
            var scopeLocalIndex = _variables.GetScopeLocalSlot(variable.ScopeName);
            if (scopeLocalIndex.Address == -1)
            {
                throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
            }
            
            // Load scope instance for the store operation later
            if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
            {
                _il.LoadArgument(scopeLocalIndex.Address);
            }
            else
            {
                _il.LoadLocal(scopeLocalIndex.Address);
            }
            
            // Load the current value from scope field  
            _il.LoadLocal(scopeLocalIndex.Address);
            _il.OpCode(ILOpCode.Ldfld);
            _il.Token(variable.FieldHandle);
            
            // unbox the variable
            _il.OpCode(ILOpCode.Unbox_any);
            _il.Token(_bclReferences.DoubleType);
            
            // increment or decrement by 1
            _il.LoadConstantR8(1.0);
            if (updateExpression.Operator == Acornima.Operator.Increment)
            {
                _il.OpCode(ILOpCode.Add);
            }
            else // Decrement
            {
                _il.OpCode(ILOpCode.Sub);
            }
            
            // box the result back to an object
            _il.OpCode(ILOpCode.Box);
            _il.Token(_bclReferences.DoubleType);
            
            // Now stack is: [scope_instance] [boxed_result] - perfect for stfld
            _il.OpCode(ILOpCode.Stfld);
            _il.Token(variable.FieldHandle);
        }

    private void GenerateCallExpression(Acornima.Ast.CallExpression callExpression, CallSiteContext context, bool discardResult)
        {
            // For simplicity, we assume the call expression is a console write line
            if (callExpression.Callee is not Acornima.Ast.MemberExpression memberExpression ||
                memberExpression.Object is not Acornima.Ast.Identifier objectIdentifier ||
                objectIdentifier.Name != "console" ||
                memberExpression.Property is not Acornima.Ast.Identifier propertyIdentifier ||
                propertyIdentifier.Name != "log")
            {
                // try to invoke local function
                if (callExpression.Callee is Acornima.Ast.Identifier identifier)
                {

                    var functionVariable = _variables.FindVariable(identifier.Name);
                    if (functionVariable == null)
                    {
                        throw new ArgumentException($"Function {identifier.Name} is not defined.");
                    }

                    // Load the scope instance as the first parameter
                    Action loadScopeInstance;
                    var scopeObjectReference = _variables.GetScopeLocalSlot(functionVariable.ScopeName);
                    if (scopeObjectReference.Address == -1)
                    {
                        throw new InvalidOperationException($"Scope '{functionVariable.ScopeName}' not found in local slots");
                    }
                    if (scopeObjectReference.Location == ObjectReferenceLocation.Local)
                    {
                        loadScopeInstance = () => _il.LoadLocal(scopeObjectReference.Address);
                    }
                    else if (scopeObjectReference.Location == ObjectReferenceLocation.Parameter)
                    {
                        // Load the scope instance from the field
                        loadScopeInstance = () =>  _il.LoadArgument(scopeObjectReference.Address);
                    }
                    else if (scopeObjectReference.Location == ObjectReferenceLocation.ScopeArray)
                    {
                        // Load from scope array at index 0
                        loadScopeInstance = () => 
                        {
                            _il.LoadArgument(0); // Load scope array parameter
                            _il.LoadConstantI4(0); // Index 0 for global scope
                            _il.OpCode(ILOpCode.Ldelem_ref);
                        };
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported scope object reference location: {scopeObjectReference.Location}");
                    }

                    // load the delegate to be invoked (from scope field)
                    loadScopeInstance();
                    _il.OpCode(ILOpCode.Ldfld);
                    _il.Token(functionVariable.FieldHandle);

                    // First argument: create scope array with appropriate scopes for the function
                    // Only include scopes that are actually needed for this function call
                    var neededScopeNames = GetNeededScopesForFunction(functionVariable, context).ToList();
                    var arraySize = neededScopeNames.Count;
                    
                    _il.LoadConstantI4(arraySize); // Array size
                    _il.OpCode(ILOpCode.Newarr);
                    _il.Token(_bclReferences.ObjectType);
                    
                    // Fill the scope array with needed scopes only
                    for (int i = 0; i < neededScopeNames.Count; i++)
                    {
                        var scopeName = neededScopeNames[i];
                        var scopeRef = _variables.GetScopeLocalSlot(scopeName);
                        
                        _il.OpCode(ILOpCode.Dup); // Duplicate array reference
                        _il.LoadConstantI4(i);    // Load array index
                        
                        // Load the scope instance based on its location
                        if (scopeRef.Location == ObjectReferenceLocation.Local)
                        {
                            _il.LoadLocal(scopeRef.Address);
                        }
                        else if (scopeRef.Location == ObjectReferenceLocation.Parameter)
                        {
                            _il.LoadArgument(scopeRef.Address);
                        }
                        else if (scopeRef.Location == ObjectReferenceLocation.ScopeArray)
                        {
                            _il.LoadArgument(0); // Load scope array parameter
                            _il.LoadConstantI4(scopeRef.Address); // Load array index
                            _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
                        }
                        
                        _il.OpCode(ILOpCode.Stelem_ref); // Store scope in array
                    }

                    // Additional arguments: directly emit each call argument (boxed as needed)
                    // If this is a declared function we could validate arity, but for arrow functions or runtime values,
                    // we simply pass through the provided arguments.
                    for (int i = 0; i < callExpression.Arguments.Count; i++)
                    {
                        var argType = _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion());
                        if (argType == JavascriptType.Number)
                        {
                            _il.OpCode(ILOpCode.Box);
                            _il.Token(_bclReferences.DoubleType);
                        }
                    }

                    // Invoke correct delegate based on parameter count.
                    // Select overloads based on call-site context to match historical snapshots.
                    var argCount = callExpression.Arguments.Count;
                    _il.OpCode(ILOpCode.Callvirt);
                    if (context == CallSiteContext.Statement)
                    {
                        // Statement: array-based Invoke for 0/1 parameters
                        if (argCount == 0)
                        {
                            _il.Token(_bclReferences.FuncObjectArrayObject_Invoke_Ref);
                        }
                        else if (argCount == 1)
                        {
                            _il.Token(_bclReferences.FuncObjectArrayObjectObject_Invoke_Ref);
                        }
                        else if (argCount <= 6)
                        {
                            _il.Token(_bclReferences.GetFuncArrayParamInvokeRef(argCount));
                        }
                        else
                        {
                            throw new NotSupportedException($"Only up to 6 parameters supported currently (got {argCount})");
                        }
                    }
                    else
                    {
                        // Expression: non-array-based Invoke for 0/1 parameters; array-based for >=2
                        if (argCount == 0)
                        {
                            _il.Token(_bclReferences.FuncObjectObject_Invoke_Ref);
                        }
                        else if (argCount == 1)
                        {
                            _il.Token(_bclReferences.FuncObjectObjectObject_Invoke_Ref);
                        }
                        else if (argCount <= 6)
                        {
                            _il.Token(_bclReferences.GetFuncArrayParamInvokeRef(argCount));
                        }
                        else
                        {
                            throw new NotSupportedException($"Only up to 6 parameters supported currently (got {argCount})");
                        }
                    }
                    // Discard result if requested (statement context)
                    if (discardResult)
                    {
                        _il.OpCode(ILOpCode.Pop);
                    }
                    return;
                }
                else
                {
                    throw new NotSupportedException($"Unsupported call expression callee type: {callExpression.Callee.Type}");
                }

            }

            // console.log special-case
            CallConsoleWriteLine(callExpression);
            if (!discardResult)
            {
                // console.log returns undefined in JS; model as null on the stack when value is needed
                _il.OpCode(ILOpCode.Ldnull);
            }
        }

        private void CallConsoleWriteLine(Acornima.Ast.CallExpression callConsoleLog)
        {
            var arguments = callConsoleLog.Arguments;
            var argumentCount = arguments.Count;

            _il.LoadConstantI4(argumentCount);

            // create a array of parameters to pass to log
            _il.OpCode(ILOpCode.Newarr);
            _il.Token(_bclReferences.ObjectType);

            for (int i = 0; i < argumentCount; i++)
            {
                // Duplicate the array reference on the stack
                _il.OpCode(ILOpCode.Dup);
                _il.LoadConstantI4(i); // Load the index for the parameter
                var argument = callConsoleLog.Arguments[i];
                
                // Emit the argument expression
                this._expressionEmitter.Emit(argument, new TypeCoercion() { boxed = true });
                
                // Store the argument in the array at the specified index
                _il.OpCode(ILOpCode.Stelem_ref);
            }


            // call the runtime helper Console.Log
            _runtime.InvokeConsoleLog();
        }

        JavascriptType IMethodExpressionEmitter.Emit(Expression expression, TypeCoercion typeCoercion, ConditionalBranching? branching)
        {
            JavascriptType javascriptType = JavascriptType.Unknown;

            switch (expression)
            {
                    case AssignmentExpression assignmentExpression:
                        if (assignmentExpression.Left is Identifier aid)
                        {
                            var variable = _variables.FindVariable(aid.Name) ?? throw new InvalidOperationException($"Variable '{aid.Name}' not found");

                            // Load the appropriate scope instance that holds this field
                            var scopeSlot = _variables.GetScopeLocalSlot(variable.ScopeName);
                            if (scopeSlot.Address == -1)
                            {
                                throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
                            }
                            // Load scope instance
                            if (scopeSlot.Location == ObjectReferenceLocation.Parameter)
                            {
                                _il.LoadArgument(scopeSlot.Address);
                            }
                            else if (scopeSlot.Location == ObjectReferenceLocation.ScopeArray)
                            {
                                _il.LoadArgument(0); // Load scope array parameter
                                _il.LoadConstantI4(scopeSlot.Address); // Load array index
                                _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
                            }
                            else
                            {
                                _il.LoadLocal(scopeSlot.Address);
                            }
                            var rhsType = _expressionEmitter.Emit(assignmentExpression.Right, typeCoercion);
                            variable.Type = rhsType;
                            if (rhsType == JavascriptType.Number)
                            {
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(_bclReferences.DoubleType);
                            }
                            _il.OpCode(ILOpCode.Stfld);
                            _il.Token(variable.FieldHandle);
                            javascriptType = rhsType;
                        }
                        else
                        {
                            throw new NotSupportedException($"Unsupported assignment target type: {assignmentExpression.Left.Type}");
                        }
                        break;
                case CallExpression callExpression:
                    // Use the unified call generator in expression context, preserving the result
                    GenerateCallExpression(callExpression, CallSiteContext.Expression, discardResult: false);
                    javascriptType = JavascriptType.Object;
                    break;
                case ArrowFunctionExpression arrowFunction:
                    {
                        // Generate a static method for the arrow function and create a delegate instance.
                        // Use a deterministic but local owner type for simplicity.
                        var paramNames = arrowFunction.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                        // Use assignment-target name for registry scope, but keep IL method/type naming location-based to preserve snapshots
                        var registryScopeName = !string.IsNullOrEmpty(_currentAssignmentTarget)
                            ? $"ArrowFunction_{_currentAssignmentTarget}"
                            : $"ArrowFunction_L{arrowFunction.Location.Start.Line}C{arrowFunction.Location.Start.Column}";
                        var ilMethodName = $"ArrowFunction_L{arrowFunction.Location.Start.Line}C{arrowFunction.Location.Start.Column}";
                        var methodHandle = GenerateArrowFunctionMethod(arrowFunction, registryScopeName, ilMethodName, paramNames);

                        // ldnull ; ldftn method ; newobj Func<object[], object, ...>
                        _il.OpCode(ILOpCode.Ldnull);
                        _il.OpCode(ILOpCode.Ldftn);
                        _il.Token(methodHandle);
                        _il.OpCode(ILOpCode.Newobj);
                        var (_, ctorRef) = _bclReferences.GetFuncObjectArrayWithParams(paramNames.Length);
                        _il.Token(ctorRef);
                        javascriptType = JavascriptType.Object;
                    }
                    break;
                case ArrayExpression arrayExpression:
                    // Generate code for ArrayExpression
                    GenerateArrayExpression(arrayExpression);
                    javascriptType = JavascriptType.Object; // Arrays are treated as objects in JavaScript
                    break;
                case BinaryExpression binaryExpression:
                    _binaryOperators.Generate(binaryExpression, branching);
                    break;
                case NumericLiteral numericLiteral:
                    // Load numeric literal
                    _binaryOperators.LoadValue(expression, typeCoercion);

                    javascriptType = JavascriptType.Number;

                    break;
                case UpdateExpression updateExpression:
                    GenerateUpdateExpression(updateExpression);
                    break;
                case ObjectExpression objectExpression:
                    GenerateObjectExpresion(objectExpression);

                    javascriptType = JavascriptType.Object;
                    break;
                case MemberExpression memberExpression:
                    // Support arr.length and arr[computedIndex]
                    // Evaluate the object (base) expression first
                    if (memberExpression.Object is Identifier arrIdent)
                    {
                        var variable = _variables.FindVariable(arrIdent.Name);
                        if (variable == null)
                        {
                            throw new InvalidOperationException($"Variable '{arrIdent.Name}' not found for member expression.");
                        }
                        var scopeSlot = _variables.GetScopeLocalSlot(variable.ScopeName);
                        if (scopeSlot.Location == ObjectReferenceLocation.Parameter)
                        {
                            _il.LoadArgument(scopeSlot.Address);
                        }
                        else if (scopeSlot.Location == ObjectReferenceLocation.ScopeArray)
                        {
                            _il.LoadArgument(0); // Load scope array parameter
                            _il.LoadConstantI4(scopeSlot.Address); // Load array index
                            _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
                        }
                        else
                        {
                            _il.LoadLocal(scopeSlot.Address);
                        }
                        _il.OpCode(ILOpCode.Ldfld);
                        _il.Token(variable.FieldHandle); // stack: object (expected object[])
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported member base expression: {memberExpression.Object.Type}");
                    }

                    if (!memberExpression.Computed && memberExpression.Property is Identifier propId && propId.Name == "length")
                    {
                        // JavaScriptRuntime.Array exposes length via get_length() returning double
                        _il.OpCode(ILOpCode.Callvirt);
                        _il.Token(_bclReferences.Array_GetCount_Ref); // returns int32 count
                        // convert int -> double for JS number semantics
                        _il.OpCode(ILOpCode.Conv_r8);
                        javascriptType = JavascriptType.Number;
                    }
                    else if (memberExpression.Computed)
                    {
                        // arr[expr] -> runtime Object.GetItem(array, doubleIndex)
                        var indexType = _expressionEmitter.Emit(memberExpression.Property, new TypeCoercion());
                        if (indexType != JavascriptType.Number)
                        {
                            // ensure numeric (primitive coercion would go here; minimal support)
                            throw new NotSupportedException("Array index must be numeric expression");
                        }
                        // stack: array, double
                        _runtime.InvokeGetItemFromObject();
                        javascriptType = JavascriptType.Object;
                    }
                    else
                    {
                        throw new NotSupportedException("Only 'length' property or computed indexing supported on arrays.");
                    }
                    break;
                default:
                    javascriptType = _binaryOperators.LoadValue(expression, typeCoercion);
                    break;
            }

            return javascriptType;
        }

        private MethodDefinitionHandle GenerateArrowFunctionMethod(ArrowFunctionExpression arrowFunction, string registryScopeName, string ilMethodName, string[] paramNames)
        {
            // Build method body using a fresh ILMethodGenerator so we never mutate the parent generator's state
            var functionVariables = new Variables(_variables, registryScopeName, paramNames, isNestedFunction: true);
            var pnames = paramNames ?? Array.Empty<string>();
            var childGen = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);
            var il = childGen.IL;

            // For arrow functions, we do NOT pre-instantiate a local scope nor initialize parameter fields
            // unless the body actually requires a local scope (e.g., block with declarations). This keeps
            // expression-bodied arrows and simple block-return arrows minimal and matches snapshot baselines.

            // Emit body
            if (arrowFunction.Body is BlockStatement block)
            {
                // Fast-path: handle common pattern `{ const x = <expr>; return x; }`
                if (block.Body.Count == 2 &&
                    block.Body[0] is VariableDeclaration vdecl &&
                    (vdecl.Kind == VariableDeclarationKind.Const || vdecl.Kind == VariableDeclarationKind.Let) &&
                    vdecl.Declarations.Count == 1 &&
                    vdecl.Declarations[0].Id is Identifier vid &&
                    vdecl.Declarations[0].Init is Expression initExpr &&
                    block.Body[1] is ReturnStatement rstmt && rstmt.Argument is Identifier rid && rid.Name == vid.Name)
                {
                    // Optimized pattern: { const x = <expr>; return x; }
                    // If <expr> is a function (arrow/function expression), we must bind closure scopes before returning.
                    bool returnsFunctionInitializer = initExpr is ArrowFunctionExpression || initExpr is FunctionExpression;

                    if (returnsFunctionInitializer)
                    {
                        // If returning a function, ensure this arrow's local scope exists and parameter fields are initialized
                        var registry = functionVariables.GetVariableRegistry();
                        if (registry != null)
                        {
                            var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                            var hasAnyFields = fields.Any();
                            if (hasAnyFields)
                            {
                                ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);
                                // Initialize parameter fields if backing fields exist
                                var localScope = functionVariables.GetLocalScopeSlot();
                                if (localScope.Address >= 0 && pnames.Length > 0)
                                {
                                    var fieldNames = new HashSet<string>(fields.Select(f => f.Name));
                                    ushort jsParamSeq = 1; // arg0 is scopes[]
                                    foreach (var pn in pnames)
                                    {
                                        if (fieldNames.Contains(pn))
                                        {
                                            il.LoadLocal(localScope.Address);
                                            il.LoadArgument(jsParamSeq);
                                            var fh = registry.GetFieldHandle(registryScopeName, pn);
                                            il.OpCode(ILOpCode.Stfld);
                                            il.Token(fh);
                                        }
                                        jsParamSeq++;
                                    }
                                }
                            }
                        }

                        // Emit the initializer expression to produce the delegate on the stack
                        // Make sure inner arrow uses assignment-target-based registry naming (e.g., ArrowFunction_inner)
                        var prevAssignment = childGen._currentAssignmentTarget;
                        childGen._currentAssignmentTarget = vid.Name;
                        try
                        {
                            var t = ((IMethodExpressionEmitter)childGen).Emit(initExpr, new TypeCoercion());
                            if (t == JavascriptType.Number)
                            {
                                il.OpCode(ILOpCode.Box);
                                il.Token(_bclReferences.DoubleType);
                            }
                        }
                        finally
                        {
                            childGen._currentAssignmentTarget = prevAssignment;
                        }

                        // Build scopes[] to bind for closure: prefer [global(if any), this local]
                        var innerVar = functionVariables.FindVariable(vid.Name);
                        if (innerVar != null)
                        {
                            var neededScopeNames = childGen.GetScopesForClosureBinding(innerVar).ToList();
                            il.LoadConstantI4(neededScopeNames.Count);
                            il.OpCode(ILOpCode.Newarr);
                            il.Token(_bclReferences.ObjectType);
                            for (int i = 0; i < neededScopeNames.Count; i++)
                            {
                                var sn = neededScopeNames[i];
                                var refSlot = functionVariables.GetScopeLocalSlot(sn);
                                il.OpCode(ILOpCode.Dup);
                                il.LoadConstantI4(i);
                                if (refSlot.Location == ObjectReferenceLocation.Local)
                                {
                                    il.LoadLocal(refSlot.Address);
                                }
                                else if (refSlot.Location == ObjectReferenceLocation.Parameter)
                                {
                                    il.LoadArgument(refSlot.Address);
                                }
                                else if (refSlot.Location == ObjectReferenceLocation.ScopeArray)
                                {
                                    il.LoadArgument(0);
                                    il.LoadConstantI4(refSlot.Address);
                                    il.OpCode(ILOpCode.Ldelem_ref);
                                }
                                il.OpCode(ILOpCode.Stelem_ref);
                            }
                            // Bind the delegate on stack to the scopes[] we just built
                            childGen._runtime.InvokeClosureBindObject();
                        }
                        il.OpCode(ILOpCode.Ret);
                    }
                    else
                    {
                        // Not returning a function; just evaluate the expression and return it (no binding, no scope instantiation)
                        var t = ((IMethodExpressionEmitter)childGen).Emit(initExpr, new TypeCoercion());
                        if (t == JavascriptType.Number)
                        {
                            il.OpCode(ILOpCode.Box);
                            il.Token(_bclReferences.DoubleType);
                        }
                        il.OpCode(ILOpCode.Ret);
                    }
                }
                else
                {
                    // General fallback: emit statements; ensure a return at end if missing
                    // If the block declares any let/const or uses fields, create a local scope instance now
                    var registry = functionVariables.GetVariableRegistry();
                    if (registry != null)
                    {
                        var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                        var hasAnyFields = fields.Any();
                        if (hasAnyFields)
                        {
                            // Create the current arrow function scope instance
                            ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);

                            // Initialize parameter fields from CLR args when a backing field exists
                            var localScope = functionVariables.GetLocalScopeSlot();
                                if (localScope.Address >= 0 && pnames.Length > 0)
                            {
                                    var fieldNames = new HashSet<string>(fields.Select(f => f.Name));
                                    ushort jsParamSeq = 1; // arg0 is scopes[]; JS params start at 1
                                    foreach (var pn in pnames)
                                {
                                    if (fieldNames.Contains(pn))
                                    {
                                        il.LoadLocal(localScope.Address);
                                        il.LoadArgument(jsParamSeq);
                                        var fh = registry.GetFieldHandle(registryScopeName, pn);
                                        il.OpCode(ILOpCode.Stfld);
                                        il.Token(fh);
                                    }
                                    jsParamSeq++;
                                }
                            }
                        }
                    }

                    // Emit statements using the child generator
                    childGen.GenerateStatements(block.Body);
                    // If no explicit return executed, fall through and return null
                    il.OpCode(ILOpCode.Ldnull);
                    il.OpCode(ILOpCode.Ret);
                }
            }
            else
            {
                // Expression-bodied arrow: evaluate via the child generator's expression emitter to keep logic isolated
                var bodyExpr = arrowFunction.Body as Expression ?? throw new NotSupportedException("Arrow function body is not an expression");
                var t = ((IMethodExpressionEmitter)childGen).Emit(bodyExpr, new TypeCoercion());
                if (t == JavascriptType.Number)
                {
                    il.OpCode(ILOpCode.Box);
                    il.Token(_bclReferences.DoubleType);
                }
                il.OpCode(ILOpCode.Ret);
            }

            // Locals signature
            StandaloneSignatureHandle localSignature = default;
            MethodBodyAttributes bodyAttributes = MethodBodyAttributes.None;
            var localCount = functionVariables.GetNumberOfLocals();
            if (localCount > 0)
            {
                var localSig = new BlobBuilder();
                var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(localCount);
                for (int i = 0; i < localCount; i++)
                {
                    localEncoder.AddVariable().Type().Object();
                }
                localSignature = _metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(localSig));
                bodyAttributes = MethodBodyAttributes.InitLocals;
            }

            var bodyOffset = _methodBodyStreamEncoder.AddMethodBody(
                il,
                localVariablesSignature: localSignature,
                attributes: bodyAttributes);

            // Build method signature: static object (object[] scopes, object p1, ...)
            var sigBuilder = new BlobBuilder();
            var paramCount = 1 + pnames.Length;
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(paramCount, returnType => returnType.Type().Object(), parameters =>
                {
                    parameters.AddParameter().Type().SZArray().Object();
                    for (int i = 0; i < pnames.Length; i++) parameters.AddParameter().Type().Object();
                });
            var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

            // Add parameter metadata
            var firstParam = _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString("scopes"), sequenceNumber: 1);
            ushort seq = 2;
            foreach (var p in pnames)
            {
                _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString(p), sequenceNumber: seq++);
            }

            // Host the arrow method on its own type under Functions namespace
            var tb = new Js2IL.Utilities.Ecma335.TypeBuilder(_metadataBuilder, "Functions", ilMethodName);
            var mdh = tb.AddMethodDefinition(MethodAttributes.Static | MethodAttributes.Public, ilMethodName, methodSig, bodyOffset, firstParam);
            tb.AddTypeDefinition(TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, _bclReferences.ObjectType);
            return mdh;
        }

        /// <summary>
    /// Determines which scopes are needed for a specific function call.
    /// Rules:
    /// - In Main (no scope-array parameter): pass the current (leaf) scope instance.
    /// - In a function context (scope-array present): pass only the caller's local scope that holds the callee delegate.
    ///   Historical snapshots do not include the global scope alongside the caller local for nested calls.
        /// </summary>
    private enum CallSiteContext { Statement, Expression }

    private IEnumerable<string> GetNeededScopesForFunction(Variable functionVariable, CallSiteContext context)
        {
            var names = _variables.GetAllScopeNames().ToList();
            var slots = names.Select(n => new { Name = n, Slot = _variables.GetScopeLocalSlot(n) }).ToList();

            bool inFunctionContext = slots.Any(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray);
            if (!inFunctionContext)
            {
        // Main: pass only the current (leaf) scope
        var globalName = _variables.GetLeafScopeName();
        yield return globalName;
                yield break;
            }

            // Inside a function: include exactly the scope that owns the callee variable
            // - If the callee is stored on the current local scope: include that local scope only.
            // - If the callee lives on a parent/global scope: include that parent/global scope only.
            if (!string.IsNullOrEmpty(functionVariable.ScopeName))
            {
                var targetSlot = _variables.GetScopeLocalSlot(functionVariable.ScopeName);
                if (targetSlot.Location == ObjectReferenceLocation.Local)
                {
                    // Nested function declared in the current local scope
                    // Statement context snapshots include [global, local]; expression context snapshots include only [local]
                    if (context == CallSiteContext.Statement)
                    {
                        var globalEntry = slots.FirstOrDefault(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray && e.Slot.Address == 0);
                        if (globalEntry != null)
                        {
                            yield return globalEntry.Name; // global first
                        }
                    }
                    yield return functionVariable.ScopeName; // local (caller) scope
                }
                else if (targetSlot.Location == ObjectReferenceLocation.ScopeArray ||
                         targetSlot.Location == ObjectReferenceLocation.Parameter)
                {
                    // Callee lives on a parent/global scope: include only that owning scope
                    yield return functionVariable.ScopeName;
                }
            }
        }

        // Returns true if callee function body references any identifiers that are top-level globals
        // (names present in globalVarNames), excluding its own parameters/locals and skipping nested function bodies.
        private static bool CalleeReferencesAnyGlobals(FunctionDeclaration decl, HashSet<string> globalVarNames)
        {
            if (globalVarNames == null || globalVarNames.Count == 0) return false;

            var declared = new HashSet<string>(decl.Params.OfType<Identifier>().Select(p => p.Name));

            // Prime declared set with function-scoped declarations (var/let/const and function declarations) inside body
            CollectDeclaredNames(decl.Body, declared);

            return ContainsGlobalRef(decl.Body, globalVarNames, declared);
        }

        private static void CollectDeclaredNames(Node node, HashSet<string> declared)
        {
            if (node is VariableDeclaration vardecl)
            {
                foreach (var d in vardecl.Declarations)
                {
                    if (d.Id is Identifier id)
                    {
                        declared.Add(id.Name);
                    }
                }
            }
            else if (node is FunctionDeclaration fdecl)
            {
                if (fdecl.Id is Identifier fid) declared.Add(fid.Name);
                // Do not traverse into nested function bodies for declaration collection beyond adding its name
                return;
            }

            // Recurse over children
            var props = node.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(node);
                if (value is Node child)
                {
                    // Skip into nested functions when collecting declarations? We already marked their name; no need to descend
                    if (child is FunctionDeclaration) continue;
                    CollectDeclaredNames(child, declared);
                }
                else if (value is System.Collections.IEnumerable enumerable && value is not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node childNode)
                        {
                            if (childNode is FunctionDeclaration) continue;
                            CollectDeclaredNames(childNode, declared);
                        }
                    }
                }
            }
        }

        private static bool ContainsGlobalRef(Node node, HashSet<string> globals, HashSet<string> declared)
        {
            // If Identifier and not declared locally/params, treat as potential reference
            if (node is Identifier id)
            {
                if (globals.Contains(id.Name) && !declared.Contains(id.Name))
                {
                    return true;
                }
            }
            else if (node is MemberExpression mex)
            {
                // Visit the object side; skip non-computed property identifiers since they are not variable refs
                if (ContainsGlobalRef(mex.Object, globals, declared)) return true;
                if (mex.Computed && ContainsGlobalRef(mex.Property, globals, declared)) return true;
                return false;
            }
            else if (node is FunctionDeclaration)
            {
                // Do not look into nested function bodies
                return false;
            }

            var props = node.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(node);
                if (value is Node child)
                {
                    if (ContainsGlobalRef(child, globals, declared)) return true;
                }
                else if (value is System.Collections.IEnumerable enumerable && value is not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node childNode)
                        {
                            if (ContainsGlobalRef(childNode, globals, declared)) return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool FunctionHasNestedFunctions(string functionName)
        {
            var decl = _dispatchTableGenerator.GetFunctionDeclaration(functionName);
            if (decl == null) return false;
            return ContainsNestedFunction(decl.Body);
        }

        private static bool ContainsNestedFunction(Acornima.Ast.Node node)
        {
            // Walk child nodes; if any FunctionDeclaration is found, return true
            if (node is Acornima.Ast.FunctionDeclaration)
            {
                return true;
            }

            var props = node.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(node);
                if (value is Acornima.Ast.Node child)
                {
                    if (ContainsNestedFunction(child)) return true;
                }
                else if (value is System.Collections.IEnumerable enumerable && value is not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Acornima.Ast.Node childNode)
                        {
                            if (ContainsNestedFunction(childNode)) return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
