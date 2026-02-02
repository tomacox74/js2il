using Acornima.Ast;
using Js2IL.Services;
using Js2IL.Utilities;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;
using System.Linq;

namespace Js2IL.Validation;

public class JavaScriptAstValidator : IAstValidator
{
    private static readonly Lazy<HashSet<string>> SupportedRequireModules = new(() =>
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var name in JavaScriptRuntime.Node.NodeModuleRegistry.GetSupportedModuleNames())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    set.Add(name);
                }
            }
        }
        catch { /* Ignore reflection errors; result will be empty set */ }
        return set;
    });

    public ValidationResult Validate(Acornima.Ast.Program ast)
    {
        var result = new ValidationResult { IsValid = true };

        // JS2IL currently only supports strict-mode semantics.
        // Require every module/script to include a directive prologue containing "use strict".
        ValidateUseStrictDirectivePrologue(ast, result);

        // Validate spec-level early errors that Acornima may parse but considers static errors.
        // In particular, break/continue target rules are specified under iteration statements.
        ValidateIterationStatementEarlyErrors(ast, result);

        // Validate async/await usage - await is only valid inside async functions.
        ValidateAsyncAwait(ast, result);

        // Validate generator/yield usage - yield is only valid inside generator functions.
        ValidateGenerators(ast, result);

        // Validate references to undeclared globals so we fail with a clear validation error
        // rather than an unhandled compiler exception later in the pipeline.
        ValidateMissingGlobals(ast, result);
        
        // Track contexts where 'this' and 'super' are supported.
        var contextStack = new Stack<ValidationContext>();
        contextStack.Push(new ValidationContext
        {
            AllowsThis = false,
            AllowsSuper = false,
            ScopeOwner = null,
            MethodDefinitionFunctionValue = null,
            InDerivedClass = false
        });
        
        // Visit all nodes in the AST
        var walker = new AstWalker();
        walker.VisitWithContext(ast, node =>
        {
            var currentContext = contextStack.Peek();

            // Track when we're inside a derived class (class with an extends clause).
            if (node is ClassDeclaration cd)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = currentContext.AllowsThis,
                    AllowsSuper = currentContext.AllowsSuper,
                    ScopeOwner = cd,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue,
                    InObjectPattern = currentContext.InObjectPattern,
                    InDerivedClass = cd.SuperClass != null
                });
            }
            else if (node is ClassExpression ce)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = currentContext.AllowsThis,
                    AllowsSuper = currentContext.AllowsSuper,
                    ScopeOwner = ce,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue,
                    InObjectPattern = currentContext.InObjectPattern,
                    InDerivedClass = ce.SuperClass != null
                });
            }

            // Track when we're inside an object binding pattern (destructuring).
            // We currently support object patterns but do not support computed keys inside them.
            if (node is ObjectPattern)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = currentContext.AllowsThis,
                    AllowsSuper = currentContext.AllowsSuper,
                    ScopeOwner = node,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue,
                    InObjectPattern = true,
                    InDerivedClass = currentContext.InDerivedClass
                });
            }
            
            // Push new context for class methods and constructors
            if (node is MethodDefinition methodDef)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = true,
                    // Allow super in class methods/constructors only when we're inside a derived class.
                    AllowsSuper = currentContext.InDerivedClass,
                    ScopeOwner = methodDef,
                    // Track the function expression that is the method body so we don't treat it as nested
                    MethodDefinitionFunctionValue = methodDef.Value,
                    InDerivedClass = currentContext.InDerivedClass
                });
            }
            // Push new context for functions (exclude the method body itself).
            else if ((node is ArrowFunctionExpression || node is FunctionExpression || node is FunctionDeclaration)
                     && !ReferenceEquals(node, currentContext.MethodDefinitionFunctionValue))
            {
                contextStack.Push(new ValidationContext
                {
                    // Arrow functions have lexical 'this' (they inherit it from the enclosing context).
                    // Non-arrow functions establish their own 'this'.
                    AllowsThis = node is ArrowFunctionExpression ? currentContext.AllowsThis : true,
                    AllowsSuper = false,
                    ScopeOwner = node,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue,
                    InDerivedClass = currentContext.InDerivedClass
                });
            }
            
            // Check for unsupported features
            switch (node.Type)
            {
                case NodeType.ClassDeclaration:
                case NodeType.ClassExpression:
                    // Classes are supported - validation for class-specific features 
                    // is handled by MethodDefinition and PropertyDefinition cases
                    break;

                case NodeType.ImportDeclaration:
                case NodeType.ExportNamedDeclaration:
                case NodeType.ExportDefaultDeclaration:
                    result.Errors.Add($"ES6 modules are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.YieldExpression:
                    // Generator/yield validity is handled by ValidateGenerators.
                    break;

                case NodeType.RestElement:
                    // RestElement is used for both rest parameters and rest destructuring patterns.
                    // Rest destructuring is supported; rest parameters are validated in ValidateFunctionParameters.
                    break;

                case NodeType.ForInStatement:
                    // for...in loops are supported (PL2.4)
                    break;

                case NodeType.SwitchStatement:
                    // Switch statements are supported (PL2.6)
                    break;

                case NodeType.WithStatement:
                    result.Errors.Add($"The 'with' statement is not supported (deprecated and problematic) (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.LabeledStatement:
                    // Labeled statements are supported (used for labeled break/continue)
                    break;

                case NodeType.DebuggerStatement:
                    result.Errors.Add($"The 'debugger' statement is not supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.MetaProperty:
                    // new.target and import.meta
                    result.Errors.Add($"new.target/import.meta are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.Super:
                    // Support super in derived class methods/constructors.
                    if (!currentContext.AllowsSuper)
                    {
                        result.Errors.Add($"The 'super' keyword is not yet supported in this context (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                    break;

                case NodeType.ThisExpression:
                    // 'this' is supported in class methods/constructors and non-arrow functions.
                    if (!currentContext.AllowsThis)
                    {
                        result.Errors.Add($"The 'this' keyword is not yet supported in this context (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                    break;

                case NodeType.ArrayPattern:
                    // Array destructuring is supported.
                    break;

                case NodeType.ObjectPattern:
                    // Object destructuring is supported.
                    break;

                case NodeType.AssignmentExpression:
                    // Destructuring assignment is supported.
                    break;

                case NodeType.Property:
                    // Check for computed properties, getters/setters in object literals
                    ValidateProperty(node, result, contextStack.Peek());
                    break;

                case NodeType.MethodDefinition:
                    // Check for getters/setters in classes
                    ValidateMethodDefinition(node, result);
                    break;

                case NodeType.PropertyDefinition:
                    // Check for computed keys in class field definitions
                    ValidatePropertyDefinition(node, result);
                    break;

                case NodeType.StaticBlock:
                    result.Errors.Add($"Class static blocks are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.CallExpression:
                    // Detect require(...) patterns and spread in call arguments
                    ValidateCallExpression(node, result);
                    break;

                case NodeType.FunctionDeclaration:
                case NodeType.FunctionExpression:
                case NodeType.ArrowFunctionExpression:
                    // Check for parameter count limit
                    ValidateFunctionParameters(node, result);
                    break;
            }
        }, exitNode =>
        {
            if (contextStack.Count > 1 && ReferenceEquals(contextStack.Peek().ScopeOwner, exitNode))
            {
                contextStack.Pop();
            }
        });

        return result;
    }

    private sealed class ScopeFrame
    {
        public required HashSet<string> DeclaredNames { get; init; }
        public required bool IsFunctionScope { get; init; }
    }

    private static readonly Lazy<HashSet<string>> KnownGlobalConstants = new(() =>
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        // JS 'undefined' is modeled as CLR null rather than a GlobalThis property.
        names.Add("undefined");

        // Reflect constant-like globals exposed by JavaScriptRuntime.GlobalThis (e.g., NaN, Infinity).
        try
        {
            var numericTypes = new HashSet<Type>
            {
                typeof(double),
                typeof(float),
                typeof(int),
                typeof(long)
            };

            var reflectedNames = typeof(JavaScriptRuntime.GlobalThis)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                // Keep this intentionally conservative: only treat simple numeric constants
                // as always-available globals.
                .Where(p => p != null && !string.IsNullOrWhiteSpace(p.Name) && numericTypes.Contains(p.PropertyType))
                .Select(p => p.Name);

            names.UnionWith(reflectedNames);
        }
        catch
        {
            // Ignore reflection errors; fall back to 'undefined' only.
        }

        return names;
    });

    // CommonJS/module wrapper globals that are provided by js2il hosting/module loader.
    // Collated from the runtime's shared module parameter list to avoid duplication.
    private static readonly Lazy<HashSet<string>> AllowedInjectedGlobals = new(() =>
    {
        try
        {
            return JavaScriptRuntime.CommonJS.ModuleParameters.Parameters
                .Select(p => p.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToHashSet(StringComparer.Ordinal);
        }
        catch
        {
            // Ignore runtime reflection/type-load issues; validator will fall back to stricter behavior.
            return new HashSet<string>(StringComparer.Ordinal);
        }
    });

    // Primitive conversion callables supported directly by the IR pipeline.
    // We collate these from the runtime intrinsic types so the validator doesn't need to hardcode names.
    private static readonly Lazy<HashSet<string>> AllowedGlobalCallables = new(() =>
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        try
        {
            static void AddIntrinsicName(HashSet<string> set, Type t)
            {
                var attr = (JavaScriptRuntime.IntrinsicObjectAttribute?)t
                    .GetCustomAttributes(typeof(JavaScriptRuntime.IntrinsicObjectAttribute), inherit: false)
                    .FirstOrDefault();
                if (attr != null && !string.IsNullOrWhiteSpace(attr.Name))
                {
                    set.Add(attr.Name);
                }
            }

            // These types are the runtime intrinsics that the compiler lowers as primitive conversion callables.
            AddIntrinsicName(names, typeof(JavaScriptRuntime.String));
            AddIntrinsicName(names, typeof(JavaScriptRuntime.Number));
            AddIntrinsicName(names, typeof(JavaScriptRuntime.Boolean));
            AddIntrinsicName(names, typeof(JavaScriptRuntime.BigInt));
            AddIntrinsicName(names, typeof(JavaScriptRuntime.Symbol));
        }
        catch
        {
            // Ignore reflection errors; the validator will fall back to stricter behavior.
        }

        return names;
    });

    private static readonly Lazy<HashSet<string>> GlobalThisPropertyNames = new(() =>
    {
        try
        {
            return typeof(JavaScriptRuntime.GlobalThis)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(p => p.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
        catch { /* reflection errors -> empty set */ }
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    });

    private static readonly Lazy<HashSet<string>> GlobalThisMethodNames = new(() =>
    {
        try
        {
            return typeof(JavaScriptRuntime.GlobalThis)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                // Skip property accessors and infrastructure.
                .Where(m => !m.IsSpecialName)
                .Select(m => m.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
        catch { /* reflection errors -> empty set */ }
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    });

    private void ValidateMissingGlobals(Acornima.Ast.Program ast, ValidationResult result)
    {
        var walker = new AstWalker();

        var nodeStack = new Stack<Node>();
        var scopeStack = new Stack<ScopeFrame>();

        // Program scope is the outermost function scope.
        scopeStack.Push(new ScopeFrame
        {
            DeclaredNames = new HashSet<string>(StringComparer.Ordinal),
            IsFunctionScope = true
        });

        void CollectHoistedDeclarations(Node node, ScopeFrame target)
        {
            // For the purpose of missing-globals validation we treat function declarations and `var`
            // declarations as hoisted to the nearest function scope. This avoids false positives for
            // patterns like calling a function before its declaration.
            void VisitHoist(Node n)
            {
                switch (n)
                {
                    case FunctionDeclaration fd:
                        if (fd.Id != null)
                        {
                            target.DeclaredNames.Add(fd.Id.Name);
                        }
                        // Do not descend into nested function scopes.
                        return;

                    case FunctionExpression:
                    case ArrowFunctionExpression:
                        // Do not descend into nested function scopes.
                        return;

                    case VariableDeclaration vd when vd.Kind == VariableDeclarationKind.Var:
                        foreach (var decl in vd.Declarations)
                        {
                            if (decl?.Id != null)
                            {
                                DeclarePatternNames(decl.Id, target);
                            }
                        }
                        break;
                }

                foreach (var child in n.ChildNodes)
                {
                    VisitHoist(child);
                }
            }

            VisitHoist(node);
        }

        // Pre-collect hoisted declarations for the top-level (Program) scope.
        CollectHoistedDeclarations(ast, scopeStack.Peek());

        static bool IsBindingPatternNode(Node n)
            => n is ObjectPattern
            || n is ArrayPattern
            || n is RestElement
            || n is AssignmentPattern;

        void DeclarePatternNames(Node pattern, ScopeFrame target)
        {
            switch (pattern)
            {
                case Identifier id:
                    target.DeclaredNames.Add(id.Name);
                    break;
                case RestElement rest:
                    if (rest.Argument != null) DeclarePatternNames(rest.Argument, target);
                    break;
                case AssignmentPattern ap:
                    if (ap.Left != null) DeclarePatternNames(ap.Left, target);
                    break;
                case ArrayPattern arr:
                    foreach (var el in arr.Elements)
                    {
                        if (el != null) DeclarePatternNames(el, target);
                    }
                    break;
                case ObjectPattern obj:
                    foreach (var prop in obj.Properties)
                    {
                        switch (prop)
                        {
                            case Property p:
                                if (p.Value != null) DeclarePatternNames(p.Value, target);
                                break;
                            case RestElement re:
                                if (re.Argument != null) DeclarePatternNames(re.Argument, target);
                                break;
                        }
                    }
                    break;
            }
        }

        ScopeFrame FindNearestFunctionScope()
        {
            foreach (var frame in scopeStack)
            {
                if (frame.IsFunctionScope) return frame;
            }
            return scopeStack.Peek();
        }

        bool IsDeclared(string name)
        {
            foreach (var frame in scopeStack)
            {
                if (frame.DeclaredNames.Contains(name)) return true;
            }
            return false;
        }

        bool IsIntrinsicObjectName(string name)
        {
            try
            {
                return JavaScriptRuntime.IntrinsicObjectRegistry.GetInfo(name) != null;
            }
            catch
            {
                return false;
            }
        }

        void ReportMissingGlobal(string name, Node node, bool calledAsFunction)
        {
            var line = node.Location.Start.Line;
            if (calledAsFunction)
            {
                result.Errors.Add($"Global function '{name}' is not yet supported (line {line}).");
            }
            else
            {
                result.Errors.Add($"Global identifier '{name}' is not yet supported (line {line}).");
            }
            result.IsValid = false;
        }

        walker.VisitWithContext(
            ast,
            enterNode: node =>
            {
                nodeStack.Push(node);

                switch (node)
                {
                    case BlockStatement:
                        scopeStack.Push(new ScopeFrame
                        {
                            DeclaredNames = new HashSet<string>(StringComparer.Ordinal),
                            IsFunctionScope = false
                        });
                        break;

                    case CatchClause cc:
                        // Catch introduces a new lexical scope for its param.
                        scopeStack.Push(new ScopeFrame
                        {
                            DeclaredNames = new HashSet<string>(StringComparer.Ordinal),
                            IsFunctionScope = false
                        });
                        if (cc.Param != null)
                        {
                            DeclarePatternNames(cc.Param, scopeStack.Peek());
                        }
                        break;

                    case FunctionDeclaration fd:
                        // Function declaration name is bound in the parent scope.
                        if (fd.Id != null)
                        {
                            // Function declarations are effectively var-scoped.
                            FindNearestFunctionScope().DeclaredNames.Add(fd.Id.Name);
                        }

                        // New function scope for parameters/body.
                        scopeStack.Push(new ScopeFrame
                        {
                            DeclaredNames = new HashSet<string>(StringComparer.Ordinal),
                            IsFunctionScope = true
                        });
                        foreach (var p in fd.Params)
                        {
                            if (p != null) DeclarePatternNames(p, scopeStack.Peek());
                        }

                        // Pre-collect hoisted declarations within this function scope.
                        if (fd.Body != null)
                        {
                            CollectHoistedDeclarations(fd.Body, scopeStack.Peek());
                        }
                        break;

                    case FunctionExpression fe:
                        scopeStack.Push(new ScopeFrame
                        {
                            DeclaredNames = new HashSet<string>(StringComparer.Ordinal),
                            IsFunctionScope = true
                        });
                        // Named function expressions bind their name in their own scope.
                        if (fe.Id != null)
                        {
                            scopeStack.Peek().DeclaredNames.Add(fe.Id.Name);
                        }
                        foreach (var p in fe.Params)
                        {
                            if (p != null) DeclarePatternNames(p, scopeStack.Peek());
                        }

                        // Pre-collect hoisted declarations within this function scope.
                        if (fe.Body != null)
                        {
                            CollectHoistedDeclarations(fe.Body, scopeStack.Peek());
                        }
                        break;

                    case ArrowFunctionExpression afe:
                        scopeStack.Push(new ScopeFrame
                        {
                            DeclaredNames = new HashSet<string>(StringComparer.Ordinal),
                            IsFunctionScope = true
                        });
                        foreach (var p in afe.Params)
                        {
                            if (p != null) DeclarePatternNames(p, scopeStack.Peek());
                        }

                        // Arrow functions with block bodies can contain hoisted `var` and nested
                        // function declarations (var-scoped to the arrow function).
                        if (afe.Body is BlockStatement bs)
                        {
                            CollectHoistedDeclarations(bs, scopeStack.Peek());
                        }
                        break;

                    case ClassDeclaration cd:
                        if (cd.Id != null)
                        {
                            // Class declarations are block scoped.
                            scopeStack.Peek().DeclaredNames.Add(cd.Id.Name);
                        }
                        break;

                    case VariableDeclaration vd:
                        {
                            var target = vd.Kind == VariableDeclarationKind.Var
                                ? FindNearestFunctionScope()
                                : scopeStack.Peek();

                            foreach (var decl in vd.Declarations)
                            {
                                if (decl?.Id != null)
                                {
                                    DeclarePatternNames(decl.Id, target);
                                }
                            }
                        }
                        break;

                    case Identifier id:
                        {
                            // Ignore identifiers in binding patterns (they are declarations).
                            if (nodeStack.Skip(1).Any(IsBindingPatternNode))
                            {
                                break;
                            }

                            // Ignore identifiers used as property keys when not computed.
                            var parent = nodeStack.Count > 1 ? nodeStack.Skip(1).First() : null;
                            if (parent is MemberExpression me && !me.Computed && ReferenceEquals(me.Property, id))
                            {
                                break;
                            }
                            if (parent is Property p && !p.Computed && ReferenceEquals(p.Key, id))
                            {
                                break;
                            }
                            if (parent is MethodDefinition md && !md.Computed && ReferenceEquals(md.Key, id))
                            {
                                break;
                            }
                            if (parent is PropertyDefinition pd && !pd.Computed && ReferenceEquals(pd.Key, id))
                            {
                                break;
                            }

                            // Ignore declaration identifiers that the walker doesn't visit (should already be handled).
                            // Now treat remaining identifiers as references.
                            var name = id.Name;
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                break;
                            }

                            // Locals and known built-in constants are always allowed.
                            if (IsDeclared(name) || KnownGlobalConstants.Value.Contains(name))
                            {
                                break;
                            }

                            // CommonJS injected values.
                            if (AllowedInjectedGlobals.Value.Contains(name))
                            {
                                break;
                            }

                            // Determine if this identifier is in a call/new callee position.
                            bool calledAsFunction = parent is CallExpression ce && ReferenceEquals(ce.Callee, id);
                            bool calledAsConstructor = parent is NewExpression ne && ReferenceEquals(ne.Callee, id);
                            bool invoked = calledAsFunction || calledAsConstructor;

                            if (invoked)
                            {
                                if (AllowedGlobalCallables.Value.Contains(name))
                                {
                                    break;
                                }

                                JavaScriptRuntime.IntrinsicObjectInfo? intrinsicInfo = null;
                                try
                                {
                                    intrinsicInfo = JavaScriptRuntime.IntrinsicObjectRegistry.GetInfo(name);
                                }
                                catch
                                {
                                    intrinsicInfo = null;
                                }

                                // Many intrinsics are constructible (e.g., Promise, Int32Array) but do not need a special
                                // intrinsic call kind: the compiler maps `new X(...)` to the intrinsic CLR type/ctor.
                                if (calledAsConstructor)
                                {
                                    if (intrinsicInfo != null)
                                    {
                                        break;
                                    }

                                    ReportMissingGlobal(name, id, calledAsFunction: true);
                                    break;
                                }

                                // Callable intrinsics (e.g., Error(...), Symbol(...), BigInt(...))
                                if (intrinsicInfo != null && intrinsicInfo.CallKind != JavaScriptRuntime.IntrinsicCallKind.None)
                                {
                                    break;
                                }

                                // GlobalThis static method callables (e.g., parseInt, setTimeout)
                                if (GlobalThisMethodNames.Value.Contains(name))
                                {
                                    break;
                                }

                                ReportMissingGlobal(name, id, calledAsFunction: true);
                                break;
                            }

                            // Identifier used as a value.
                            // Intrinsic objects (e.g., Math, JSON) are supported when used as the base of a member access
                            // (Math.abs, JSON.parse) or when invoked/constructed. They are not generally supported as
                            // first-class values unless explicitly exposed via a GlobalThis value property.
                            if (GlobalThisPropertyNames.Value.Contains(name))
                            {
                                break;
                            }

                            var isIntrinsic = IsIntrinsicObjectName(name);
                            var usedAsMemberBase = parent is MemberExpression meObj && ReferenceEquals(meObj.Object, id);
                            var usedAsExtendsBase = (parent is ClassDeclaration cd && ReferenceEquals(cd.SuperClass, id))
                                || (parent is ClassExpression classExpr && ReferenceEquals(classExpr.SuperClass, id));
                            if (isIntrinsic && (usedAsMemberBase || usedAsExtendsBase))
                            {
                                break;
                            }

                            ReportMissingGlobal(name, id, calledAsFunction: false);
                        }
                        break;
                }
            },
            exitNode: node =>
            {
                // Pop scopes on exit.
                switch (node)
                {
                    case BlockStatement:
                        scopeStack.Pop();
                        break;
                    case CatchClause:
                        scopeStack.Pop();
                        break;
                    case FunctionDeclaration:
                    case FunctionExpression:
                    case ArrowFunctionExpression:
                        scopeStack.Pop();
                        break;
                }

                nodeStack.Pop();
            });
    }

    private static void ValidateUseStrictDirectivePrologue(Acornima.Ast.Program ast, ValidationResult result)
    {
        var hasUseStrict = false;

        foreach (var statement in ast.Body)
        {
            if (statement is ExpressionStatement { Expression: StringLiteral str })
            {
                if (string.Equals(str.Value, "use strict", StringComparison.Ordinal))
                {
                    hasUseStrict = true;
                    break;
                }
                continue;
            }

            // Directive prologue ends at the first non-string-literal expression statement.
            break;
        }

        if (!hasUseStrict)
        {
            AddError(
                result,
                "JS2IL requires strict mode: add a \"use strict\"; directive prologue at the start of the module",
                ast);
        }
    }

    private static void ValidateIterationStatementEarlyErrors(Acornima.Ast.Program ast, ValidationResult result)
    {
        var walker = new AstWalker();
        var functionContexts = new Stack<EarlyErrorContext>();
        functionContexts.Push(new EarlyErrorContext());

        walker.VisitWithContext(ast, node =>
        {
            // Function boundaries reset label/loop/switch target sets.
            if (node is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression)
            {
                functionContexts.Push(new EarlyErrorContext());
                return;
            }

            var ctx = functionContexts.Peek();

            switch (node)
            {
                case LabeledStatement labeledStmt:
                    {
                        var labelNode = labeledStmt.Label;
                        var labelName = labelNode?.Name;
                        if (string.IsNullOrWhiteSpace(labelName))
                        {
                            break;
                        }

                        if (!ctx.ActiveLabelNames.Add(labelName))
                        {
                            AddError(result,
                                $"Duplicate label '{labelName}' is not allowed",
                                labelNode!);
                            // Still push so the exit pop stays consistent.
                        }

                        ctx.LabelStack.Push(new LabelEntry(labelName, labeledStmt.Body, labelNode!));
                        break;
                    }

                case SwitchStatement:
                    ctx.BreakableStack.Push(BreakableKind.Switch);
                    break;

                case DoWhileStatement:
                case WhileStatement:
                case ForStatement:
                case ForInStatement:
                case ForOfStatement:
                    ctx.BreakableStack.Push(BreakableKind.Iteration);
                    ctx.IterationDepth++;

                    if (node is ForInStatement forIn)
                    {
                        ValidateForInOfLeft(forIn.Left, isForOf: false, result);
                    }
                    else if (node is ForOfStatement forOf)
                    {
                        ValidateForInOfLeft(forOf.Left, isForOf: true, result);
                    }
                    break;

                case BreakStatement breakStmt:
                    ValidateBreakStatement(breakStmt, ctx, result);
                    break;

                case ContinueStatement continueStmt:
                    ValidateContinueStatement(continueStmt, ctx, result);
                    break;
            }
        }, exitNode =>
        {
            // Function boundaries restore previous context.
            if (exitNode is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression)
            {
                if (functionContexts.Count > 1)
                {
                    functionContexts.Pop();
                }
                return;
            }

            var ctx = functionContexts.Peek();

            switch (exitNode)
            {
                case LabeledStatement:
                    if (ctx.LabelStack.Count > 0)
                    {
                        var entry = ctx.LabelStack.Pop();
                        ctx.ActiveLabelNames.Remove(entry.Name);
                    }
                    break;

                case SwitchStatement:
                    if (ctx.BreakableStack.Count > 0)
                    {
                        ctx.BreakableStack.Pop();
                    }
                    break;

                case DoWhileStatement:
                case WhileStatement:
                case ForStatement:
                case ForInStatement:
                case ForOfStatement:
                    if (ctx.BreakableStack.Count > 0)
                    {
                        ctx.BreakableStack.Pop();
                    }
                    if (ctx.IterationDepth > 0)
                    {
                        ctx.IterationDepth--;
                    }
                    break;
            }
        });
    }

    private static void ValidateBreakStatement(BreakStatement breakStmt, EarlyErrorContext ctx, ValidationResult result)
    {
        if (breakStmt.Label == null)
        {
            if (ctx.BreakableStack.Count == 0)
            {
                AddError(result, "Illegal break statement (not inside a loop or switch)", breakStmt);
            }
            return;
        }

        var labelName = breakStmt.Label.Name;
        if (!TryFindLabelTarget(labelName, ctx, out _))
        {
            AddError(result, $"Undefined label '{labelName}' in break statement", breakStmt.Label);
        }
    }

    private static void ValidateContinueStatement(ContinueStatement continueStmt, EarlyErrorContext ctx, ValidationResult result)
    {
        if (continueStmt.Label == null)
        {
            if (ctx.IterationDepth == 0)
            {
                AddError(result, "Illegal continue statement (not inside a loop)", continueStmt);
            }
            return;
        }

        var labelName = continueStmt.Label.Name;
        if (!TryFindLabelTarget(labelName, ctx, out var targetStatement))
        {
            AddError(result, $"Undefined label '{labelName}' in continue statement", continueStmt.Label);
            return;
        }

        if (!IsIterationStatement(targetStatement))
        {
            AddError(result, $"Illegal continue target '{labelName}' (label does not refer to a loop)", continueStmt.Label);
        }
    }

    private static bool TryFindLabelTarget(string labelName, EarlyErrorContext ctx, out Node? targetStatement)
    {
        foreach (var entry in ctx.LabelStack)
        {
            if (string.Equals(entry.Name, labelName, StringComparison.Ordinal))
            {
                targetStatement = entry.TargetStatement;
                return true;
            }
        }

        targetStatement = null;
        return false;
    }

    private static bool IsIterationStatement(Node? node)
    {
        return node is DoWhileStatement
            or WhileStatement
            or ForStatement
            or ForInStatement
            or ForOfStatement;
    }

    private static void ValidateForInOfLeft(Node left, bool isForOf, ValidationResult result)
    {
        // Spec-level early error: for-in/of variable declarations must not have initializers
        // and must declare exactly one binding.
        if (left is VariableDeclaration vd)
        {
            if (vd.Declarations.Count != 1)
            {
                AddError(result,
                    $"Invalid {(isForOf ? "for...of" : "for...in")} head: exactly one variable declarator is required",
                    vd);
                return;
            }

            var decl = vd.Declarations[0];
            if (decl.Init != null)
            {
                AddError(result,
                    $"Invalid {(isForOf ? "for...of" : "for...in")} head: variable declarator cannot have an initializer",
                    decl.Init);
            }

            return;
        }

        // Defensive: Acornima usually won't produce these, but if it does, treat as early error.
        if (left is AssignmentExpression)
        {
            AddError(result,
                $"Invalid {(isForOf ? "for...of" : "for...in")} head: assignment is not allowed in the loop target",
                left);
        }
    }

    private static void AddError(ValidationResult result, string message, Node node)
    {
        var loc = node.Location.Start;
        result.Errors.Add($"{message} (line {loc.Line}, col {loc.Column})");
        result.IsValid = false;
    }

    private enum BreakableKind
    {
        Iteration,
        Switch
    }

    private sealed record LabelEntry(string Name, Node TargetStatement, Node LabelNode);

    private sealed class EarlyErrorContext
    {
        public int IterationDepth;
        public Stack<BreakableKind> BreakableStack { get; } = new();
        public Stack<LabelEntry> LabelStack { get; } = new();
        public HashSet<string> ActiveLabelNames { get; } = new(StringComparer.Ordinal);
    }

    private static void ValidateAsyncAwait(Node ast, ValidationResult result)
    {
        // Track whether we're inside an async function for await validation.
        // async functions themselves are now supported; await is only valid inside them.
        var asyncFunctionDepth = 0;
        var finallyDepth = 0; // Track if we're inside a finally block
        var visited = new HashSet<Node>(ReferenceEqualityComparer<Node>.Default);

        void WalkForAsyncAwait(Node? node)
        {
            if (node is null) return;
            if (!visited.Add(node)) return;

            // Track entering async functions
            bool isAsyncFunction = node switch
            {
                FunctionDeclaration fd => fd.Async,
                FunctionExpression fe => fe.Async,
                ArrowFunctionExpression af => af.Async,
                _ => false
            };

            if (isAsyncFunction)
            {
                asyncFunctionDepth++;
            }

            // Handle try statements explicitly to distinguish finally blocks
            if (node is TryStatement tryStmt)
            {
                WalkForAsyncAwait(tryStmt.Block);

                if (tryStmt.Handler != null)
                {
                    WalkForAsyncAwait(tryStmt.Handler);
                }

                if (tryStmt.Finalizer != null)
                {
                    finallyDepth++;
                    WalkForAsyncAwait(tryStmt.Finalizer);
                    finallyDepth--;
                }

                return;
            }

            // Validate await is only inside async functions
            if (node.Type == NodeType.AwaitExpression)
            {
                if (asyncFunctionDepth == 0)
                {
                    result.Errors.Add($"The 'await' keyword is only valid inside async functions (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }

            // Validate for-await-of is not yet supported
            if (node is ForOfStatement forOfStmt && forOfStmt.Await)
            {
                if (asyncFunctionDepth == 0)
                {
                    result.Errors.Add($"The 'for await...of' statement is only valid inside async functions (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }

            // Walk children
            var type = node.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!prop.CanRead) continue;
                if (prop.GetIndexParameters().Length != 0) continue;

                object? value;
                try { value = prop.GetValue(node); }
                catch { continue; }

                if (value is null || value is string) continue;

                if (value is Node childNode)
                {
                    WalkForAsyncAwait(childNode);
                }
                else if (value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node itemNode)
                        {
                            WalkForAsyncAwait(itemNode);
                        }
                    }
                }
            }

            // Track exiting async functions
            if (isAsyncFunction)
            {
                asyncFunctionDepth--;
            }
        }

        WalkForAsyncAwait(ast);
    }

    private static void ValidateGenerators(Node ast, ValidationResult result)
    {
        var generatorFunctionDepth = 0;
        var visited = new HashSet<Node>(ReferenceEqualityComparer<Node>.Default);

        void WalkForGenerators(Node? node)
        {
            if (node == null) return;
            if (!visited.Add(node)) return;

            bool isGeneratorFunction = node switch
            {
                FunctionDeclaration fd => fd.Generator,
                FunctionExpression fe => fe.Generator,
                // Arrow functions cannot be generators.
                _ => false
            };

            if (isGeneratorFunction)
            {
                generatorFunctionDepth++;
            }

            if (node is YieldExpression ye)
            {
                if (generatorFunctionDepth == 0)
                {
                    result.Errors.Add($"The 'yield' keyword is only valid inside generator functions (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }

            // Walk children (reflection-based, consistent with ValidateAsyncAwait)
            var type = node.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!prop.CanRead) continue;
                if (prop.GetIndexParameters().Length != 0) continue;

                object? value;
                try { value = prop.GetValue(node); }
                catch { continue; }

                if (value is null || value is string) continue;

                if (value is Node childNode)
                {
                    WalkForGenerators(childNode);
                }
                else if (value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node itemNode)
                        {
                            WalkForGenerators(itemNode);
                        }
                    }
                }
            }

            if (isGeneratorFunction)
            {
                generatorFunctionDepth--;
            }
        }

        WalkForGenerators(ast);
    }

    private static void WalkAllNodes(Node? root, HashSet<Node> visited, Action<Node> visit)
    {
        if (root is null) return;
        if (!visited.Add(root)) return;

        visit(root);

        var type = root.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!prop.CanRead) continue;
            if (prop.GetIndexParameters().Length != 0) continue;

            object? value;
            try
            {
                value = prop.GetValue(root);
            }
            catch
            {
                continue;
            }

            if (value is null) continue;
            if (value is Node childNode)
            {
                WalkAllNodes(childNode, visited, visit);
                continue;
            }

            if (value is string) continue;

            if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item is Node itemNode)
                    {
                        WalkAllNodes(itemNode, visited, visit);
                    }
                }
            }
        }
    }

    // Simple reference equality comparer so the visited set doesn't depend on Node.Equals implementations.
    private sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public static readonly ReferenceEqualityComparer<T> Default = new();
        public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
        public int GetHashCode(T obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }

    // Note: Object/Array patterns (including nested + defaults + rest) and destructuring assignment are supported.

    private void ValidateProperty(Node node, ValidationResult result, ValidationContext currentContext)
    {
        if (node is Property prop)
        {
            // Computed keys are supported in object literals, but not yet supported in object binding patterns.
            if (prop.Computed && currentContext.InObjectPattern)
            {
                result.Errors.Add($"Computed property names are not yet supported in this context (line {node.Location.Start.Line})");
                result.IsValid = false;
            }

            // Check for getters and setters
            if (prop.Kind == PropertyKind.Get)
            {
                result.Errors.Add($"Getter properties are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
            else if (prop.Kind == PropertyKind.Set)
            {
                result.Errors.Add($"Setter properties are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
        }
    }

    private void ValidateMethodDefinition(Node node, ValidationResult result)
    {
        if (node is MethodDefinition method)
        {
            if (method.Key is PrivateIdentifier)
            {
                result.Errors.Add($"Private methods in classes are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
                return;
            }

            if (method.Computed || method.Key is not Identifier)
            {
                result.Errors.Add($"Computed/non-identifier method names in classes are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
                return;
            }

            // Check for getters and setters in classes
            if (method.Kind == PropertyKind.Get)
            {
                result.Errors.Add($"Getter methods in classes are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
            else if (method.Kind == PropertyKind.Set)
            {
                result.Errors.Add($"Setter methods in classes are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
            // Static methods are supported
        }
    }

    private void ValidatePropertyDefinition(Node node, ValidationResult result)
    {
        if (node is PropertyDefinition pdef
            && (pdef.Computed || (pdef.Key is not Identifier && pdef.Key is not PrivateIdentifier)))
        {
            result.Errors.Add($"Computed/non-identifier class field names are not yet supported (line {node.Location.Start.Line})");
            result.IsValid = false;
        }
    }

    private void ValidateCallExpression(Node node, ValidationResult result)
    {
        if (node is CallExpression call)
        {
            // Check for spread in function call arguments
            foreach (var arg in call.Arguments)
            {
                if (arg is SpreadElement)
                {
                    result.Errors.Add($"Spread in function calls is not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }

            // Check for require() patterns
            if (call.Callee is Identifier id && id.Name == "require")
            {
                if (call.Arguments.Count == 0)
                {
                    result.Errors.Add($"require() called without arguments (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
                else if (call.Arguments[0] is Literal lit && lit.Value is string modName)
                {
                    var normalizedName = JavaScriptRuntime.Node.NodeModuleRegistry.NormalizeModuleName(modName);
                    var isLocalModule = normalizedName.StartsWith(".") || normalizedName.StartsWith("/");

                    if (!SupportedRequireModules.Value.Contains(normalizedName) && !isLocalModule)
                    {
                        result.Errors.Add($"Module '{modName}' is not yet supported (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                }
                else
                {
                    // Dynamic/non-literal require argument detected.
                    result.Errors.Add($"Dynamic require() with non-literal argument is not supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }
        }
    }

    private void ValidateFunctionParameters(Node node, ValidationResult result)
    {
        NodeList<Node>? parameters = node switch
        {
            FunctionDeclaration fd => fd.Params,
            FunctionExpression fe => fe.Params,
            ArrowFunctionExpression af => af.Params,
            _ => null
        };

        int paramCount = parameters?.Count ?? 0;

        // Check for parameter count limit
        if (paramCount > 32)
        {
            result.Errors.Add($"Functions with more than 32 parameters are not yet supported (line {node.Location.Start.Line})");
            result.IsValid = false;
        }

        // Rest parameters (...args) are not supported.
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                if (param is RestElement)
                {
                    result.Errors.Add($"Rest parameters are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;
                }
            }
        }
    }

    private class ValidationContext
    {
        public bool AllowsThis { get; set; }
        public bool AllowsSuper { get; set; }
        public Node? ScopeOwner { get; set; }
        // Track the FunctionExpression that is the direct body of a MethodDefinition
        // so we don't incorrectly treat it as a nested function
        public Node? MethodDefinitionFunctionValue { get; set; }
        public bool InObjectPattern { get; set; }
        public bool InDerivedClass { get; set; }
    }
}
