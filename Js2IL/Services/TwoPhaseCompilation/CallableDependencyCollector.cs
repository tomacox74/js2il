using Acornima.Ast;
using Js2IL.SymbolTables;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Milestone 2b: AST-based dependency discovery.
///
/// Scope (by design):
/// - Record edges when targets are identifier-resolvable via the symbol table.
/// - Record edges for inline anonymous callables (arrow/function expression nodes) without descending into their bodies.
/// - Record edges for <c>new C()</c> only when <c>C</c> resolves to a class binding (constructor metadata).
/// - Do NOT attempt to add edges for member calls like <c>obj.m()</c> (deferred to Milestone 2b1).
/// </summary>
public sealed class CallableDependencyCollector
{
    private readonly SymbolTable _symbolTable;
    private readonly CallableRegistry _registry;
    private readonly IReadOnlyList<CallableId> _callablesInStableOrder;

    // Lookup for class constructor callables by (declaringScopeName, className)
    private readonly Dictionary<(string DeclaringScopeName, string ClassName), CallableId> _classConstructors;

    public CallableDependencyCollector(SymbolTable symbolTable, CallableRegistry registry, IReadOnlyList<CallableId> callablesInStableOrder)
    {
        _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _callablesInStableOrder = callablesInStableOrder ?? throw new ArgumentNullException(nameof(callablesInStableOrder));

        _classConstructors = new Dictionary<(string, string), CallableId>();
        foreach (var c in _callablesInStableOrder)
        {
            if (c.Kind == CallableKind.ClassConstructor && !string.IsNullOrEmpty(c.Name))
            {
                _classConstructors[(c.DeclaringScopeName, c.Name!)] = c;
            }
        }
    }

    public CallableDependencyGraph Collect()
    {
        var edges = new Dictionary<CallableId, IReadOnlyList<CallableId>>();

        foreach (var caller in _callablesInStableOrder)
        {
            var deps = CollectDependenciesForCallable(caller);
            edges[caller] = deps;
        }

        return new CallableDependencyGraph(_callablesInStableOrder, edges);
    }

