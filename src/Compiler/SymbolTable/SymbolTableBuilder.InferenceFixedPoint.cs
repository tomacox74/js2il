using System.Text;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private void InferTypesToFixedPoint(Scope root)
    {
        var visitedStates = new HashSet<string>(StringComparer.Ordinal);
        var before = CaptureInferenceState(root);
        visitedStates.Add(before);

        while (true)
        {
            // Parameter facts are recomputed first so variable inference can restore derived
            // metadata such as stable Array element types before return inference consumes it.
            InferCallableParameterClrTypes(root);

            // Definite-initialization runs immediately after variable inference so transient
            // guesses from hoisted var initializers cannot become field or return facts.
            InferVariableClrTypes(root);
            InferDefinitelyInitializedNumericVarLocals(root);
            InferClassInstanceFieldClrTypes(root);
            InferCallableReturnClrTypes(root);
            AnalyzeObjectLiteralShapes(root);

            var after = CaptureInferenceState(root);
            if (string.Equals(before, after, StringComparison.Ordinal))
            {
                return;
            }

            if (!visitedStates.Add(after))
            {
                throw new InvalidOperationException("Type inference entered a non-converging cycle.");
            }

            before = after;
        }
    }

    private static string CaptureInferenceState(Scope root)
    {
        var state = new StringBuilder();
        var scopeIndex = 0;

        foreach (var scope in EnumerateScopes(root))
        {
            state.Append('S').Append(scopeIndex++).Append(':');
            AppendType(state, scope.StableReturnClrType);
            AppendType(state, scope.StableReturnArrayElementClrType);
            state.Append(scope.StableReturnIsThis ? '1' : '0').Append(';');

            foreach (var (index, type) in scope.StableParameterClrTypes.OrderBy(pair => pair.Key))
            {
                state.Append('P').Append(index).Append(':');
                AppendType(state, type);
            }

            foreach (var (name, type) in scope.StableInstanceFieldClrTypes.OrderBy(pair => pair.Key, StringComparer.Ordinal))
            {
                state.Append('F');
                AppendText(state, name);
                AppendType(state, type);
            }

            foreach (var (name, className) in scope.StableInstanceFieldUserClassNames.OrderBy(pair => pair.Key, StringComparer.Ordinal))
            {
                state.Append('U');
                AppendText(state, name);
                AppendText(state, className);
            }

            foreach (var (name, binding) in scope.Bindings.OrderBy(pair => pair.Key, StringComparer.Ordinal))
            {
                state.Append('B');
                AppendText(state, name);
                AppendType(state, binding.ClrType);
                AppendType(state, binding.StableElementClrType);
                state.Append(binding.IsStableType ? '1' : '0');
                state.Append(binding.CanUseUnboxedLocal ? '1' : '0');

                if (binding.ObjectLiteralShape is not { } shape)
                {
                    state.Append('0');
                    continue;
                }

                state.Append('1');
                state.Append(shape.IsEligible ? '1' : '0');
                AppendText(state, shape.DisqualifyReason);
                AppendText(state, shape.GetStructuralSignatureKey());
            }
        }

        return state.ToString();
    }

    private static void AppendType(StringBuilder state, Type? type)
        => AppendText(state, type?.AssemblyQualifiedName);

    private static void AppendText(StringBuilder state, string? value)
    {
        if (value == null)
        {
            state.Append("-1:");
            return;
        }

        state.Append(value.Length).Append(':').Append(value);
    }
}
