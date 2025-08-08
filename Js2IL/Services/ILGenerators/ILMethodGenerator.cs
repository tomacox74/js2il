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

        public void DeclareFunctions(IEnumerable<FunctionDeclaration> functionDeclarations)
        {
            // Iterate through each function declaration in the block
            foreach (var functionDeclaration in functionDeclarations)
            {
                DeclareFunction(functionDeclaration);
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
            else
            {
                _il.LoadLocal(scopeLocalIndex.Address);
            }
            _il.OpCode(ILOpCode.Ldsfld);
            _il.Token(dispatchDelegateField);
            _il.OpCode(ILOpCode.Stfld);
            _il.Token(functionVariable.FieldHandle);
        }


        public void DeclareFunction(FunctionDeclaration functionDeclaration)
        {
            var functionVariables = new Variables(_variables);
            var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);

            var methodDefinition = methodGenerator.GenerateMethodForFunction(functionDeclaration);
            if (this._firstMethod.IsNil)
            {
                this._firstMethod = methodDefinition;
            }
        }

        /// <summary>
        /// Generates the method (IL + metadata) for a function declaration using the current generator's context.
        /// Intended to be called on a dedicated ILMethodGenerator instance whose Variables represent the function scope.
        /// </summary>
        /// <param name="functionDeclaration">The function AST node.</param>
        /// <returns>The MethodDefinitionHandle created for the function.</returns>
        public MethodDefinitionHandle GenerateMethodForFunction(FunctionDeclaration functionDeclaration)
        {
            var functionName = (functionDeclaration.Id as Acornima.Ast.Identifier)!.Name;

            if (functionDeclaration.Body is not BlockStatement blockStatement)
            {
                throw new NotSupportedException($"Unsupported function body type: {functionDeclaration.Body.Type}");
            }

            // Register parameters in Variables before emitting body so identifiers resolve
            int paramBaseIndex = 1; // 0 = scope
            for (int i = 0; i < functionDeclaration.Params.Count; i++)
            {
                if (functionDeclaration.Params[i] is Acornima.Ast.Identifier pidPre)
                {
                    _variables.AddParameter(pidPre.Name, paramBaseIndex + i);
                }
            }

            // Emit body statements
            GenerateStatements(blockStatement.Body);
            // Implicit return undefined => null
            _il.OpCode(ILOpCode.Ldnull);
            _il.OpCode(ILOpCode.Ret);

            // Add method body (no locals for functions yet)
            var bodyoffset = _methodBodyStreamEncoder.AddMethodBody(
                _il,
                localVariablesSignature: default,
                attributes: MethodBodyAttributes.None);
            // Build method signature: static object (object scope, object param1, ...)
            var sigBuilder = new BlobBuilder();
            var paramCount = 1 + functionDeclaration.Params.Count; // scope + declared params
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(paramCount, returnType => returnType.Type().Object(), parameters =>
                {
                    // scope parameter
                    parameters.AddParameter().Type().Object();
                    // each JS parameter as System.Object for now
                    foreach (var p in functionDeclaration.Params)
                    {
                        parameters.AddParameter().Type().Object();
                    }
                });
            var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

            // Add parameters with names
            var scopeNames = _variables.GetAllScopeNames().ToList();
            var scopeParamName = scopeNames.FirstOrDefault() ?? functionName;
            ParameterHandle firstParamHandle = _metadataBuilder.AddParameter(
                ParameterAttributes.None,
                _metadataBuilder.GetOrAddString(scopeParamName),
                sequenceNumber: 1);
            // subsequent params
            ushort seq = 2;
            foreach (var p in functionDeclaration.Params)
            {
                if (p is Acornima.Ast.Identifier pid)
                {
                    _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString(pid.Name), sequenceNumber: seq++);
                }
                else
                {
                    _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString($"param{seq-1}"), sequenceNumber: seq++);
                }
            }

            var methodDefinition = _metadataBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                MethodImplAttributes.IL,
                _metadataBuilder.GetOrAddString(functionName),
                methodSig,
                bodyoffset,
                parameterList: firstParamHandle);

            // Register with dispatch table
            _dispatchTableGenerator.SetMethodDefinitionHandle(functionName, methodDefinition);

            return methodDefinition;
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
                case EmptyStatement:
                    // Empty statements (like standalone semicolons) do nothing
                    break;
                default:
                    throw new NotSupportedException($"Unsupported statement type: {statement.Type}");
            }
        }

        public void GenerateExpressionStatement(Acornima.Ast.ExpressionStatement expressionStatement)
        { 
            switch (expressionStatement.Expression)
            {
                case Acornima.Ast.CallExpression callExpression:
                    // Handle CallExpression
                    GenerateCallExpression(callExpression);
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
            // create a new array of type object
            // push the array size onto the stack
            _il.LoadConstantI4(arrayExpression.Elements.Count);
            _runtime.InvokeArrayCtor();

            // enumerate over the array elements loading each one and setting it in the array
            for (int i = 0; i < arrayExpression.Elements.Count; i++)
            {
                var element = arrayExpression.Elements[i];

                // Duplicate the array reference on the stack
                _il.OpCode(ILOpCode.Dup);

                // Emit the element expression to get its value
                _expressionEmitter.Emit(element!, new TypeCoercion());

                // Store the value in the array at the specified index
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
                    else
                    {
                        throw new InvalidOperationException($"Unsupported scope object reference location: {scopeObjectReference.Location}");
                    }

                    // load the delegate to be invoked (from scope field)
                    loadScopeInstance();
                    _il.OpCode(ILOpCode.Ldfld);
                    _il.Token(functionVariable.FieldHandle);

                    // First argument: scope instance
                    loadScopeInstance();

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
                        _il.Token(_bclReferences.FuncObjectObject_Invoke_Ref);
                    }
                    else if (callExpression.Arguments.Count == 1)
                    {
                        _il.OpCode(ILOpCode.Callvirt);
                        _il.Token(_bclReferences.FuncObjectObjectObject_Invoke_Ref);
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
                default:
                    javascriptType = _binaryOperators.LoadValue(expression, typeCoercion);
                    break;
            }

            return javascriptType;
        }
    }
}