    private IReadOnlyList<CallableId> CollectDependenciesForCallable(CallableId caller)
    {
        var callerAst = caller.AstNode;
        if (callerAst == null)
        {
            return Array.Empty<CallableId>();
        }

        var callerScope = ResolveScopeForCallable(caller);
        var deps = new HashSet<CallableId>();

        void AddDependency(CallableId callee)
        {
            if (!Equals(callee, caller))
            {
                deps.Add(callee);
            }
        }

        void TryAddFunctionBindingDependency(string identifierName)
        {
            if (callerScope == null)
            {
                return;
            }

            var symbol = callerScope.FindSymbol(identifierName);
            var binding = symbol.BindingInfo;
            if (binding.Kind != BindingKind.Function)
            {
                return;
            }

            if (_registry.TryGetCallableIdForAstNode(binding.DeclarationNode, out var calleeId))
            {
                AddDependency(calleeId);
            }
        }

        void TryAddClassConstructorDependency(Identifier classIdentifier)
        {
            if (callerScope == null)
            {
                return;
            }

            var symbol = callerScope.FindSymbol(classIdentifier.Name);
            var binding = symbol.BindingInfo;
            if (binding.DeclarationNode is not ClassDeclaration classDecl)
            {
                return;
            }

            var classScope = _symbolTable.FindScopeByAstNode(classDecl);
            if (classScope?.Parent == null)
            {
                return;
            }

            var declaringScopeName = GetDeclaringScopeName(classScope.Parent, _symbolTable.Root.Name);
            if (_classConstructors.TryGetValue((declaringScopeName, classIdentifier.Name), out var ctorId))
            {
                AddDependency(ctorId);
            }
        }

        void VisitNode(Node? n)
        {
            if (n == null) return;

            switch (n)
            {
                case FunctionDeclaration:
                    // Nested callable boundary: do not descend.
                    return;

                case FunctionExpression funcExpr:
                    if (_registry.TryGetCallableIdForAstNode(funcExpr, out var funcExprId))
                    {
                        AddDependency(funcExprId);
                    }
                    return; // Do not descend into body.

                case ArrowFunctionExpression arrowExpr:
                    if (_registry.TryGetCallableIdForAstNode(arrowExpr, out var arrowId))
                    {
                        AddDependency(arrowId);
                    }
                    return; // Do not descend into body.

                case ClassDeclaration:
                    // Nested callable boundary: do not descend into members.
                    return;

                case Identifier id:
                    // Identifier reference (in expression context; declaration positions are excluded by traversal rules below)
                    TryAddFunctionBindingDependency(id.Name);
                    return;

                case NewExpression ne:
                    if (ne.Callee is Identifier newId)
                    {
                        TryAddClassConstructorDependency(newId);
                    }
                    VisitNode(ne.Callee);
                    foreach (var arg in ne.Arguments)
                    {
                        VisitNode(arg as Node);
                    }
                    return;

                case MemberExpression me:
                    // Milestone 2b: do not treat non-computed property identifiers as variable references.
                    VisitNode(me.Object);
                    if (me.Computed)
                    {
                        VisitNode(me.Property);
                    }
                    return;

                case Property prop:
                    // Skip non-computed Identifier keys to avoid false "identifier reference" edges.
                    if (prop.Computed)
                    {
                        VisitNode(prop.Key);
                    }
                    VisitNode(prop.Value as Node);
                    return;

                case MethodDefinition:
                    // Nested callable boundary.
                    return;

                case VariableDeclarator vd:
                    // Don't treat Identifier Id as a reference; but do visit destructuring patterns.
                    if (vd.Id is not Identifier)
                    {
                        VisitNode(vd.Id);
                    }
                    VisitNode(vd.Init);
                    return;
            }

            // Fallback traversal for common composite nodes.
            // Keep this conservative; missing nodes just mean fewer edges, which is acceptable for 2b.
            switch (n)
            {
                case Acornima.Ast.Program p:
                    foreach (var s in p.Body) VisitNode(s);
                    break;

                case BlockStatement bs:
                    foreach (var s in bs.Body) VisitNode(s);
                    break;

                case ExpressionStatement es:
                    VisitNode(es.Expression);
                    break;

                case ReturnStatement rs:
                    VisitNode(rs.Argument);
                    break;

                case CallExpression callExpr:
                    VisitNode(callExpr.Callee);
                    foreach (var a in callExpr.Arguments) VisitNode(a as Node);
                    break;

                case AssignmentExpression ae:
                    VisitNode(ae.Left);
                    VisitNode(ae.Right);
                    break;

                case BinaryExpression be:
                    VisitNode(be.Left);
                    VisitNode(be.Right);
                    break;

                case UnaryExpression ue:
                    VisitNode(ue.Argument);
                    break;

                case Acornima.Ast.ConditionalExpression condExpr:
                    VisitNode(condExpr.Test);
                    VisitNode(condExpr.Consequent);
                    VisitNode(condExpr.Alternate);
                    break;

                case IfStatement ifs:
                    VisitNode(ifs.Test);
                    VisitNode(ifs.Consequent);
                    VisitNode(ifs.Alternate);
                    break;

                case WhileStatement ws:
                    VisitNode(ws.Test);
                    VisitNode(ws.Body);
                    break;

                case ForStatement fs:
                    VisitNode(fs.Init);
                    VisitNode(fs.Test);
                    VisitNode(fs.Update);
                    VisitNode(fs.Body);
                    break;

                case ArrayExpression arr:
                    foreach (var e in arr.Elements) VisitNode(e as Node);
                    break;

                case ObjectExpression obj:
                    foreach (var p2 in obj.Properties) VisitNode(p2);
                    break;

                case VariableDeclaration vdecl:
                    foreach (var d in vdecl.Declarations) VisitNode(d);
                    break;

                case SequenceExpression seq:
                    foreach (var e in seq.Expressions) VisitNode(e);
                    break;

                case ThrowStatement ts:
                    VisitNode(ts.Argument);
                    break;

                case TryStatement tr:
                    VisitNode(tr.Block);
                    VisitNode(tr.Handler);
                    VisitNode(tr.Finalizer);
                    break;

                case CatchClause cc:
                    // Param is a declaration pattern; avoid Identifier-as-reference.
                    if (cc.Param is not Identifier)
                    {
                        VisitNode(cc.Param);
                    }
                    VisitNode(cc.Body);
                    break;

                case SwitchStatement ss:
                    VisitNode(ss.Discriminant);
                    foreach (var c in ss.Cases) VisitNode(c);
                    break;

                case Acornima.Ast.SwitchCase sc:
                    VisitNode(sc.Test);
                    foreach (var s in sc.Consequent) VisitNode(s);
                    break;
            }
        }

        // Seed traversal based on callable kind.
        switch (caller.Kind)
        {
            case CallableKind.FunctionDeclaration:
                if (callerAst is FunctionDeclaration fd)
                {
                    foreach (var p in fd.Params) VisitNode(p as Node);
                    VisitNode(fd.Body);
                }
                break;

            case CallableKind.FunctionExpression:
                if (callerAst is FunctionExpression fe)
                {
                    foreach (var p in fe.Params) VisitNode(p as Node);
                    VisitNode(fe.Body);
                }
                break;

            case CallableKind.Arrow:
                if (callerAst is ArrowFunctionExpression af)
                {
                    foreach (var p in af.Params) VisitNode(p as Node);
                    VisitNode(af.Body);
                }
                break;

            case CallableKind.ClassConstructor:
            case CallableKind.ClassMethod:
            case CallableKind.ClassStaticMethod:
                if (callerAst is MethodDefinition md && md.Value is FunctionExpression mdf)
                {
                    foreach (var p in mdf.Params) VisitNode(p as Node);
                    VisitNode(mdf.Body);
                }
                break;

            case CallableKind.ClassStaticInitializer:
                if (callerAst is ClassDeclaration cd)
                {
                    foreach (var prop in cd.Body.Body.OfType<PropertyDefinition>().Where(p => p.Static && p.Value != null))
                    {
                        VisitNode(prop.Value);
                    }
                }
                break;
        }

        // Return stable ordering.
        var ordered = deps
            .OrderBy(d => d.UniqueKey, StringComparer.Ordinal)
            .ToList()
            .AsReadOnly();
        return ordered;
    }

    private Scope? ResolveScopeForCallable(CallableId callable)
    {
        if (callable.AstNode == null)
        {
            return null;
        }

        return callable.Kind switch
        {
            CallableKind.FunctionDeclaration => _symbolTable.FindScopeByAstNode(callable.AstNode),
            CallableKind.FunctionExpression => _symbolTable.FindScopeByAstNode(callable.AstNode),
            CallableKind.Arrow => _symbolTable.FindScopeByAstNode(callable.AstNode),
            CallableKind.ClassConstructor or CallableKind.ClassMethod or CallableKind.ClassStaticMethod
                when callable.AstNode is MethodDefinition md && md.Value is FunctionExpression fe => _symbolTable.FindScopeByAstNode(fe),
            CallableKind.ClassStaticInitializer => _symbolTable.FindScopeByAstNode(callable.AstNode),
            _ => _symbolTable.FindScopeByAstNode(callable.AstNode)
        };
    }

    private static string GetDeclaringScopeName(Scope scope, string moduleName)
    {
        if (scope.Kind == ScopeKind.Global)
        {
            return moduleName;
        }

        var qual = scope.GetQualifiedName();
        return string.IsNullOrEmpty(qual) ? moduleName : $"{moduleName}/{qual}";
    }
}
