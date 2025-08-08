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
        private Variables _variables;
        private BaseClassLibraryReferences _bclReferences;
        private MetadataBuilder _metadataBuilder;
        private InstructionEncoder _il;
        private BinaryOperators _binaryOperators;
        private IMethodExpressionEmitter _expressionEmitter;
        private Runtime _runtime;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private MethodDefinitionHandle _firstMethod = default;

        private Dispatch.DispatchTableGenerator _dispatchTableGenerator;

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

            // add the variable to the collection
            var variable = _variables.CreateLocal(variableName);

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
                variable.Type = this._expressionEmitter.Emit(variableAST.Init, new TypeCoercion());
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
            var functionVariable = _variables.CreateLocal(functionName);

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
                    // Handle BlockStatement
                    GenerateStatements(blockStatement.Body);
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

        private void GenerateReturnStatement(ReturnStatement returnStatement)
        {
            if (returnStatement.Argument != null)
            {
                var type = _expressionEmitter.Emit(returnStatement.Argument, new TypeCoercion());
                if (type == JavascriptType.Number)
                {
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.DoubleType);
                }
            }
            else
            {
                _il.OpCode(ILOpCode.Ldnull);
            }
            _il.OpCode(ILOpCode.Ret);
        }

        public void GenerateExpressionStatement(Acornima.Ast.ExpressionStatement expressionStatement)
        { 
            switch (expressionStatement.Expression)
            {
                case Acornima.Ast.CallExpression callExpression:
                    // Handle CallExpression
                    GenerateCallExpression(callExpression);
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

        private void GenerateCallExpression(Acornima.Ast.CallExpression callExpression)
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

                    // First argument: create scope array with current scope
                    _il.LoadConstantI4(1); // Array size
                    _il.OpCode(ILOpCode.Newarr);
                    _il.Token(_bclReferences.ObjectType);
                    _il.OpCode(ILOpCode.Dup);
                    _il.LoadConstantI4(0);
                    loadScopeInstance();
                    _il.OpCode(ILOpCode.Stelem_ref);

                    // Additional arguments: directly emit each call argument (boxed as needed)
                    var funcDecl = _dispatchTableGenerator.GetFunctionDeclaration(identifier.Name);
                    if (funcDecl != null)
                    {
                        if (callExpression.Arguments.Count != funcDecl.Params.Count)
                        {
                            throw new InvalidOperationException("Argument count mismatch");
                        }
                        for (int i = 0; i < callExpression.Arguments.Count; i++)
                        {
                            var argType = _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion());
                            if (argType == JavascriptType.Number)
                            {
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(_bclReferences.DoubleType);
                            }
                        }
                    }
                    else if (callExpression.Arguments.Count > 0)
                    {
                        throw new InvalidOperationException("Function declaration not found for arguments");
                    }

                    // Invoke correct delegate based on parameter count
                    if (callExpression.Arguments.Count == 0)
                    {
                        _il.OpCode(ILOpCode.Callvirt);
                        _il.Token(_bclReferences.FuncObjectArrayObject_Invoke_Ref);
                    }
                    else if (callExpression.Arguments.Count == 1)
                    {
                        _il.OpCode(ILOpCode.Callvirt);
                        _il.Token(_bclReferences.FuncObjectArrayObjectObject_Invoke_Ref);
                    }
                    else
                    {
                        throw new NotSupportedException("Only up to 1 parameter supported currently");
                    }
                    // For expression statements, discard return value
                    _il.OpCode(ILOpCode.Pop);
                    return;
                }
                else
                {
                    throw new NotSupportedException($"Unsupported call expression callee type: {callExpression.Callee.Type}");
                }

            }

            CallConsoleWriteLine(callExpression);
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
                    // Reuse existing statement-level call generation but keep return value if needed
                    // Currently GenerateCallExpression handles console.log (void) and variable/member/identifier calls
                    // For expression context we need the result of the call on stack; existing GenerateCallExpression
                    // emits or discards values via Pop depending on usage. We'll implement minimal direct handling here.
                    if (callExpression.Callee is Identifier id)
                    {
                        // Load target function delegate from scope variable field
                        var variable = _variables.FindVariable(id.Name)!;
                        if (variable == null)
                        {
                            throw new InvalidOperationException($"Function '{id.Name}' not found in current scope.");
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
                        _il.Token(variable.FieldHandle);

                        // Create scope array with single element for now (TODO: proper scope chain)
                        _il.LoadConstantI4(1); // Array size
                        _il.OpCode(ILOpCode.Newarr);
                        _il.Token(_bclReferences.ObjectType);
                        _il.OpCode(ILOpCode.Dup); // Duplicate array reference
                        _il.LoadConstantI4(0); // Index 0
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
                        _il.OpCode(ILOpCode.Stelem_ref); // Store scope in array

                        // load each argument
                        foreach (var arg in callExpression.Arguments)
                        {
                            _expressionEmitter.Emit(arg, new TypeCoercion() { boxed = true });
                        }

                        // call Invoke on appropriate Func
                        if (callExpression.Arguments.Count == 0)
                        {
                            _il.OpCode(ILOpCode.Callvirt);
                            _il.Token(_bclReferences.FuncObjectObject_Invoke_Ref);
                        }
                        else if (callExpression.Arguments.Count == 1)
                        {
                            _il.OpCode(ILOpCode.Callvirt);
                            _il.Token(_bclReferences.FuncObjectObjectObject_Invoke_Ref);
                        }
                        else
                        {
                            throw new NotSupportedException("Only up to one argument supported in function calls currently");
                        }
                        javascriptType = JavascriptType.Object;
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported call expression callee type in expression context: {callExpression.Callee.Type}");
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
    }
}
