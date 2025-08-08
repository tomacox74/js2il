using Acornima.Ast;
using System.Collections.Generic;

namespace Js2IL.Scoping
{
    /// <summary>
    /// Represents a lexical scope in the JavaScript source code.
    /// Each scope contains its own bindings and references to child scopes.
    /// </summary>
    public class ScopeTree
    {
        public ScopeNode Root { get; }

        public ScopeTree(ScopeNode root)
        {
            Root = root;
        }
    }

    public class ScopeNode
    {
        /// <summary>
        /// The name of the scope (used as the class name in .NET codegen).
        /// </summary>
        public string Name { get; }
        public ScopeNode? Parent { get; }
        public List<ScopeNode> Children { get; } = new();
        public Dictionary<string, BindingInfo> Bindings { get; } = new();
    // Names of parameters (for function scopes) so we can avoid generating backing fields for them.
    public HashSet<string> Parameters { get; } = new();
        public ScopeKind Kind { get; }
        public Node? AstNode { get; }

        public ScopeNode(string name, ScopeKind kind, ScopeNode? parent, Node? astNode = null)
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
        Block
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
