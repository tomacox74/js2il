using Acornima.Ast;
using System.Collections.Generic;
using System.Linq;

namespace Js2IL.SymbolTables
{
    /// <summary>
    /// Represents the symbol table for JavaScript source code.
    /// Contains the complete scope hierarchy and provides access to all symbols and functions.
    /// </summary>
    public class SymbolTable
    {
        public Scope Root { get; }

        public SymbolTable(Scope root)
        {
            Root = root;
        }

        /// <summary>
        /// Gets all function declarations in the symbol table, including nested functions.
        /// </summary>
        public IEnumerable<(Scope FunctionScope, FunctionDeclaration Declaration)> GetAllFunctions()
        {
            return GetAllFunctionsRecursive(Root);
        }

        private IEnumerable<(Scope, FunctionDeclaration)> GetAllFunctionsRecursive(Scope scope)
        {
            if (scope.Kind == ScopeKind.Function && scope.AstNode is FunctionDeclaration funcDecl)
            {
                yield return (scope, funcDecl);
            }
            
            foreach (var childScope in scope.Children)
            {
                foreach (var func in GetAllFunctionsRecursive(childScope))
                {
                    yield return func;
                }
            }
        }
    }

    public class Scope
    {
        /// <summary>
        /// The name of the scope (used as the class name in .NET codegen).
        /// </summary>
        public string Name { get; }
        public Scope? Parent { get; }
        public List<Scope> Children { get; } = new();
        public Dictionary<string, BindingInfo> Bindings { get; } = new();
        // Names of parameters (for function scopes) so we can avoid generating backing fields for them.
        public HashSet<string> Parameters { get; } = new();
        public ScopeKind Kind { get; }
        public Node? AstNode { get; }

    /// <summary>
    /// Authoritative .NET namespace for this scope's generated type (if any).
    /// When null, generators may apply a default.
    /// </summary>
    public string? DotNetNamespace { get; set; }

    /// <summary>
    /// Authoritative .NET simple type name for this scope's generated type (if any).
    /// When null, generators may use <see cref="Name"/>.
    /// </summary>
    public string? DotNetTypeName { get; set; }

        public Scope(string name, ScopeKind kind, Scope? parent, Node? astNode = null)
        {
            Name = name;
            Kind = kind;
            Parent = parent;
            AstNode = astNode;
            Parent?.Children.Add(this);
        }
    }

    public enum ScopeKind
    {
        Global,
    Function,
    Block,
    Class
    }

    public class BindingInfo
    {
        public string Name { get; }
        public BindingKind Kind { get; }
        public Node DeclarationNode { get; }

        public BindingInfo(string name, BindingKind kind, Node declarationNode)
        {
            Name = name;
            Kind = kind;
            DeclarationNode = declarationNode;
        }
    }

    public enum BindingKind
    {
        Var,
        Let,
        Const,
        Function
    }
}
