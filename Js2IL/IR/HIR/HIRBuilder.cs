using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Immutable;
using Acornima.Ast;
using Js2IL.DebugSymbols;
using Js2IL.Services;
using Js2IL.Services.TwoPhaseCompilation;
using ScopesCallableKind = Js2IL.Services.ScopesAbi.CallableKind;

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
    public static bool TryParseMethod(
        [In, NotNull] Acornima.Ast.Node node,
        [In, NotNull] Scope scope,
        ScopesCallableKind callableKind,
        bool hasScopesParameter,
        out HIRMethod? method)
    {
        static bool TryGetEnclosingClass(Scope startingScope, [NotNullWhen(true)] out Scope? classScope, [NotNullWhen(true)] out ClassDeclaration? classDecl)
        {
            classScope = null;
            classDecl = null;

            var current = startingScope;
            while (current != null)
            {
                if (current.Kind == ScopeKind.Class && current.AstNode is ClassDeclaration cd)
                {
                    classScope = current;
                    classDecl = cd;
                    return true;
                }
                current = current.Parent;
            }

            return false;
        }

        static string GetRegistryClassName(Scope classScope)
        {
            var ns = classScope.DotNetNamespace ?? "Classes";
            var name = classScope.DotNetTypeName ?? classScope.Name;
            return $"{ns}.{name}";
        }

        switch (node)
        {
            case Acornima.Ast.Program programAst:
                var builder = new HIRMethodBuilder(scope);
                return builder.TryParseStatements(programAst.Body, [], out method);
            case Acornima.Ast.BlockStatement blockStmt:
                // Parse block statements by processing their body statements
                var blockBuilder = new HIRMethodBuilder(scope);
                return blockBuilder.TryParseStatements(blockStmt.Body, [], out method);

            case Acornima.Ast.ClassDeclaration classDeclNode when callableKind == ScopesCallableKind.ClassStaticInitializer:
                {
                    // Synthesize a .cctor body from static field initializers.
                    if (!TryGetEnclosingClass(scope, out var enclosingClassScope, out var enclosingClassDecl))
                    {
                        method = null!;
                        return false;
                    }

                    var registryClassName = GetRegistryClassName(enclosingClassScope);
                    var cctorBuilder = new HIRMethodBuilder(scope);

                    foreach (var element in classDeclNode.Body.Body.OfType<PropertyDefinition>())
                    {
                        if (!element.Static || element.Value == null) continue;

                        if (!cctorBuilder.TryParseExpressionForPrologue((Expression)element.Value, out var hirValue) || hirValue == null)
                        {
                            method = null!;
                            return false;
                        }

                        if (element.Key is PrivateIdentifier priv)
                        {
                            cctorBuilder.AddPrologueStatement(new HIRStoreUserClassStaticFieldStatement
                            {
                                RegistryClassName = registryClassName,
                                FieldName = priv.Name,
                                IsPrivateField = true,
                                Value = hirValue,
                                Location = SourceLocation.FromNode(element)
                            });
                        }
                        else if (element.Key is Identifier pid)
                        {
                            cctorBuilder.AddPrologueStatement(new HIRStoreUserClassStaticFieldStatement
                            {
                                RegistryClassName = registryClassName,
                                FieldName = pid.Name,
                                IsPrivateField = false,
                                Value = hirValue,
                                Location = SourceLocation.FromNode(element)
                            });
                        }
                    }

                    // No JS parameters for .cctor.
                    return cctorBuilder.TryParseStatements(Array.Empty<Statement>(), Array.Empty<HIRPattern>(), out method);
                }

            case Acornima.Ast.ClassBody classBody when callableKind == ScopesCallableKind.Constructor:
                {
                    // Synthetic/implicit constructor body.
                    if (!TryGetEnclosingClass(scope, out var enclosingClassScope, out var enclosingClassDecl))
                    {
                        method = null!;
                        return false;
                    }

                    var registryClassName = GetRegistryClassName(enclosingClassScope);
                    var ctorStatements = new List<HIRStatement>();

                    var isDerivedConstructor = enclosingClassDecl.SuperClass != null;

                    static int GetMaxSuperCtorArgCount(Scope classScope, ClassDeclaration classDecl)
                    {
                        if (classDecl.SuperClass is not Identifier superId)
                        {
                            return 0;
                        }

                        var superSymbol = classScope.FindSymbol(superId.Name);
                        if (superSymbol.BindingInfo.DeclarationNode is not ClassDeclaration baseDecl)
                        {
                            return 0;
                        }

                        var baseCtor = baseDecl.Body.Body
                            .OfType<Acornima.Ast.MethodDefinition>()
                            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");

                        if (baseCtor?.Value is not FunctionExpression baseCtorFunc)
                        {
                            return 0;
                        }

                        // If the base constructor uses rest parameters, we can't represent that
                        // in the IR pipeline today.
                        if (baseCtorFunc.Params.Any(p => p is RestElement))
                        {
                            return 0;
                        }

                        return baseCtorFunc.Params.Count;
                    }

                    // Default derived constructors in JS forward received args to super(...).
                    // We approximate by synthesizing N parameters (N = base ctor max param count when resolvable)
                    // and passing them through to the implicit super call.
                    var parameterPatterns = new List<HIRPattern>();
                    var superArgs = new List<HIRExpression>();
                    if (isDerivedConstructor)
                    {
                        var argCount = GetMaxSuperCtorArgCount(enclosingClassScope, enclosingClassDecl);
                        for (int i = 0; i < argCount; i++)
                        {
                            var paramName = $"__arg{i}";
                            if (!scope.Bindings.TryGetValue(paramName, out var binding))
                            {
                                binding = new BindingInfo(paramName, BindingKind.Var, scope, classBody);
                                scope.Bindings[paramName] = binding;
                            }

                            scope.Parameters.Add(paramName);
                            var sym = new Symbol(binding);
                            parameterPatterns.Add(new HIRIdentifierPattern(sym));
                            superArgs.Add(new HIRVariableExpression(sym));
                        }
                    }

                    // Derived constructors must call super() before accessing `this`.
                    // For implicit constructors in derived classes, insert an implicit super() call.
                    if (isDerivedConstructor)
                    {
                        ctorStatements.Add(new HIRExpressionStatement(
                            new HIRCallExpression(new HIRSuperExpression(), superArgs.ToArray())));
                    }

                    // Store scopes array to this._scopes if constructor has scopes parameter.
                    if (hasScopesParameter)
                    {
                        ctorStatements.Add(new HIRStoreUserClassInstanceFieldStatement
                        {
                            RegistryClassName = registryClassName,
                            FieldName = "_scopes",
                            IsPrivateField = true,
                            Value = new HIRScopesArrayExpression(),
                            Location = SourceLocation.FromNode(classBody)
                        });
                    }

                    // Instance field initializers
                    foreach (var element in enclosingClassDecl.Body.Body.OfType<PropertyDefinition>())
                    {
                        if (element.Static || element.Value == null) continue;

                        // Parse initializer expression in the class scope context.
                        var initBuilder = new HIRMethodBuilder(enclosingClassScope);
                        if (!initBuilder.TryParseExpressionForPrologue((Expression)element.Value, out var initExpr) || initExpr == null)
                        {
                            method = null!;
                            return false;
                        }

                        if (element.Key is PrivateIdentifier priv)
                        {
                            ctorStatements.Add(new HIRStoreUserClassInstanceFieldStatement
                            {
                                RegistryClassName = registryClassName,
                                FieldName = priv.Name,
                                IsPrivateField = true,
                                Value = initExpr,
                                Location = SourceLocation.FromNode(element)
                            });
                        }
                        else if (element.Key is Identifier pid)
                        {
                            ctorStatements.Add(new HIRStoreUserClassInstanceFieldStatement
                            {
                                RegistryClassName = registryClassName,
                                FieldName = pid.Name,
                                IsPrivateField = false,
                                Value = initExpr,
                                Location = SourceLocation.FromNode(element)
                            });
                        }
                    }

                    // Empty user body.
                    method = new HIRMethod
                    {
                        Parameters = parameterPatterns,
                        Body = new HIRBlock(ctorStatements)
                    };

                    return true;
                }

            case Acornima.Ast.MethodDefinition classMethodDef:
                var methodFuncExpr = classMethodDef.Value as FunctionExpression;                
                var methodBuilder = new HIRMethodBuilder(scope);
                if (!methodBuilder.TryParseParameters(methodFuncExpr.Params, out var methodParams))
                {
                    method = null!;
                    return false;
                }
                return methodBuilder.TryParseStatements(methodFuncExpr.Body.Body, methodParams, out method);
            case Acornima.Ast.ArrowFunctionExpression arrowFunc:
                // IR pipeline supports identifier params, simple defaults, and destructuring patterns.
                // Rest parameters (top-level RestElement) are not supported.
                if (!ParamsSupportedForIR(arrowFunc.Params))
                {
                    method = null!;
                    return false;
                }
                var arrowBuilder = new HIRMethodBuilder(scope);
                if (!arrowBuilder.TryParseParameters(arrowFunc.Params, out var arrowParams))
                {
                    method = null!;
                    return false;
                }

                // PL3.7a: concise-body arrows wrap implicit return
                if (arrowFunc.Body is BlockStatement arrowBlock)
                {
                    return arrowBuilder.TryParseStatements(arrowBlock.Body, arrowParams, out method);
                }

                if (arrowFunc.Body is Expression conciseExpr)
                {
                    return arrowBuilder.TryParseConciseBodyExpression(conciseExpr, arrowParams, out method);
                }

                method = null!;
                return false;
            case Acornima.Ast.FunctionExpression funcExpr:
                // FunctionExpression is used for class constructors and method values
                // IR pipeline supports identifier params, simple defaults, and destructuring patterns.
                // Rest parameters (top-level RestElement) are not supported.
                if (!ParamsSupportedForIR(funcExpr.Params))
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
                if (!funcExprBuilder.TryParseParameters(funcExpr.Params, out var funcParams))
                {
                    method = null!;
                    return false;
                }

                // If this is a class constructor, prepend implicit initializations.
                if (callableKind == ScopesCallableKind.Constructor)
                {
                    if (!TryGetEnclosingClass(scope, out var enclosingClassScope, out var enclosingClassDecl))
                    {
                        method = null!;
                        return false;
                    }

                    var registryClassName = GetRegistryClassName(enclosingClassScope);
                    var isDerivedConstructor = enclosingClassDecl.SuperClass != null;

                    var initStatements = new List<HIRStatement>();

                    if (hasScopesParameter)
                    {
                        initStatements.Add(new HIRStoreUserClassInstanceFieldStatement
                        {
                            RegistryClassName = registryClassName,
                            FieldName = "_scopes",
                            IsPrivateField = true,
                            Value = new HIRScopesArrayExpression(),
                            Location = SourceLocation.FromNode(funcExpr)
                        });
                    }

                    foreach (var element in enclosingClassDecl.Body.Body.OfType<PropertyDefinition>())
                    {
                        if (element.Static || element.Value == null) continue;

                        var initBuilder = new HIRMethodBuilder(enclosingClassScope);
                        if (!initBuilder.TryParseExpressionForPrologue((Expression)element.Value, out var initExpr) || initExpr == null)
                        {
                            method = null!;
                            return false;
                        }

                        if (element.Key is PrivateIdentifier priv)
                        {
                            initStatements.Add(new HIRStoreUserClassInstanceFieldStatement
                            {
                                RegistryClassName = registryClassName,
                                FieldName = priv.Name,
                                IsPrivateField = true,
                                Value = initExpr,
                                Location = SourceLocation.FromNode(element)
                            });
                        }
                        else if (element.Key is Identifier pid)
                        {
                            initStatements.Add(new HIRStoreUserClassInstanceFieldStatement
                            {
                                RegistryClassName = registryClassName,
                                FieldName = pid.Name,
                                IsPrivateField = false,
                                Value = initExpr,
                                Location = SourceLocation.FromNode(element)
                            });
                        }
                    }

                    if (!funcExprBuilder.TryParseStatementsToList(funcBlock.Body, out var bodyStatements))
                    {
                        method = null!;
                        return false;
                    }

                    if (isDerivedConstructor)
                    {
                        // Insert initializers after the first direct super(...) call.
                        var superCallIndex = bodyStatements.FindIndex(s =>
                            s is HIRExpressionStatement es
                            && es.Expression is HIRCallExpression ce
                            && ce.Callee is HIRSuperExpression);

                        if (superCallIndex < 0)
                        {
                            method = null!;
                            return false;
                        }

                        bodyStatements.InsertRange(superCallIndex + 1, initStatements);
                    }
                    else
                    {
                        bodyStatements.InsertRange(0, initStatements);
                    }

                    method = new HIRMethod
                    {
                        Parameters = funcParams,
                        Body = new HIRBlock(bodyStatements)
                    };

                    return true;
                }

                return funcExprBuilder.TryParseStatements(funcBlock.Body, funcParams, out method);

            case Acornima.Ast.FunctionDeclaration funcDecl:
                // IR pipeline supports identifier params, simple defaults, and destructuring patterns.
                // Rest parameters (top-level RestElement) are not supported.
                if (!ParamsSupportedForIR(funcDecl.Params))
                {
                    method = null!;
                    return false;
                }
                if (funcDecl.Body is not BlockStatement declBlock)
                {
                    method = null!;
                    return false;
                }
                var funcDeclBuilder = new HIRMethodBuilder(scope);
                if (!funcDeclBuilder.TryParseParameters(funcDecl.Params, out var declParams))
                {
                    method = null!;
                    return false;
                }
                return funcDeclBuilder.TryParseStatements(declBlock.Body, declParams, out method);
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

    /// <summary>
    /// Returns true if parameters are supported by the IR pipeline for function expressions/arrow functions.
    /// Supports: Identifier, AssignmentPattern with Identifier left-hand side, ObjectPattern, ArrayPattern.
    /// Does not support: top-level RestElement parameters.
    /// </summary>
    internal static bool ParamsSupportedForIR(in NodeList<Node> parameters)
    {
        return parameters.All(param => param switch
        {
            Acornima.Ast.Identifier => true,
            Acornima.Ast.AssignmentPattern ap => ap.Left is Acornima.Ast.Identifier,
            Acornima.Ast.ObjectPattern => true,
            Acornima.Ast.ArrayPattern => true,
            Acornima.Ast.RestElement => false,
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

    public void AddPrologueStatement([In, NotNull] HIRStatement statement)
    {
        ArgumentNullException.ThrowIfNull(statement, nameof(statement));
        _statements.Add(statement);
    }

    public bool TryParseStatementsToList([In, NotNull] IEnumerable<Acornima.Ast.Statement> statements, out List<HIRStatement> hirStatements)
    {
        hirStatements = new List<HIRStatement>(_statements.Count + 16);
        hirStatements.AddRange(_statements);

        var documentId = GetCurrentDocumentId();

        foreach (var statement in statements)
        {
            hirStatements.Add(new HIRSequencePointStatement
            {
                Span = SourceSpan.FromNode(statement, documentId)
            });

            if (!TryParseStatement(statement, out var hirStatement))
            {
                return false;
            }
            hirStatements.Add(hirStatement!);
        }

        return true;
    }

    private string GetCurrentDocumentId()
    {
        // Currently the best stable identifier available throughout the pipeline is the module id
        // (global scope name). PDB generation can later map this to an actual file path.
        var scope = _currentScope;
        while (scope.Parent != null)
        {
            scope = scope.Parent;
        }

        return scope.Name;
    }

    public bool TryParseExpressionForPrologue([In, NotNull] Acornima.Ast.Expression expression, out HIRExpression? hirExpression)
    {
        return TryParseExpression(expression, out hirExpression);
    }

    public bool TryParseStatements([In, NotNull] IEnumerable<Acornima.Ast.Statement> statements, IReadOnlyList<HIRPattern> parameters, out HIRMethod? method)
    {
        method = null;

        if (!TryParseStatementsToList(statements, out var hirStatements))
        {
            return false;
        }

        method = new HIRMethod
        {
            Parameters = parameters,
            Body = new HIRBlock(hirStatements)
        };

        return true;
    }

    public bool TryParseConciseBodyExpression([In, NotNull] Acornima.Ast.Expression expression, IReadOnlyList<HIRPattern> parameters, out HIRMethod? method)
    {
        method = null;

        if (!TryParseExpression(expression, out var hirExpr))
        {
            return false;
        }

        method = new HIRMethod
        {
            Parameters = parameters,
            Body = new HIRBlock(new List<HIRStatement> { new HIRReturnStatement(hirExpr) })
        };

        return true;
    }

    public bool TryParseParameters(in NodeList<Node> parameters, [NotNullWhen(true)] out IReadOnlyList<HIRPattern>? hirParameters)
    {
        hirParameters = null;

        var patterns = new List<HIRPattern>(parameters.Count);
        foreach (var param in parameters)
        {
            if (!TryParsePattern(param, out var pattern) || pattern == null)
            {
                return false;
            }
            patterns.Add(pattern);
        }

        hirParameters = patterns;
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
                // Function declarations are hoisted and compiled separately by the two-phase IR pipeline.
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
                    // For loops with `let/const` loop-head bindings create a dedicated block-like scope.
                    // SymbolTableBuilder models this scope as a child whose AstNode is the init VariableDeclaration,
                    // not the ForStatement itself.
                    Scope? forScope = null;
                    if (forStmt.Init is VariableDeclaration forInitDecl
                        && (forInitDecl.Kind == VariableDeclarationKind.Let || forInitDecl.Kind == VariableDeclarationKind.Const))
                    {
                        forScope = FindChildScopeForAstNode(forInitDecl);
                    }

                    forScope ??= FindChildScopeForAstNode(forStmt);
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
                    var previousForOfScope = _currentScope;

                    static BindingKind MapDeclarationKind(VariableDeclarationKind kind) => kind switch
                    {
                        VariableDeclarationKind.Var => BindingKind.Var,
                        VariableDeclarationKind.Let => BindingKind.Let,
                        VariableDeclarationKind.Const => BindingKind.Const,
                        _ => BindingKind.Var
                    };

                    static void CollectBindings(HIRPattern pattern, List<BindingInfo> bindings)
                    {
                        switch (pattern)
                        {
                            case HIRIdentifierPattern id:
                                bindings.Add(id.Symbol.BindingInfo);
                                break;

                            case HIRDefaultPattern def:
                                CollectBindings(def.Target, bindings);
                                break;

                            case HIRRestPattern rest:
                                CollectBindings(rest.Target, bindings);
                                break;

                            case HIRObjectPattern obj:
                                foreach (var prop in obj.Properties)
                                {
                                    CollectBindings(prop.Value, bindings);
                                }
                                if (obj.Rest != null)
                                {
                                    CollectBindings(obj.Rest, bindings);
                                }
                                break;

                            case HIRArrayPattern arr:
                                foreach (var el in arr.Elements.Where(el => el != null))
                                {
                                    CollectBindings(el!, bindings);
                                }
                                if (arr.Rest != null)
                                {
                                    CollectBindings(arr.Rest, bindings);
                                }
                                break;
                        }
                    }

                    // Determine whether the left-hand side is a declaration and whether it has a
                    // dedicated loop-head scope (for let/const).
                    bool isDeclaration = false;
                    var declarationKind = BindingKind.Var;
                    Scope? forScope = null;
                    Node? declIdNode = null;

                    if (forOfStmt.Left is VariableDeclaration forOfDecl && forOfDecl.Declarations.Count == 1)
                    {
                        isDeclaration = true;
                        declarationKind = MapDeclarationKind(forOfDecl.Kind);
                        declIdNode = forOfDecl.Declarations[0].Id;

                        if (forOfDecl.Kind == VariableDeclarationKind.Let || forOfDecl.Kind == VariableDeclarationKind.Const)
                        {
                            forScope = FindChildScopeForAstNode(forOfDecl);
                        }
                    }

                    // Parse RHS iterable expression in the outer scope (matches current runtime semantics).
                    _currentScope = previousForOfScope;
                    if (!TryParseExpression(forOfStmt.Right, out var iterableExpr))
                    {
                        _currentScope = previousForOfScope;
                        return false;
                    }

                    // Parse target + body in the loop scope when present.
                    _currentScope = forScope ?? previousForOfScope;

                    HIRPattern? targetPattern = null;
                    var loopHeadBindings = new List<BindingInfo>();

                    if (isDeclaration)
                    {
                        if (declIdNode == null)
                        {
                            _currentScope = previousForOfScope;
                            return false;
                        }

                        if (!TryParsePattern(declIdNode, out targetPattern))
                        {
                            _currentScope = previousForOfScope;
                            return false;
                        }

                        CollectBindings(targetPattern!, loopHeadBindings);
                    }
                    else if (forOfStmt.Left is Identifier id)
                    {
                        var symbol = _currentScope.FindSymbol(id.Name);
                        if (symbol == null)
                        {
                            _currentScope = previousForOfScope;
                            return false;
                        }
                        targetPattern = new HIRIdentifierPattern(symbol);
                    }
                    else
                    {
                        _currentScope = previousForOfScope;
                        return false;
                    }

                    if (!TryParseStatement(forOfStmt.Body, out var bodyStmt))
                    {
                        _currentScope = previousForOfScope;
                        return false;
                    }

                    _currentScope = previousForOfScope;
                    hirStatement = new HIRForOfStatement(targetPattern!, isDeclaration, declarationKind, forOfStmt.Await, loopHeadBindings, iterableExpr!, bodyStmt!);
                    return true;
                }

            case ForInStatement forInStmt:
                {
                    var previousForInScope = _currentScope;

                    static BindingKind MapDeclarationKind(VariableDeclarationKind kind) => kind switch
                    {
                        VariableDeclarationKind.Var => BindingKind.Var,
                        VariableDeclarationKind.Let => BindingKind.Let,
                        VariableDeclarationKind.Const => BindingKind.Const,
                        _ => BindingKind.Var
                    };

                    static void CollectBindings(HIRPattern pattern, List<BindingInfo> bindings)
                    {
                        switch (pattern)
                        {
                            case HIRIdentifierPattern id:
                                bindings.Add(id.Symbol.BindingInfo);
                                break;

                            case HIRDefaultPattern def:
                                CollectBindings(def.Target, bindings);
                                break;

                            case HIRRestPattern rest:
                                CollectBindings(rest.Target, bindings);
                                break;

                            case HIRObjectPattern obj:
                                foreach (var prop in obj.Properties)
                                {
                                    CollectBindings(prop.Value, bindings);
                                }
                                if (obj.Rest != null)
                                {
                                    CollectBindings(obj.Rest, bindings);
                                }
                                break;

                            case HIRArrayPattern arr:
                                foreach (var el in arr.Elements.Where(el => el != null))
                                {
                                    CollectBindings(el!, bindings);
                                }
                                if (arr.Rest != null)
                                {
                                    CollectBindings(arr.Rest, bindings);
                                }
                                break;
                        }
                    }

                    bool isDeclaration = false;
                    var declarationKind = BindingKind.Var;
                    Scope? forScope = null;
                    Node? declIdNode = null;

                    if (forInStmt.Left is VariableDeclaration forInDecl && forInDecl.Declarations.Count == 1)
                    {
                        isDeclaration = true;
                        declarationKind = MapDeclarationKind(forInDecl.Kind);
                        declIdNode = forInDecl.Declarations[0].Id;

                        if (forInDecl.Kind == VariableDeclarationKind.Let || forInDecl.Kind == VariableDeclarationKind.Const)
                        {
                            forScope = FindChildScopeForAstNode(forInDecl);
                        }
                    }

                    // Parse RHS enumerable expression in the outer scope (matches current runtime semantics).
                    _currentScope = previousForInScope;
                    if (!TryParseExpression(forInStmt.Right, out var enumerableExpr))
                    {
                        _currentScope = previousForInScope;
                        return false;
                    }

                    // Parse target + body in the loop scope when present.
                    _currentScope = forScope ?? previousForInScope;

                    HIRPattern? targetPattern = null;
                    var loopHeadBindings = new List<BindingInfo>();

                    if (isDeclaration)
                    {
                        if (declIdNode == null)
                        {
                            _currentScope = previousForInScope;
                            return false;
                        }

                        if (!TryParsePattern(declIdNode, out targetPattern))
                        {
                            _currentScope = previousForInScope;
                            return false;
                        }

                        CollectBindings(targetPattern!, loopHeadBindings);
                    }
                    else if (forInStmt.Left is Identifier id)
                    {
                        var symbol = _currentScope.FindSymbol(id.Name);
                        if (symbol == null)
                        {
                            _currentScope = previousForInScope;
                            return false;
                        }
                        targetPattern = new HIRIdentifierPattern(symbol);
                    }
                    else
                    {
                        _currentScope = previousForInScope;
                        return false;
                    }

                    if (!TryParseStatement(forInStmt.Body, out var bodyStmt))
                    {
                        _currentScope = previousForInScope;
                        return false;
                    }

                    _currentScope = previousForInScope;
                    hirStatement = new HIRForInStatement(targetPattern!, isDeclaration, declarationKind, loopHeadBindings, enumerableExpr!, bodyStmt!);
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
                        HIRForOfStatement forOfStmt => new HIRForOfStatement(forOfStmt.Target, forOfStmt.IsDeclaration, forOfStmt.DeclarationKind, forOfStmt.IsAwait, forOfStmt.LoopHeadBindings, forOfStmt.Iterable, forOfStmt.Body, labelName),
                        HIRForInStatement forInStmt => new HIRForInStatement(forInStmt.Target, forInStmt.IsDeclaration, forInStmt.DeclarationKind, forInStmt.LoopHeadBindings, forInStmt.Enumerable, forInStmt.Body, labelName),
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

    private static bool TryGetEnclosingClassScope(Scope startingScope, [NotNullWhen(true)] out Scope? classScope)
    {
        classScope = null;

        var current = startingScope;
        while (current != null)
        {
            if (current.Kind == ScopeKind.Class)
            {
                classScope = current;
                return true;
            }
            current = current.Parent;
        }

        return false;
    }

    private static string GetRegistryClassName(Scope classScope)
    {
        var ns = classScope.DotNetNamespace ?? "Classes";
        var name = classScope.DotNetTypeName ?? classScope.Name;
        return $"{ns}.{name}";
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

        // PL4.1: Variable declarator destructuring (object/array patterns, including nested defaults/rest)
        if (decl.Id is Acornima.Ast.ObjectPattern or Acornima.Ast.ArrayPattern)
        {
            if (decl.Init == null)
            {
                return false;
            }

            if (!TryParseExpression(decl.Init, out var hirInitExpr))
            {
                return false;
            }

            if (!TryParsePattern(decl.Id, out var hirPattern))
            {
                return false;
            }

            hirStatement = new HIRDestructuringVariableDeclaration(hirPattern!, hirInitExpr!);
            return true;
        }

        return false;
    }

    private bool TryParsePattern(Acornima.Ast.Node node, out HIRPattern? pattern)
    {
        pattern = null;

        switch (node)
        {
            case Acornima.Ast.Identifier id:
                pattern = new HIRIdentifierPattern(_currentScope.FindSymbol(id.Name));
                return true;

            case Acornima.Ast.AssignmentPattern ap:
                if (!TryParsePattern(ap.Left, out var targetPattern))
                {
                    return false;
                }
                if (!TryParseExpression(ap.Right, out var defaultExpr))
                {
                    return false;
                }
                pattern = new HIRDefaultPattern(targetPattern!, defaultExpr!);
                return true;

            case Acornima.Ast.RestElement rest:
                if (!TryParsePattern(rest.Argument, out var restTargetPattern))
                {
                    return false;
                }
                pattern = new HIRRestPattern(restTargetPattern!);
                return true;

            case Acornima.Ast.ObjectPattern objPat:
                {
                    var props = new List<HIRObjectPatternProperty>();
                    HIRRestPattern? restPattern = null;

                    foreach (var propNode in objPat.Properties)
                    {
                        if (propNode is Acornima.Ast.Property p)
                        {
                            string? key = p.Key switch
                            {
                                Acornima.Ast.Identifier kid => kid.Name,
                                Acornima.Ast.StringLiteral sl => sl.Value,
                                Acornima.Ast.NumericLiteral nl => nl.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                Acornima.Ast.Literal lit when lit.Value is string s => s,
                                Acornima.Ast.Literal lit when lit.Value is double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                _ => null
                            };

                            if (string.IsNullOrEmpty(key))
                            {
                                return false;
                            }

                            if (!TryParsePattern(p.Value, out var valuePattern))
                            {
                                return false;
                            }

                            props.Add(new HIRObjectPatternProperty(key!, valuePattern!));
                            continue;
                        }

                        if (propNode is Acornima.Ast.RestElement re)
                        {
                            if (!TryParsePattern(re.Argument, out var restTargetPattern2))
                            {
                                return false;
                            }

                            restPattern = new HIRRestPattern(restTargetPattern2!);
                            continue;
                        }

                        return false;
                    }

                    pattern = new HIRObjectPattern(props, restPattern);
                    return true;
                }

            case Acornima.Ast.ArrayPattern arrPat:
                {
                    var elements = new List<HIRPattern?>();
                    HIRRestPattern? restPattern = null;

                    foreach (var el in arrPat.Elements)
                    {
                        if (el == null)
                        {
                            elements.Add(null);
                            continue;
                        }

                        if (el is Acornima.Ast.RestElement re)
                        {
                            if (!TryParsePattern(re.Argument, out var restTargetPattern3))
                            {
                                return false;
                            }
                            restPattern = new HIRRestPattern(restTargetPattern3!);
                            continue;
                        }

                        if (!TryParsePattern(el, out var elementPattern))
                        {
                            return false;
                        }
                        elements.Add(elementPattern);
                    }

                    pattern = new HIRArrayPattern(elements, restPattern);
                    return true;
                }

            default:
                return false;
        }
    }

    private bool TryParseExpression(Acornima.Ast.Expression? expr, out HIRExpression? hirExpr)
    {
        hirExpr = null;

        switch (expr)
        {
            case ChainExpression chainExpr:
                // Optional chaining is represented by ChainExpression wrapping a chain element
                // (MemberExpression/CallExpression) with Optional=true.
                return TryParseExpression(chainExpr.Expression, out hirExpr);

            case AwaitExpression awaitExpr:
                {
                    if (!TryParseExpression(awaitExpr.Argument, out var awaitedArg) || awaitedArg == null)
                    {
                        return false;
                    }

                    hirExpr = new HIRAwaitExpression(awaitedArg);
                    return true;
                }

            case YieldExpression yieldExpr:
                {
                    // yield may omit an argument (yield;), which yields undefined.
                    HIRExpression? yieldedArg = null;
                    if (yieldExpr.Argument != null)
                    {
                        if (!TryParseExpression(yieldExpr.Argument, out var parsed) || parsed == null)
                        {
                            return false;
                        }
                        yieldedArg = parsed;
                    }

                    hirExpr = new HIRYieldExpression(yieldedArg, yieldExpr.Delegate);
                    return true;
                }

            case ThisExpression:
                // PL3.5: ThisExpression support.
                // Support 'this' in function scopes, including arrow functions.
                // Non-arrow functions get their dynamic 'this' from the runtime call sites.
                // Arrow functions get lexical 'this' via binding at closure creation time.
                var allowsThis = _rootScope.Parent?.Kind == ScopeKind.Class
                    || _rootScope.AstNode is FunctionExpression
                    || _rootScope.AstNode is FunctionDeclaration
                    || _rootScope.AstNode is ArrowFunctionExpression;
                if (!allowsThis)
                {
                    return false;
                }
                hirExpr = new HIRThisExpression();
                return true;

            case Super:
                hirExpr = new HIRSuperExpression();
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

                if (callExpr.Optional)
                {
                    hirExpr = new HIROptionalCallExpression(calleeExpr!, argExprs);
                    return true;
                }

                hirExpr = new HIRCallExpression(calleeExpr!, argExprs);
                return true;

            case NewExpression newExpr:
                // PL3.3: NewExpression support in IR pipeline.
                // - PL3.3a: built-in Error types
                // - PL3.3b: user-defined classes
                // - PL3.3c: dynamic/new-on-value (e.g., const C = require('...'); new C(...))
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
                    var userArgs = new List<HIRExpression>();
                    foreach (var arg in newExpr.Arguments)
                    {
                        if (!TryParseExpression(arg, out var argHirExpr))
                        {
                            return false;
                        }
                        userArgs.Add(argHirExpr!);
                    }

                    // If this is a user-defined class declaration, lowering will take the direct class ctor fast-path.
                    // Otherwise, lowering will fall back to dynamic construction (runtime ConstructValue).

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
                if (!TryParseExpression(assignExpr.Right, out var assignValueExpr))
                {
                    return false;
                }

                // Identifier assignment: x = expr / x += expr
                if (assignExpr.Left is Identifier assignTargetId)
                {
                    var targetSymbol = _currentScope.FindSymbol(assignTargetId.Name);
                    hirExpr = new HIRAssignmentExpression(targetSymbol, assignExpr.Operator, assignValueExpr!);
                    return true;
                }

                // PL4.2a/b: Member assignments (obj.prop = value, obj[index] = value)
                if (assignExpr.Left is MemberExpression memberTarget)
                {
                    if (!TryParseExpression(memberTarget.Object, out var memberObjectExpr))
                    {
                        return false;
                    }

                    if (memberTarget.Computed)
                    {
                        if (!TryParseExpression(memberTarget.Property, out var memberIndexExpr))
                        {
                            return false;
                        }

                        hirExpr = new HIRIndexAssignmentExpression(memberObjectExpr!, memberIndexExpr!, assignExpr.Operator, assignValueExpr!);
                        return true;
                    }

                    if (memberTarget.Property is not Identifier memberPropId)
                    {
                        return false;
                    }

                    hirExpr = new HIRPropertyAssignmentExpression(memberObjectExpr!, memberPropId.Name, assignExpr.Operator, assignValueExpr!);
                    return true;
                }

                // PL4.2c: Destructuring assignment (only simple '=' supported)
                if (assignExpr.Operator != Acornima.Operator.Assignment)
                {
                    return false;
                }

                if (assignExpr.Left is Acornima.Ast.ObjectPattern or Acornima.Ast.ArrayPattern)
                {
                    if (!TryParsePattern(assignExpr.Left, out var hirPattern))
                    {
                        return false;
                    }

                    hirExpr = new HIRDestructuringAssignmentExpression(hirPattern!, assignValueExpr!);
                    return true;
                }

                return false;

            case MemberExpression memberExpr:
                // Handle member expressions
                HIRExpression? objectExpr;

                // Private instance field access: this.#name
                if (!memberExpr.Computed && memberExpr.Object is ThisExpression && memberExpr.Property is Acornima.Ast.PrivateIdentifier ppid)
                {
                    if (!TryGetEnclosingClassScope(_currentScope, out var classScope))
                    {
                        return false;
                    }

                    hirExpr = new HIRLoadUserClassInstanceFieldExpression
                    {
                        RegistryClassName = GetRegistryClassName(classScope),
                        FieldName = ppid.Name,
                        IsPrivateField = true
                    };
                    return true;
                }

                if (!TryParseExpression(memberExpr.Object, out objectExpr))
                {
                    return false;
                }

                // Optional chaining: obj?.prop / obj?.[expr]
                if (memberExpr.Optional)
                {
                    if (memberExpr.Computed)
                    {
                        if (!TryParseExpression(memberExpr.Property, out var optionalIndexExpr))
                        {
                            return false;
                        }

                        hirExpr = new HIROptionalIndexAccessExpression(objectExpr!, optionalIndexExpr!);
                        return true;
                    }

                    if (memberExpr.Property is not Identifier optionalPropertyIdentifier)
                    {
                        return false;
                    }

                    hirExpr = new HIROptionalPropertyAccessExpression(objectExpr!, optionalPropertyIdentifier.Name);
                    return true;
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

            case ClassExpression classExpr:
                {
                    // ClassExpression used as a value (e.g., module.exports = class Foo {...}).
                    // The class is emitted as a CLR type; the expression value is the System.Type.
                    var classExprScope = FindChildScopeForAstNode(classExpr);
                    if (classExprScope == null)
                    {
                        return false;
                    }

                    var registryClassName = $"{(classExprScope.DotNetNamespace ?? "Classes")}.{(classExprScope.DotNetTypeName ?? classExprScope.Name)}";
                    hirExpr = new HIRUserClassTypeExpression(registryClassName);
                    return true;
                }

            case ArrowFunctionExpression arrowExpr:
                // PL3.7: ArrowFunctionExpression as an expression (closure creation)
                // We treat the arrow as an opaque callable value; its body is compiled separately.
                // Support matches current IR pipeline: identifiers, simple defaults, and destructuring patterns.
                // Rest parameters are not supported.
                if (!HIRBuilder.ParamsSupportedForIR(arrowExpr.Params))
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
                // Support matches current IR pipeline: identifiers, simple defaults, and destructuring patterns.
                // Rest parameters are not supported.
                if (!HIRBuilder.ParamsSupportedForIR(funcExpr.Params))
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
            case Literal regexLiteral when regexLiteral.Raw != null && regexLiteral.Raw.TrimStart().StartsWith("/"):
                // Regex literal like /pattern/flags.
                // NOTE: Acornima 1.1.1 does not expose parsed pattern/flags on Literal,
                // so we extract from Literal.Value when possible and otherwise parse Literal.Raw.
                if (TryExtractRegexLiteral(regexLiteral, out var pattern, out var flags))
                {
                    var regExpSymbol = _currentScope.FindSymbol("RegExp");
                    hirExpr = new HIRNewExpression(
                        new HIRVariableExpression(regExpSymbol),
                        new List<HIRExpression>
                        {
                            new HIRLiteralExpression(JavascriptType.String, pattern),
                            new HIRLiteralExpression(JavascriptType.String, flags)
                        });
                    return true;
                }
                return false;
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
                var objectMembers = new List<HIRObjectMember>();
                foreach (var property in objExpr.Properties)
                {
                    if (property is SpreadElement spread)
                    {
                        // Spread member: { ...expr }
                        if (!TryParseExpression(spread.Argument, out var spreadArgHir))
                        {
                            return false;
                        }

                        objectMembers.Add(new HIRObjectSpreadProperty(spreadArgHir!));
                        continue;
                    }

                    if (property is MethodDefinition methodDef)
                    {
                        // Method definition member: { m() { ... } }
                        // Represented as a property whose value is a function expression.
                        if (methodDef.Kind != PropertyKind.Init)
                        {
                            // getters/setters are not supported
                            return false;
                        }

                        if (methodDef.Value is not Acornima.Ast.Expression methodValueExpression)
                        {
                            return false;
                        }

                        if (!TryParseExpression(methodValueExpression, out var methodValueHir))
                        {
                            return false;
                        }

                        if (methodDef.Computed)
                        {
                            if (methodDef.Key is not Acornima.Ast.Expression methodKeyExpression)
                            {
                                return false;
                            }
                            if (!TryParseExpression(methodKeyExpression, out var keyExprHir))
                            {
                                return false;
                            }
                            objectMembers.Add(new HIRObjectComputedProperty(keyExprHir!, methodValueHir!));
                        }
                        else
                        {
                            string? methodName = null;
                            if (methodDef.Key is Identifier methodKeyId)
                            {
                                methodName = methodKeyId.Name;
                            }
                            else if (methodDef.Key is StringLiteral strLit)
                            {
                                methodName = strLit.Value;
                            }
                            else if (methodDef.Key is NumericLiteral numLit)
                            {
                                methodName = numLit.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                return false;
                            }

                            objectMembers.Add(new HIRObjectProperty(methodName!, methodValueHir!));
                        }

                        continue;
                    }

                    if (property is not ObjectProperty objProp)
                    {
                        // Unhandled object member type
                        return false;
                    }

                    if (objProp.Kind != PropertyKind.Init)
                    {
                        // getters/setters are not supported
                        return false;
                    }

                    // Computed property: { [expr]: value }
                    if (objProp.Computed)
                    {
                        if (objProp.Key is not Acornima.Ast.Expression keyExpression)
                        {
                            return false;
                        }
                        if (!TryParseExpression(keyExpression, out var keyExprHir))
                        {
                            return false;
                        }

                        if (objProp.Value is not Acornima.Ast.Expression computedValueExpression)
                        {
                            return false;
                        }
                        if (!TryParseExpression(computedValueExpression, out var computedValueExpr))
                        {
                            return false;
                        }

                        objectMembers.Add(new HIRObjectComputedProperty(keyExprHir!, computedValueExpr!));
                        continue;
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

                    objectMembers.Add(new HIRObjectProperty(propName!, valueExpr!));
                }
                hirExpr = new HIRObjectExpression(objectMembers);
                return true;

            // Handle other expression types as needed
            default:
                return false;
        }
    }

    private static bool TryExtractRegexLiteral(Literal literal, out string pattern, out string flags)
    {
        pattern = string.Empty;
        flags = string.Empty;

        // Prefer structured data if Acornima provides it via Literal.Value (implementation detail).
        // We use reflection to avoid hard dependency on internal Acornima types.
        if (literal.Value != null)
        {
            var valueType = literal.Value.GetType();
            var patternProp = valueType.GetProperty("Pattern") ?? valueType.GetProperty("pattern");
            var flagsProp = valueType.GetProperty("Flags") ?? valueType.GetProperty("flags");
            if (patternProp != null && flagsProp != null)
            {
                var p = patternProp.GetValue(literal.Value) as string;
                var f = flagsProp.GetValue(literal.Value) as string;
                if (p != null && f != null)
                {
                    pattern = p;
                    flags = f;
                    return true;
                }
            }
        }

        // Acornima represents regex literals as Literal with Raw like "/b/g".
        var raw = literal.Raw;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }
        raw = raw.Trim();
        if (raw.Length < 2 || raw[0] != '/')
        {
            return false;
        }

        // Find the final unescaped '/'
        int lastSlash = -1;
        bool escaped = false;
        for (int i = 1; i < raw.Length; i++)
        {
            char c = raw[i];
            if (!escaped && c == '/')
            {
                lastSlash = i;
            }
            escaped = (!escaped && c == '\\');
        }

        if (lastSlash <= 0)
        {
            return false;
        }

        pattern = raw.Substring(1, lastSlash - 1);
        flags = raw.Substring(lastSlash + 1);
        return true;
    }
    
    /// <summary>
    /// Finds the child scope that corresponds to the given AST node.
    /// Returns null if no matching child scope exists.
    /// </summary>
    private Scope? FindChildScopeForAstNode(Node astNode)
    {
        // Primary: exact AST node identity match (fast path when the HIR is built from
        // the same AST instance that was used to build the SymbolTable).
        var byReference = _currentScope.Children.FirstOrDefault(child => ReferenceEquals(child.AstNode, astNode));
        if (byReference != null)
        {
            return byReference;
        }

        // Fallback: match block scopes by deterministic location-based name.
        // This makes scope resolution resilient if AST nodes are not reference-identical
        // across phases (e.g., if the AST is reparsed).
        if (astNode is BlockStatement blockStmt)
        {
            var blockName = $"Block_L{blockStmt.Location.Start.Line}C{blockStmt.Location.Start.Column}";
            return _currentScope.Children.FirstOrDefault(child => child.Kind == ScopeKind.Block && child.Name == blockName);
        }

        // Fallback for `for (let/const ...)` loop-head scopes.
        // SymbolTableBuilder creates these with AstNode = init VariableDeclaration and name `For_L{line}C{col}`.
        if (astNode is VariableDeclaration decl
            && (decl.Kind == VariableDeclarationKind.Let || decl.Kind == VariableDeclarationKind.Const))
        {
            var loc = decl.Location.Start;
            var forName = $"For_L{loc.Line}C{loc.Column}";
            return _currentScope.Children.FirstOrDefault(child => child.Kind == ScopeKind.Block && child.Name == forName);
        }

        return null;
    }
}