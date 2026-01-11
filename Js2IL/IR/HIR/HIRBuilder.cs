using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Immutable;
using Acornima.Ast;
using Js2IL.Services;
using Js2IL.Services.TwoPhaseCompilation;

using Js2IL.SymbolTables;

namespace Js2IL.HIR;
public static class HIRBuilder
{
    /// <summary>
    /// Attempts to parse a method from the AST node.
    /// Failure is not an error, just falls back to an older legacy IL emitter.
    /// TODO: Expand to make this the required path for IL emission.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool TryParseMethod([In, NotNull] Acornima.Ast.Node node, [In, NotNull] Scope scope, out HIRMethod? method)
    {
        switch (node)
        {
            case Acornima.Ast.Program programAst:
                var builder = new HIRMethodBuilder(scope);
                return builder.TryParseStatements(programAst.Body, out method);
            case Acornima.Ast.BlockStatement blockStmt:
                // Parse block statements by processing their body statements
                var blockBuilder = new HIRMethodBuilder(scope);
                return blockBuilder.TryParseStatements(blockStmt.Body, out method);
            case Acornima.Ast.MethodDefinition classMethodDef:
                var methodFuncExpr = classMethodDef.Value as FunctionExpression;                
                var methodBuilder = new HIRMethodBuilder(scope);
                return methodBuilder.TryParseStatements(methodFuncExpr.Body.Body, out method);
            case Acornima.Ast.ArrowFunctionExpression arrowFunc:
                // IR pipeline supports simple identifier parameters only
                // Fall back to legacy emitter for destructuring, defaults, rest patterns
                if (!AllParamsAreSimpleIdentifiers(arrowFunc.Params))
                {
                    method = null!;
                    return false;
                }
                var arrowBuilder = new HIRMethodBuilder(scope);

                // PL3.7a: concise-body arrows wrap implicit return
                if (arrowFunc.Body is BlockStatement arrowBlock)
                {
                    return arrowBuilder.TryParseStatements(arrowBlock.Body, out method);
                }

                if (arrowFunc.Body is Expression conciseExpr)
                {
                    return arrowBuilder.TryParseConciseBodyExpression(conciseExpr, out method);
                }

                method = null!;
                return false;
            case Acornima.Ast.FunctionExpression funcExpr:
                // FunctionExpression is used for class constructors and method values
                // IR pipeline supports simple identifier parameters only
                if (!AllParamsAreSimpleIdentifiers(funcExpr.Params))
                {
                    method = null!;
                    return false;
                }
                if (funcExpr.Body is not BlockStatement funcBlock)
                {
                    method = null!;
                    return false;
                }
                var funcExprBuilder = new HIRMethodBuilder(scope);
                return funcExprBuilder.TryParseStatements(funcBlock.Body, out method);
            // Handle other node types as needed
            default:
                method = null!;
                return false;
        }
    }

    /// <summary>
    /// Returns true if all parameters are simple identifiers or simple default parameter patterns.
    /// Supports: Identifier, AssignmentPattern with Identifier left-hand side.
    /// Does not support: destructuring patterns, rest patterns, nested defaults.
    /// </summary>
    internal static bool AllParamsAreSimpleIdentifiers(in NodeList<Node> parameters)
    {
        return parameters.All(param => param switch
        {
            Acornima.Ast.Identifier => true,
            Acornima.Ast.AssignmentPattern ap => ap.Left is Acornima.Ast.Identifier,
            _ => false
        });
    }
}

class HIRMethodBuilder
{
    /// <summary>
    /// JavaScript global constants that can be compiled as literals when not shadowed.
    /// </summary>
    private static readonly Dictionary<string, (JavascriptType Kind, object? Value)> KnownGlobalConstants = new()
    {
        ["undefined"] = (JavascriptType.Undefined, null),
        ["NaN"] = (JavascriptType.Number, double.NaN),
        ["Infinity"] = (JavascriptType.Number, double.PositiveInfinity),
    };

    readonly Scope _rootScope;
    Scope _currentScope;
    readonly List<HIRStatement> _statements = new();

    public HIRMethodBuilder(Scope scope)
    {
        ArgumentNullException.ThrowIfNull(scope, nameof(scope));
        _rootScope = scope;
        _currentScope = scope;
    }

