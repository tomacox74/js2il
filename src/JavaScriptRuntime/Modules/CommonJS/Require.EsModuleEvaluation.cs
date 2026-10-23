using System.Reflection;
using Jroc.Runtime;
using JavaScriptRuntime.Modules.Shared;

namespace JavaScriptRuntime.Modules.CommonJS;

internal sealed partial class Require
{
    private Dictionary<string, JsCompiledEsModuleAttribute> GetEsModuleInformation()
        => _state.CompiledEsModuleInfo ??= _localModulesAssembly!
            .GetCustomAttributes<JsCompiledEsModuleAttribute>()
            .ToDictionary(info => NormalizeModuleIdKey(info.ModuleId), StringComparer.Ordinal);

    private object? RequireForEvaluation(string moduleId)
    {
        if (!TryResolveCompiledModule(moduleId, out var canonicalId, out var typeName))
            throw new ReferenceError($"Cannot find module '{moduleId}' while evaluating the compiled graph");
        return RequireCompiledModule(canonicalId, typeName, moduleId);
    }

    private bool HasAsynchronousEvaluation(string moduleId, HashSet<string> visited)
    {
        if (!visited.Add(moduleId) || !GetEsModuleInformation().TryGetValue(moduleId, out var info))
            return false;
        return info.HasTopLevelAwait || info.Dependencies.Where((_, index) =>
            index >= info.DependencyPhases.Length || !info.DependencyPhases[index].StartsWith("Type:", StringComparison.Ordinal))
            .Any(dependency => HasAsynchronousEvaluation(NormalizeModuleIdKey(dependency), visited));
    }

    private List<string> GetAsynchronousDependencies(string moduleId)
    {
        var result = new List<string>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var information = GetEsModuleInformation();
        void Gather(string id)
        {
            if (!visited.Add(id) || !information.TryGetValue(id, out var info))
                return;
            if (info.LinkError is { } linkError)
                throw new SyntaxError(linkError);
            if (info.ResolutionError is { } resolutionError)
                throw new ReferenceError(resolutionError);
            if (_modules.TryGetValue("compiled:" + id, out var module)
                && (module.loaded || module.IsSchedulingEvaluation || module.IsExecutingBody))
                return;
            if (info.HasTopLevelAwait)
            {
                result.Add(id);
                return;
            }
            for (var index = 0; index < info.Dependencies.Length; index++)
            {
                if (index < info.DependencyPhases.Length
                    && info.DependencyPhases[index].StartsWith("Type:", StringComparison.Ordinal))
                    continue;
                Gather(NormalizeModuleIdKey(info.Dependencies[index]));
            }
        }
        Gather(moduleId);
        return result;
    }

    private List<string> GetEvaluationDependencies(JsCompiledEsModuleAttribute info)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        for (var index = 0; index < info.Dependencies.Length; index++)
        {
            var phase = index < info.DependencyPhases.Length ? info.DependencyPhases[index] : "None";
            if (phase.StartsWith("Type:", StringComparison.Ordinal))
                continue;
            var dependency = NormalizeModuleIdKey(info.Dependencies[index]);
            foreach (var id in phase == "Defer" ? GetAsynchronousDependencies(dependency) : [dependency])
            {
                if (seen.Add(id))
                    result.Add(id);
            }
        }
        return result;
    }

    private bool IsReadyForSynchronousEvaluation(string moduleId, HashSet<string> visited)
    {
        if (!visited.Add(moduleId))
            return true;
        if (_modules.TryGetValue("compiled:" + moduleId, out var module))
            return module.loaded || module.EvaluationError is not null;
        if (!GetEsModuleInformation().TryGetValue(moduleId, out var info))
            return true;
        if (info.HasTopLevelAwait)
            return false;
        for (var index = 0; index < info.Dependencies.Length; index++)
        {
            if (index < info.DependencyPhases.Length
                && info.DependencyPhases[index].StartsWith("Type:", StringComparison.Ordinal))
                continue;
            if (!IsReadyForSynchronousEvaluation(NormalizeModuleIdKey(info.Dependencies[index]), visited))
                return false;
        }
        return true;
    }

    internal bool TryExecuteNativeModule(Module module, Action initializer)
    {
        var id = NormalizeModuleIdKey(module.id);
        if (!GetEsModuleInformation().TryGetValue(id, out var info) || !info.IsNativeEsModule
            || !HasAsynchronousEvaluation(id, new HashSet<string>(StringComparer.Ordinal)))
            return false;

        module.EvaluationPromise = Promise.CreateModuleEvaluation();
        module.IsSchedulingEvaluation = true;
        ESM.EsModuleLinker.PredeclareExports(module.exports, id, info.ExportNames);

        void Complete()
        {
            module.MarkLoaded();
            module.EvaluationPromise.CompleteModuleEvaluation(null, rejected: false);
        }

        void Fail(Exception error)
        {
            if (module.EvaluationError is not null)
                return;
            module.EvaluationError = error;
            module.EvaluationPromise.CompleteModuleEvaluation(Promise.GetThrownJsValue(error), rejected: true);
        }

        void RunBody(bool propagate)
        {
            var previousParent = _currentParentModule;
            try
            {
                _currentParentModule = module;
                module.IsExecutingBody = true;
                initializer();
                if (module.AsyncBodyPromise is not { } bodyPromise)
                    Complete();
                else if (bodyPromise.TryGetModuleEvaluationResult(out var rejected, out var value))
                {
                    if (rejected)
                        throw new JsThrownValueException(value);
                    Complete();
                }
                else
                    bodyPromise.ObserveModuleEvaluation(_ => Complete(), reason => Fail(new JsThrownValueException(reason)));
            }
            catch (Exception error) when (error is not ScriptProcessExitException)
            {
                Fail(error);
                if (propagate)
                    throw;
            }
            finally
            {
                module.IsExecutingBody = false;
                _currentParentModule = previousParent;
            }
        }

        try
        {
            var pending = new List<Promise>();
            foreach (var dependency in GetEvaluationDependencies(info))
            {
                RequireForEvaluation(dependency);
                if (!_modules.TryGetValue("compiled:" + dependency, out var dependencyModule)
                    || dependencyModule.IsSchedulingEvaluation
                    || dependencyModule.EvaluationPromise is not { } evaluation)
                    continue;
                if (!evaluation.TryGetModuleEvaluationResult(out var rejected, out var reason))
                    pending.Add(evaluation);
                else if (rejected)
                    throw new JsThrownValueException(reason);
            }
            if (pending.Count == 0)
                RunBody(propagate: true);
            else
            {
                var remaining = pending.Count;
                foreach (var evaluation in pending)
                    evaluation.ObserveModuleEvaluation(
                        _ => { if (--remaining == 0 && module.EvaluationError is null) RunBody(propagate: false); },
                        reason => Fail(new JsThrownValueException(reason)));
            }
        }
        catch (Exception error) when (error is not ScriptProcessExitException)
        {
            Fail(error);
            throw;
        }
        finally
        {
            module.IsSchedulingEvaluation = false;
        }
        return true;
    }
}
