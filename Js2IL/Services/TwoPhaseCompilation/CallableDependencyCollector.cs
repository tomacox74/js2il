using System;
using System.Collections.Generic;
using System.Linq;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Utilities;

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

    // Lookup for class method callables by (declaringScopeName, className, methodName)
    private readonly Dictionary<(string DeclaringScopeName, string ClassName, string MethodName), CallableId> _classMethods;

    // Lookup for class static method callables by (declaringScopeName, className, methodName)
    private readonly Dictionary<(string DeclaringScopeName, string ClassName, string MethodName), CallableId> _classStaticMethods;

    public CallableDependencyCollector(SymbolTable symbolTable, CallableRegistry registry, IReadOnlyList<CallableId> callablesInStableOrder)
    {
        _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _callablesInStableOrder = callablesInStableOrder ?? throw new ArgumentNullException(nameof(callablesInStableOrder));

        _classConstructors = new Dictionary<(string, string), CallableId>();
        foreach (var c in _callablesInStableOrder.Where(c => c.Kind == CallableKind.ClassConstructor && !string.IsNullOrEmpty(c.Name)))
        {
            _classConstructors[(c.DeclaringScopeName, c.Name!)] = c;
        }

        _classMethods = new Dictionary<(string, string, string), CallableId>();
        foreach (var c in _callablesInStableOrder.Where(c => c.Kind == CallableKind.ClassMethod && !string.IsNullOrEmpty(c.Name)))
        {
            if (!JavaScriptCallableNaming.TrySplitClassMethodCallableName(c.Name, out var className, out var methodName))
            {
                continue;
            }

            _classMethods[(c.DeclaringScopeName, className, methodName)] = c;
        }

        _classStaticMethods = new Dictionary<(string, string, string), CallableId>();
        foreach (var c in _callablesInStableOrder.Where(c => c.Kind == CallableKind.ClassStaticMethod && !string.IsNullOrEmpty(c.Name)))
        {
            if (!JavaScriptCallableNaming.TrySplitClassMethodCallableName(c.Name, out var className, out var methodName))
            {
                continue;
            }

            _classStaticMethods[(c.DeclaringScopeName, className, methodName)] = c;
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

        // Optional class context for member-call edges (Milestone 2b1).
        // This is intentionally narrow: only this.method() and super.method() are considered.
        string? callerClassName = null;
        bool callerIsStaticMethod = false;
        Scope? callerClassScope = null;
        if (caller.Kind is CallableKind.ClassMethod or CallableKind.ClassStaticMethod)
        {
            callerIsStaticMethod = caller.Kind == CallableKind.ClassStaticMethod;

            if (JavaScriptCallableNaming.TrySplitClassMethodCallableName(caller.Name, out var parsedClass, out _))
            {
                callerClassName = parsedClass;
            }

            // Find the class scope that encloses this method body scope (for resolving super).
            var s = callerScope;
            while (s != null && s.Kind != ScopeKind.Class)
            {
                s = s.Parent;
            }
            callerClassScope = s;
        }

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

        void TryAddThisOrSuperMemberCallDependency(CallExpression callExpr)
        {
            if (callerClassName == null)
            {
                return;
            }

            if (callExpr.Callee is not MemberExpression mem)
            {
                return;
            }

            if (mem.Computed)
            {
                return;
            }

            if (mem.Property is not Identifier propId)
            {
                return;
            }

            var methodName = propId.Name;

            // this.method()
            if (mem.Object is ThisExpression)
            {
                var table = callerIsStaticMethod ? _classStaticMethods : _classMethods;
                if (table.TryGetValue((caller.DeclaringScopeName, callerClassName, methodName), out var callee))
                {
                    AddDependency(callee);
                }
                return;
            }

            // super.method() - resolve base class name via symbol table if possible.
            if (mem.Object is Super)
            {
                if (callerClassScope?.AstNode is not ClassDeclaration classDecl)
                {
                    return;
                }

                if (classDecl.SuperClass is not Identifier superId)
                {
                    return;
                }

                if (callerScope == null)
                {
                    return;
                }

                var symbol = callerScope.FindSymbol(superId.Name);
                var binding = symbol.BindingInfo;
                if (binding.DeclarationNode is not ClassDeclaration superClassDecl)
                {
                    return;
                }

                var superScope = _symbolTable.FindScopeByAstNode(superClassDecl);
                if (superScope?.Parent == null)
                {
                    return;
                }

                var superDeclaringScopeName = GetDeclaringScopeName(superScope.Parent, _symbolTable.Root.Name);
                var superClassName = superId.Name;

                var table = callerIsStaticMethod ? _classStaticMethods : _classMethods;
                if (table.TryGetValue((superDeclaringScopeName, superClassName, methodName), out var callee))
                {
                    AddDependency(callee);
                }
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
                    // Milestone 2b1: add narrow, class-aware member-call edges when statically knowable.
                    // This intentionally ignores obj.m() since it is generally dynamic in JavaScript.
                    TryAddThisOrSuperMemberCallDependency(callExpr);
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