    public bool TryParseStatements([In, NotNull] IEnumerable<Acornima.Ast.Statement> statements, out HIRMethod? method)
    {
        method = null;

        foreach (var statement in statements)
        {
            if (!TryParseStatement(statement, out var hirStatement))
            {
                return false;
            }
            _statements.Add(hirStatement!);
        }
        method = new HIRMethod
        {
            Body = new HIRBlock(_statements)
        };

        return true;
    }

    public bool TryParseConciseBodyExpression([In, NotNull] Acornima.Ast.Expression expression, out HIRMethod? method)
    {
        method = null;

        if (!TryParseExpression(expression, out var hirExpr))
        {
            return false;
        }

        method = new HIRMethod
        {
            Body = new HIRBlock(new List<HIRStatement> { new HIRReturnStatement(hirExpr) })
        };

        return true;
    }

    private bool TryParseStatement(Acornima.Ast.Statement statement, out HIRStatement? hirStatement)
    {
        hirStatement = null;

        switch (statement)
        {
            case EmptyStatement:
                // Empty statements can appear from stray semicolons (e.g., after function declarations).
                // Treat as a no-op.
                hirStatement = new HIRBlock([]);
                return true;

            case VariableDeclaration declStmt:
                // Variable declarations can have multiple declarators, handle them as a block
                var declStatements = new List<HIRStatement>();
                foreach (var decl in declStmt.Declarations)
                {
                    if (!TryParseDeclarator(decl, out var declHir))
                    {
                        return false;
                    }
                    declStatements.Add(declHir!);
                }
                // If single declaration, return it directly; otherwise wrap in block
                hirStatement = declStatements.Count == 1
                    ? declStatements[0]
                    : new HIRBlock(declStatements);
                return true;

            case ExpressionStatement exprStmt:
                if (!TryParseExpression(exprStmt.Expression, out var hirExpr))
                {
                    return false;
                }
                hirStatement = new HIRExpressionStatement(hirExpr!);
                return true;

            case IfStatement ifStmt:
                if (!TryParseExpression(ifStmt.Test, out var testExpr))
                {
                    return false;
                }

                if (!TryParseStatement(ifStmt.Consequent, out var consequentStmt))
                {
                    return false;
                }

                HIRStatement? alternateStmt = null;
                if (ifStmt.Alternate != null && !TryParseStatement(ifStmt.Alternate, out alternateStmt))
                {
                    return false;
                }

                hirStatement = new HIRIfStatement(testExpr!, consequentStmt!, alternateStmt);
                return true;

            case BlockStatement blockStmt:
                // Find the child scope for this block statement (if one exists)
                var blockScope = FindChildScopeForAstNode(blockStmt);
                var previousScope = _currentScope;
                if (blockScope != null)
                {
                    _currentScope = blockScope;
                }
                
                var blockStatements = new List<HIRStatement>();
                foreach (var innerStmt in blockStmt.Body)
                {
                    if (!TryParseStatement(innerStmt, out var innerHir))
                    {
                        _currentScope = previousScope;
                        return false;
                    }
                    blockStatements.Add(innerHir!);
                }
                
                // Restore the previous scope
                _currentScope = previousScope;
                hirStatement = new HIRBlock(blockStatements);
                return true;

            case FunctionDeclaration:
                // Function declarations are hoisted and compiled separately by JavaScriptFunctionGenerator.
                // In the main method body, we skip them (they're not executable statements).
                hirStatement = new HIRBlock([]); // empty block = no-op
                return true;

            case ClassDeclaration:
                // Class declarations are compiled separately by ClassesGenerator.
                // In the main method body, treat them as non-executable statements for the IR pipeline.
                hirStatement = new HIRBlock([]); // empty block = no-op
                return true;

            case ReturnStatement returnStmt:
                HIRExpression? returnExpr = null;
                if (returnStmt.Argument != null)
                {
                    if (!TryParseExpression(returnStmt.Argument, out returnExpr))
                    {
                        return false;
                    }
                }
                hirStatement = new HIRReturnStatement(returnExpr);
                return true;

            case ForStatement forStmt:
                {
                    // For loops with 'let' create a child block scope for the loop variable
                    // Find the child scope for the for loop (if one exists)
                    var forScope = FindChildScopeForAstNode(forStmt);
                    var previousForScope = _currentScope;
                    if (forScope != null)
                    {
                        _currentScope = forScope;
                    }

                    // Parse init - can be VariableDeclaration or Expression
                    HIRStatement? forInitStmt = null;
                    if (forStmt.Init != null)
                    {
                        if (forStmt.Init is VariableDeclaration varDecl)
                        {
                            var forDeclStatements = new List<HIRStatement>();
                            foreach (var decl in varDecl.Declarations)
                            {
                                if (!TryParseDeclarator(decl, out var declHir))
                                {
                                    _currentScope = previousForScope;
                                    return false;
                                }
                                forDeclStatements.Add(declHir!);
                            }
                            forInitStmt = forDeclStatements.Count == 1
                                ? forDeclStatements[0]
                                : new HIRBlock(forDeclStatements);
                        }
                        else if (forStmt.Init is Acornima.Ast.Expression initExpr)
                        {
                            if (!TryParseExpression(initExpr, out var hirInitExpr))
                            {
                                _currentScope = previousForScope;
                                return false;
                            }
                            forInitStmt = new HIRExpressionStatement(hirInitExpr!);
                        }
                        else
                        {
                            _currentScope = previousForScope;
                            return false;
                        }
                    }

                    // Parse test condition
                    HIRExpression? forTestExpr = null;
                    if (forStmt.Test != null && !TryParseExpression(forStmt.Test, out forTestExpr))
                    {
                        _currentScope = previousForScope;
                        return false;
                    }

                    // Parse update expression
                    HIRExpression? updateExpr = null;
                    if (forStmt.Update != null && !TryParseExpression(forStmt.Update, out updateExpr))
                    {
                        _currentScope = previousForScope;
                        return false;
                    }

                    // Parse body
                    if (!TryParseStatement(forStmt.Body, out var bodyStmt))
                    {
                        _currentScope = previousForScope;
                        return false;
                    }

                    // Restore the previous scope
                    _currentScope = previousForScope;
                    hirStatement = new HIRForStatement(forInitStmt, forTestExpr, updateExpr, bodyStmt!);
                    return true;
                }

            case ForOfStatement forOfStmt:
                {
                    // Parse RHS iterable expression
                    if (!TryParseExpression(forOfStmt.Right, out var iterableExpr))
                    {
                        return false;
                    }

                    // Parse body
                    if (!TryParseStatement(forOfStmt.Body, out var bodyStmt))
                    {
                        return false;
                    }

                    // Determine loop target identifier
                    string? targetName = null;
                    if (forOfStmt.Left is VariableDeclaration vd && vd.Declarations.Count == 1 && vd.Declarations[0].Id is Identifier vid)
                    {
                        targetName = vid.Name;
                    }
                    else if (forOfStmt.Left is Identifier id)
                    {
                        targetName = id.Name;
                    }

                    if (string.IsNullOrEmpty(targetName))
                    {
                        return false;
                    }

                    var symbol = _currentScope.FindSymbol(targetName!);
                    if (symbol == null)
                    {
                        return false;
                    }

                    hirStatement = new HIRForOfStatement(symbol, iterableExpr!, bodyStmt!);
                    return true;
                }

            case ForInStatement forInStmt:
                {
                    // Parse RHS enumerable expression
                    if (!TryParseExpression(forInStmt.Right, out var enumerableExpr))
                    {
                        return false;
                    }

                    // Parse body
                    if (!TryParseStatement(forInStmt.Body, out var bodyStmt))
                    {
                        return false;
                    }

                    // Determine loop target identifier
                    string? targetName = null;
                    if (forInStmt.Left is VariableDeclaration vd && vd.Declarations.Count == 1 && vd.Declarations[0].Id is Identifier vid)
                    {
                        targetName = vid.Name;
                    }
                    else if (forInStmt.Left is Identifier id)
                    {
                        targetName = id.Name;
                    }

                    if (string.IsNullOrEmpty(targetName))
                    {
                        return false;
                    }

                    var symbol = _currentScope.FindSymbol(targetName!);
                    if (symbol == null)
                    {
                        return false;
                    }

                    hirStatement = new HIRForInStatement(symbol, enumerableExpr!, bodyStmt!);
                    return true;
                }

            case WhileStatement whileStmt:
                {
                    if (!TryParseExpression(whileStmt.Test, out var whileTestExpr))
                    {
                        return false;
                    }

                    if (!TryParseStatement(whileStmt.Body, out var bodyStmt))
                    {
                        return false;
                    }

                    hirStatement = new HIRWhileStatement(whileTestExpr!, bodyStmt!);
                    return true;
                }

            case DoWhileStatement doWhileStmt:
                {
                    if (!TryParseStatement(doWhileStmt.Body, out var bodyStmt))
                    {
                        return false;
                    }

                    if (!TryParseExpression(doWhileStmt.Test, out var doWhileTestExpr))
                    {
                        return false;
                    }

                    hirStatement = new HIRDoWhileStatement(bodyStmt!, doWhileTestExpr!);
                    return true;
                }

            case BreakStatement breakStmt:
                hirStatement = new HIRBreakStatement(breakStmt.Label?.Name);
                return true;

            case ContinueStatement continueStmt:
                hirStatement = new HIRContinueStatement(continueStmt.Label?.Name);
                return true;

            case LabeledStatement labeledStmt:
                {
                    // Support labeled loops and labeled blocks so labeled break can target them.
                    if (!TryParseStatement(labeledStmt.Body, out var labeledBody))
                    {
                        return false;
                    }

                    var labelName = labeledStmt.Label?.Name;
                    if (string.IsNullOrEmpty(labelName))
                    {
                        return false;
                    }

                    hirStatement = labeledBody switch
                    {
                        HIRForStatement forStmt => new HIRForStatement(forStmt.Init, forStmt.Test, forStmt.Update, forStmt.Body, labelName),
                        HIRForOfStatement forOfStmt => new HIRForOfStatement(forOfStmt.Target, forOfStmt.Iterable, forOfStmt.Body, labelName),
                        HIRForInStatement forInStmt => new HIRForInStatement(forInStmt.Target, forInStmt.Enumerable, forInStmt.Body, labelName),
                        HIRWhileStatement whileStmt => new HIRWhileStatement(whileStmt.Test, whileStmt.Body, labelName),
                        HIRDoWhileStatement dws => new HIRDoWhileStatement(dws.Body, dws.Test, labelName),
                        _ => new HIRLabeledStatement(labelName, labeledBody!)
                    };

                    return true;
                }

            case SwitchStatement switchStmt:
                {
                    if (!TryParseExpression(switchStmt.Discriminant, out var discriminant))
                    {
                        return false;
                    }

                    var cases = new List<HIRSwitchCase>();
                    foreach (var sc in switchStmt.Cases)
                    {
                        HIRExpression? test = null;
                        if (sc.Test != null && !TryParseExpression(sc.Test, out test))
                        {
                            return false;
                        }

                        var consequent = new List<HIRStatement>();
                        foreach (var consStmt in sc.Consequent)
                        {
                            if (!TryParseStatement(consStmt, out var consHir))
                            {
                                return false;
                            }
                            consequent.Add(consHir!);
                        }

                        cases.Add(new HIRSwitchCase(test, consequent.ToImmutableArray()));
                    }

                    hirStatement = new HIRSwitchStatement(discriminant!, cases);
                    return true;
                }

            case TryStatement tryStmt:
                {
                    if (!TryParseStatement(tryStmt.Block, out var tryBlock))
                    {
                        return false;
                    }

                    BindingInfo? catchParamBinding = null;
                    HIRStatement? catchBody = null;
                    if (tryStmt.Handler != null)
                    {
                        // Catch clause body is a BlockStatement; parsing it will enter the correct child scope.
                        if (!TryParseStatement(tryStmt.Handler.Body, out catchBody))
                        {
                            return false;
                        }

                        if (tryStmt.Handler.Param is Identifier catchId)
                        {
                            // Catch parameter is block-scoped to the catch body.
                            // TryParseStatement restores _currentScope after parsing the catch block,
                            // so resolve the binding from the catch block's child scope directly.
                            var catchScope = FindChildScopeForAstNode(tryStmt.Handler.Body) ?? _currentScope;
                            catchParamBinding = catchScope.FindSymbol(catchId.Name).BindingInfo;
                        }
                        else if (tryStmt.Handler.Param != null)
                        {
                            return false;
                        }
                    }

                    HIRStatement? finallyBody = null;
                    if (tryStmt.Finalizer != null)
                    {
                        if (!TryParseStatement(tryStmt.Finalizer, out finallyBody))
                        {
                            return false;
                        }
                    }

                    hirStatement = new HIRTryStatement(tryBlock!, catchParamBinding, catchBody, finallyBody);
                    return true;
                }

            case ThrowStatement throwStmt:
                {
                    if (throwStmt.Argument == null || !TryParseExpression(throwStmt.Argument, out var argExpr))
                    {
                        return false;
                    }
                    hirStatement = new HIRThrowStatement(argExpr!);
                    return true;
                }

            default:
                // Unsupported statement type
                return false;
        }
    }

    private bool TryParseDeclarator(Acornima.Ast.VariableDeclarator decl, out HIRStatement? hirStatement)
    {
        hirStatement = null;

        if (decl.Id is Identifier id)
        {
            var symbol = _currentScope.FindSymbol(id.Name);
            if (decl.Init != null)
            {
                if (!TryParseExpression(decl.Init, out var hirInitExpr))
                {
                    return false;
                }

                hirStatement = new HIRVariableDeclaration(symbol, hirInitExpr);
                return true;
            }
            hirStatement = new HIRVariableDeclaration(symbol);
            return true;
        }

        return false;
    }

    private bool TryParseExpression(Acornima.Ast.Expression? expr, out HIRExpression? hirExpr)
    {
        hirExpr = null;

        switch (expr)
        {
            case ThisExpression:
                // PL3.5: ThisExpression support.
                // For now, only support 'this' inside class instance methods/constructors.
                // (Top-level/function 'this' semantics are more complex and remain legacy.)
                if (_rootScope.Parent?.Kind != ScopeKind.Class)
                {
                    return false;
                }
                hirExpr = new HIRThisExpression();
                return true;

            case TemplateLiteral templateLiteral:
                {
                    static string GetQuasiText(TemplateElement te)
                    {
                        var val = te.Value;
                        // Prefer cooked when available; fall back to raw.
                        var cooked = val.Cooked;
                        if (!string.IsNullOrEmpty(cooked))
                        {
                            return cooked!;
                        }
                        return val.Raw ?? string.Empty;
                    }

                    var quasis = new List<string>(templateLiteral.Quasis.Count);
                    foreach (var quasi in templateLiteral.Quasis)
                    {
                        quasis.Add(GetQuasiText(quasi));
                    }

                    var expressions = new List<HIRExpression>(templateLiteral.Expressions.Count);
                    foreach (var exprNode in templateLiteral.Expressions)
                    {
                        if (!TryParseExpression(exprNode, out var parsedExpr))
                        {
                            return false;
                        }
                        expressions.Add(parsedExpr!);
                    }

                    hirExpr = new HIRTemplateLiteralExpression(quasis, expressions);
                    return true;
                }

            case ConditionalExpression conditionalExpr:
                // Handle conditional (ternary) expressions: test ? consequent : alternate
                if (!TryParseExpression(conditionalExpr.Test, out var testExpr) ||
                    !TryParseExpression(conditionalExpr.Consequent, out var consequentExpr) ||
                    !TryParseExpression(conditionalExpr.Alternate, out var alternateExpr))
                {
                    return false;
                }

                hirExpr = new HIRConditionalExpression(testExpr!, consequentExpr!, alternateExpr!);
                return true;

            case LogicalExpression logicalExpr:
                // Handle logical expressions (&&, ||) as binary expressions.
                // Short-circuit semantics are handled during lowering.
                if (!TryParseExpression(logicalExpr.Left, out var logicalLeft) ||
                    !TryParseExpression(logicalExpr.Right, out var logicalRight))
                {
                    return false;
                }

                hirExpr = new HIRBinaryExpression(logicalExpr.Operator, logicalLeft!, logicalRight!);
                return true;

            case BinaryExpression binaryExpr:
                // Handle binary expressions
                HIRExpression? leftExpr;
                HIRExpression? rightExpr;

                if (!TryParseExpression(binaryExpr.Left, out leftExpr) ||
                    !TryParseExpression(binaryExpr.Right, out rightExpr))
                {
                    return false;
                }

                hirExpr = new HIRBinaryExpression(binaryExpr.Operator, leftExpr!, rightExpr!);
                return true;

            case CallExpression callExpr:
                // Handle call expressions
                HIRExpression? calleeExpr;
                var argExprs = new List<HIRExpression>();

                if (!TryParseExpression(callExpr.Callee, out calleeExpr))
                {
                    return false;
                }

                foreach (var arg in callExpr.Arguments)
                {
                    if (!TryParseExpression(arg, out var argHirExpr))
                    {
                        return false;
                    }
                    argExprs.Add(argHirExpr!);
                }

                hirExpr = new HIRCallExpression(calleeExpr!, argExprs);
                return true;

            case NewExpression newExpr:
                // PL3.3: NewExpression support in IR pipeline.
                // - PL3.3a: built-in Error types
                // - PL3.3b: user-defined classes
                // - PL3.3d: Array constructor semantics
                // - PL3.3e/f: String/Boolean/Number constructor sugar
                // - PL3.3g: intrinsic runtime constructors (Date/RegExp/Set/Promise/Int32Array/etc.)
                if (newExpr.Callee is not Identifier newCalleeId)
                {
                    return false;
                }

                var newCalleeSymbol = _currentScope.FindSymbol(newCalleeId.Name);
                if (newCalleeSymbol.Kind != BindingKind.Global)
                {
                    // User-defined class ctor: allow only when the binding is a ClassDeclaration.
                    if (newCalleeSymbol.BindingInfo.DeclarationNode is not ClassDeclaration)
                    {
                        return false;
                    }

                    var userArgs = new List<HIRExpression>();
                    foreach (var arg in newExpr.Arguments)
                    {
                        if (!TryParseExpression(arg, out var argHirExpr))
                        {
                            return false;
                        }
                        userArgs.Add(argHirExpr!);
                    }

                    hirExpr = new HIRNewExpression(new HIRVariableExpression(newCalleeSymbol), userArgs);
                    return true;
                }

                var calleeName = newCalleeId.Name;

                bool isBuiltInError = Js2IL.IR.BuiltInErrorTypes.IsBuiltInErrorTypeName(calleeName);
                bool isArrayCtor = string.Equals(calleeName, "Array", StringComparison.Ordinal);
                bool isStringCtor = string.Equals(calleeName, "String", StringComparison.Ordinal);
                bool isBooleanCtor = string.Equals(calleeName, "Boolean", StringComparison.Ordinal);
                bool isNumberCtor = string.Equals(calleeName, "Number", StringComparison.Ordinal);
                var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(calleeName);

                if (!isBuiltInError && !isArrayCtor && !isStringCtor && !isBooleanCtor && !isNumberCtor && intrinsicType == null)
                {
                    return false;
                }

                var newArgExprs = new List<HIRExpression>();
                foreach (var arg in newExpr.Arguments)
                {
                    if (!TryParseExpression(arg, out var argHirExpr))
                    {
                        return false;
                    }
                    newArgExprs.Add(argHirExpr!);
                }

                if (isBuiltInError && newArgExprs.Count > 1)
                {
                    // Match legacy behavior for built-in Error types.
                    return false;
                }

                // For constructor-sugar primitives, accept 0 or 1 argument only.
                if ((isStringCtor || isBooleanCtor || isNumberCtor) && newArgExprs.Count > 1)
                {
                    return false;
                }

                // For generic intrinsic ctors, keep the IR surface conservative for now.
                // We only lower common ctor shapes: .ctor(), .ctor(object), .ctor(object, object).
                if (intrinsicType != null && !(isArrayCtor || isStringCtor || isBooleanCtor || isNumberCtor))
                {
                    bool isStaticClass = intrinsicType.IsAbstract && intrinsicType.IsSealed;
                    if (isStaticClass)
                    {
                        // Constructible intrinsics must be non-static classes.
                        return false;
                    }
                    if (newArgExprs.Count > 2)
                    {
                        return false;
                    }
                }

                hirExpr = new HIRNewExpression(new HIRVariableExpression(newCalleeSymbol), newArgExprs);
                return true;

            case UpdateExpression updateExpr:
                if (!TryParseExpression(updateExpr.Argument, out var updateArgExpr))
                {
                    return false;
                }

                hirExpr = new HIRUpdateExpression(updateExpr.Operator, updateExpr.Prefix, updateArgExpr!);
                return true;

            case UnaryExpression unaryExpr:
                if (!TryParseExpression(unaryExpr.Argument, out var unaryArgExpr))
                {
                    return false;
                }

                hirExpr = new HIRUnaryExpression(unaryExpr.Operator, unaryArgExpr!);
                return true;
            case Identifier identifierExpr:
                // FindSymbol always returns a Symbol; BindingKind.Global indicates an undeclared/ambient global.
                // If the symbol is user-declared (var/let/const/function), it shadows any global constant.
                var symbol = _currentScope.FindSymbol(identifierExpr.Name);
                if (symbol.Kind != BindingKind.Global)
                {
                    hirExpr = new HIRVariableExpression(symbol);
                    return true;
                }

                // Synthetic global binding: prefer known global constants when not shadowed.
                if (KnownGlobalConstants.TryGetValue(identifierExpr.Name, out var constant))
                {
                    hirExpr = new HIRLiteralExpression(constant.Kind, constant.Value);
                    return true;
                }

                // Otherwise treat it as a global variable reference (e.g., console/require).
                hirExpr = new HIRVariableExpression(symbol);
                return true;
            case AssignmentExpression assignExpr:
                // Handle assignment expressions (e.g., x = 5, y = x + 1)
                // Currently only support simple identifier targets
                if (assignExpr.Left is not Identifier assignTargetId)
                {
                    return false; // Only support identifier targets for now
                }

                var targetSymbol = _currentScope.FindSymbol(assignTargetId.Name);
                
                if (!TryParseExpression(assignExpr.Right, out var assignValueExpr))
                {
                    return false;
                }

                hirExpr = new HIRAssignmentExpression(targetSymbol, assignExpr.Operator, assignValueExpr!);
                return true;

            case MemberExpression memberExpr:
                // Handle member expressions
                HIRExpression? objectExpr;
                if (!TryParseExpression(memberExpr.Object, out objectExpr))
                {
                    return false;
                }

                if (memberExpr.Computed)
                {
                    // Computed property access: obj[expr]
                    if (!TryParseExpression(memberExpr.Property, out var indexExpr))
                    {
                        return false;
                    }
                    hirExpr = new HIRIndexAccessExpression(objectExpr!, indexExpr!);
                    return true;
                }

                if (memberExpr.Property is not Identifier propertyIdentifier)
                {
                    return false; // Only support identifier properties for non-computed access
                }

                hirExpr = new HIRPropertyAccessExpression(objectExpr!, propertyIdentifier.Name);
                return true;

            case ArrowFunctionExpression arrowExpr:
                // PL3.7: ArrowFunctionExpression as an expression (closure creation)
                // We treat the arrow as an opaque callable value; its body is compiled separately.
                // Support is intentionally conservative: only simple identifier/default params.
                if (!HIRBuilder.AllParamsAreSimpleIdentifiers(arrowExpr.Params))
                {
                    return false;
                }

                var arrowScope = FindChildScopeForAstNode(arrowExpr);
                if (arrowScope == null)
                {
                    return false;
                }

                // Match CallableDiscovery conventions so CallableRegistry lookups succeed.
                string? assignmentTarget = null;
                if (arrowScope.Name.StartsWith("ArrowFunction_") && !arrowScope.Name.StartsWith("ArrowFunction_L"))
                {
                    assignmentTarget = arrowScope.Name.Substring("ArrowFunction_".Length);
                }

                var root = _currentScope;
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
                var moduleName = root.Name;
                var declaringScopeName = _currentScope.Kind == ScopeKind.Global
                    ? moduleName
                    : $"{moduleName}/{_currentScope.GetQualifiedName()}";

                var arrowCallableId = new CallableId
                {
                    Kind = CallableKind.Arrow,
                    DeclaringScopeName = declaringScopeName,
                    Name = assignmentTarget,
                    Location = SourceLocation.FromNode(arrowExpr),
                    JsParamCount = arrowExpr.Params.Count,
                    AstNode = null
                };

                hirExpr = new HIRArrowFunctionExpression(arrowCallableId, arrowScope);
                return true;

            case FunctionExpression funcExpr:
                // PL3.6: FunctionExpression as an expression (closure creation)
                // Treat the function expression as an opaque callable value; its body is compiled separately.
                // Support is intentionally conservative: only simple identifier/default params.
                if (!HIRBuilder.AllParamsAreSimpleIdentifiers(funcExpr.Params))
                {
                    return false;
                }

                var funcScope = FindChildScopeForAstNode(funcExpr);
                if (funcScope == null)
                {
                    return false;
                }

                var root2 = _currentScope;
                while (root2.Parent != null)
                {
                    root2 = root2.Parent;
                }
                var moduleName2 = root2.Name;
                var declaringScopeName2 = _currentScope.Kind == ScopeKind.Global
                    ? moduleName2
                    : $"{moduleName2}/{_currentScope.GetQualifiedName()}";

                var functionName = (funcExpr.Id as Identifier)?.Name;
                var funcCallableId = new CallableId
                {
                    Kind = CallableKind.FunctionExpression,
                    DeclaringScopeName = declaringScopeName2,
                    Name = functionName,
                    Location = SourceLocation.FromNode(funcExpr),
                    JsParamCount = funcExpr.Params.Count,
                    AstNode = null
                };

                hirExpr = new HIRFunctionExpression(funcCallableId, funcScope);
                return true;

            case NumericLiteral literalExpr:
                hirExpr = new HIRLiteralExpression(JavascriptType.Number, literalExpr.Value);
                return true;
            case StringLiteral stringLiteralExpr:
                hirExpr = new HIRLiteralExpression(JavascriptType.String, stringLiteralExpr.Value);
                return true;
            case BooleanLiteral booleanLiteralExpr:
                hirExpr = new HIRLiteralExpression(JavascriptType.Boolean, booleanLiteralExpr.Value);
                return true;
            case Literal genericLiteral when genericLiteral.Value is null:
                // JavaScript 'null' literal
                hirExpr = new HIRLiteralExpression(JavascriptType.Null, null);
                return true;

            case ArrayExpression arrayExpr:
                // Parse array literal elements including spread elements
                var arrayElements = new List<HIRExpression>();
                foreach (var element in arrayExpr.Elements)
                {
                    if (element is SpreadElement spreadElement)
                    {
                        // Parse the spread element's argument
                        if (!TryParseExpression(spreadElement.Argument, out var spreadArgHir))
                        {
                            return false;
                        }
                        arrayElements.Add(new HIRSpreadElement(spreadArgHir!));
                    }
                    else
                    {
                        if (!TryParseExpression(element, out var elementHir))
                        {
                            return false;
                        }
                        arrayElements.Add(elementHir!);
                    }
                }
                hirExpr = new HIRArrayExpression(arrayElements);
                return true;

            case ObjectExpression objExpr:
                // Parse object literal properties
                var objectProperties = new List<HIRObjectProperty>();
                foreach (var property in objExpr.Properties)
                {
                    if (property is not ObjectProperty objProp)
                    {
                        // Spread properties, method definitions etc. not yet supported
                        return false;
                    }

                    // Determine property key name
                    string? propName = null;
                    if (objProp.Key is Identifier keyId)
                    {
                        propName = keyId.Name;
                    }
                    else if (objProp.Key is StringLiteral strLit)
                    {
                        propName = strLit.Value;
                    }
                    else if (objProp.Key is NumericLiteral numLit)
                    {
                        propName = numLit.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        // Computed property keys not yet supported
                        return false;
                    }

                    // Parse property value (Value is a Node, cast to Expression)
                    if (objProp.Value is not Acornima.Ast.Expression valueExpression)
                    {
                        // Property value is not an expression (e.g., shorthand method)
                        return false;
                    }
                    if (!TryParseExpression(valueExpression, out var valueExpr))
                    {
                        return false;
                    }

                    objectProperties.Add(new HIRObjectProperty(propName!, valueExpr!));
                }
                hirExpr = new HIRObjectExpression(objectProperties);
                return true;

            // Handle other expression types as needed
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Finds the child scope that corresponds to the given AST node.
    /// Returns null if no matching child scope exists.
    /// </summary>
    private Scope? FindChildScopeForAstNode(Node astNode)
    {
        return _currentScope.Children.FirstOrDefault(child => ReferenceEquals(child.AstNode, astNode));
    }
}