using System;
using Js2IL;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests;

public class ModuleLoaderTests
{
    [Fact]
    public void LoadModules_ParseError_ReturnsNullAndLogsErrors()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var modulePath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "bad.js"));
        fileSystem.AddFile(modulePath, "while (false) { notALoop: { continue notALoop; } }");

        var modules = loader.LoadModules(modulePath);

        Assert.Null(modules);
        Assert.Contains("Parse Errors", logger.Errors);
        Assert.Contains("does not denote an iteration statement", logger.Errors);
    }

    [Fact]
    public void LoadModules_DependencyMissingUseStrict_ReturnsNullAndLogsErrors()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var rootPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "root.js"));
        var depPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "dep.js"));

        fileSystem.AddFile(rootPath, "\"use strict\";\nconst d = require('./dep');\nconsole.log(d);\n");
        fileSystem.AddFile(depPath, "module.exports = 1;\n");

        var modules = loader.LoadModules(rootPath);

        Assert.Null(modules);
        Assert.Contains("Validation Errors", logger.Errors);
        Assert.Contains("requires strict mode", logger.Errors, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LoadModules_MultipleDependenciesMissingUseStrict_ReturnsNullAndLogsAllErrorsWithModuleNames()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var rootPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "root.js"));
        var depAPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "depA.js"));
        var depBPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "depB.js"));

        fileSystem.AddFile(rootPath, "\"use strict\";\nconst a = require('./depA');\nconst b = require('./depB');\nconsole.log(a, b);\n");
        fileSystem.AddFile(depAPath, "module.exports = 1;\n");
        fileSystem.AddFile(depBPath, "module.exports = 2;\n");

        var modules = loader.LoadModules(rootPath);

        Assert.Null(modules);
        Assert.Contains("Validation Errors", logger.Errors);

        // Both dependencies should be reported (fail-fast hides one of these).
        Assert.Contains("requires strict mode", logger.Errors, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(Path.GetFileName(depAPath), logger.Errors, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(Path.GetFileName(depBPath), logger.Errors, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LoadModules_DependencyMissingUseStrict_StrictModeWarn_LoadsAndLogsWarning()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false, StrictMode = StrictModeDirectivePrologueMode.Warn };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var rootPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "root.js"));
        var depPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "dep.js"));

        fileSystem.AddFile(rootPath, "\"use strict\";\nconst d = require('./dep');\nconsole.log(d);\n");
        fileSystem.AddFile(depPath, "module.exports = 1;\n");

        var modules = loader.LoadModules(rootPath);

        Assert.NotNull(modules);
        Assert.True(string.IsNullOrWhiteSpace(logger.Errors));
        Assert.Contains("requires strict mode", logger.Warnings, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LoadModules_RequestResolutionFailures_ReportAllErrors_AndStillWalkResolvedDependencies()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var rootPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "root.js"));
        var okPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "ok.js"));

        fileSystem.AddFile(
            rootPath,
            "\"use strict\";\n"
            + "require('./ok');\n"
            + "require('./missingA');\n"
            + "require('./missingB');\n"
        );
        fileSystem.AddFile(okPath, "module.exports = 1;\n");

        var modules = loader.LoadModules(rootPath);

        Assert.Null(modules);
        Assert.Contains("require('./missingA')", logger.Errors);
        Assert.Contains("require('./missingB')", logger.Errors);
        Assert.Contains(Path.GetFileName(okPath), logger.Errors, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("requires strict mode", logger.Errors, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LoadModules_StaticModuleSyntax_CreatesExplicitModuleRecordMetadata()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var rootPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "root.mjs"));
        var depPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "dep.mjs"));

        fileSystem.AddFile(rootPath, """
"use strict";
import { value as importedValue } from "./dep.mjs";
export { importedValue as renamed };
""");
        fileSystem.AddFile(depPath, """
"use strict";
export const value = 1;
""");

        var modules = loader.LoadModules(rootPath);

        Assert.NotNull(modules);
        var record = modules!.rootModule.ModuleRecord;
        Assert.NotNull(record);
        Assert.Contains(record!.RequestedModules, request => request.Specifier == "./dep.mjs");
        Assert.Contains(record.ImportEntries, entry =>
            entry.Kind == ModuleImportKind.Named
            && entry.ModuleRequest == "./dep.mjs"
            && entry.LocalName == "importedValue"
            && entry.ImportName == "value");
        Assert.Contains(record.LocalExportEntries, entry =>
            entry.Kind == ModuleExportKind.Local
            && entry.ExportName == "renamed"
            && entry.LocalName == "importedValue");
    }

    [Fact]
    public void LoadModules_ModuleRecordRequestedModules_ExcludeRuntimeOnlyDependencies()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var rootPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "root.mjs"));
        var esmPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "esm.mjs"));
        var reExportPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "reexport.mjs"));
        var cjsPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "dep.cjs"));
        var dynamicPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "dynamic.mjs"));

        fileSystem.AddFile(rootPath, """
"use strict";
import { value } from "./esm.mjs";
export { value };
export * from "./reexport.mjs";
const dep = require("./dep.cjs");
void dep;
async function loadLater() {
    return import("./dynamic.mjs");
}
""");
        fileSystem.AddFile(esmPath, """
"use strict";
export const value = 1;
""");
        fileSystem.AddFile(reExportPath, """
"use strict";
export const other = 2;
""");
        fileSystem.AddFile(cjsPath, """
"use strict";
module.exports = { runtimeOnly: true };
""");
        fileSystem.AddFile(dynamicPath, """
"use strict";
export const later = 3;
""");

        var modules = loader.LoadModules(rootPath);

        Assert.NotNull(modules);
        var record = modules!.rootModule.ModuleRecord;
        Assert.NotNull(record);
        Assert.Equal(2, record!.RequestedModules.Count);
        Assert.Contains(record.RequestedModules, request => request.Specifier == "./esm.mjs");
        Assert.Contains(record.RequestedModules, request => request.Specifier == "./reexport.mjs");
        Assert.DoesNotContain(record.RequestedModules, request => request.Specifier == "./dep.cjs");
        Assert.DoesNotContain(record.RequestedModules, request => request.Specifier == "./dynamic.mjs");
        Assert.Contains(modules.rootModule.Dependencies, dependency => dependency.Request == "./dep.cjs");
        Assert.Contains(modules.rootModule.Dependencies, dependency => dependency.Request == "./dynamic.mjs");
    }

    [Fact]
    public void LinkModules_CyclicGraph_TracksLinkAndEvaluationPhases()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var rootPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "main.mjs"));
        var aPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "a.mjs"));
        var bPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "b.mjs"));

        fileSystem.AddFile(rootPath, """
"use strict";
import { aValue } from "./a.mjs";
console.log(aValue);
""");
        fileSystem.AddFile(aPath, """
"use strict";
import { bValue } from "./b.mjs";
export const aValue = "a";
export function readB() { return bValue; }
""");
        fileSystem.AddFile(bPath, """
"use strict";
import { aValue } from "./a.mjs";
export const bValue = "b";
export function readA() { return aValue; }
""");

        var modules = loader.LoadModules(rootPath);

        Assert.NotNull(modules);
        Assert.True(loader.LinkModules(modules!, logger));
        Assert.True(loader.PlanModuleEvaluation(modules!, logger));

        var aModule = modules!._modules[aPath];
        var bModule = modules._modules[bPath];
        Assert.Equal(ModuleLinkPhase.Linked, aModule.ModuleRecord!.LinkPhase);
        Assert.Equal(ModuleLinkPhase.Linked, bModule.ModuleRecord!.LinkPhase);
        Assert.Equal(ModuleEvaluationPhase.Planned, aModule.ModuleRecord.EvaluationPhase);
        Assert.Equal(ModuleEvaluationPhase.Planned, bModule.ModuleRecord.EvaluationPhase);
        Assert.Equal(aModule.ModuleRecord.EvaluationComponent, bModule.ModuleRecord.EvaluationComponent);
    }

    [Fact]
    public void PlanModuleEvaluation_UsesStaticModuleGraphForOrderAndComponents()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var resolver = new NodeModuleResolver(fileSystem);
        var loader = new ModuleLoader(options, fileSystem, resolver, logger);

        var rootPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "main.mjs"));
        var aPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "a.mjs"));
        var bPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "b.mjs"));
        var cPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "c.mjs"));
        var runtimePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rootPath)!, "runtime.cjs"));

        fileSystem.AddFile(rootPath, """
"use strict";
import { aValue } from "./a.mjs";
const runtime = require("./runtime.cjs");
void runtime;
console.log(aValue);
""");
        fileSystem.AddFile(aPath, """
"use strict";
import { bValue } from "./b.mjs";
export const aValue = "a";
export function readB() { return bValue; }
""");
        fileSystem.AddFile(bPath, """
"use strict";
import { cValue } from "./c.mjs";
export const bValue = "b";
export function readC() { return cValue; }
""");
        fileSystem.AddFile(cPath, """
"use strict";
import { aValue } from "./a.mjs";
export const cValue = aValue;
""");
        fileSystem.AddFile(runtimePath, """
"use strict";
module.exports = { runtimeOnly: true };
""");

        var modules = loader.LoadModules(rootPath);

        Assert.NotNull(modules);
        Assert.True(loader.LinkModules(modules!, logger));
        Assert.True(loader.PlanModuleEvaluation(modules!, logger));

        var rootModule = modules!.rootModule;
        var aModule = modules._modules[aPath];
        var bModule = modules._modules[bPath];
        var cModule = modules._modules[cPath];
        var runtimeModule = modules._modules[runtimePath];

        Assert.Equal(ModuleEvaluationPhase.Planned, rootModule.ModuleRecord!.EvaluationPhase);
        Assert.Equal(ModuleEvaluationPhase.Planned, aModule.ModuleRecord!.EvaluationPhase);
        Assert.Equal(ModuleEvaluationPhase.Planned, bModule.ModuleRecord!.EvaluationPhase);
        Assert.Equal(ModuleEvaluationPhase.Planned, cModule.ModuleRecord!.EvaluationPhase);
        Assert.Equal(ModuleEvaluationPhase.Planned, runtimeModule.ModuleRecord!.EvaluationPhase);

        Assert.Equal(aModule.ModuleRecord.EvaluationComponent, bModule.ModuleRecord.EvaluationComponent);
        Assert.Equal(bModule.ModuleRecord.EvaluationComponent, cModule.ModuleRecord.EvaluationComponent);
        Assert.NotEqual(rootModule.ModuleRecord.EvaluationComponent, aModule.ModuleRecord.EvaluationComponent);
        Assert.NotEqual(runtimeModule.ModuleRecord.EvaluationComponent, rootModule.ModuleRecord.EvaluationComponent);
        Assert.Equal(0, aModule.ModuleRecord.EvaluationOrder);
        Assert.Equal(0, bModule.ModuleRecord.EvaluationOrder);
        Assert.Equal(0, cModule.ModuleRecord.EvaluationOrder);
        Assert.Equal(1, rootModule.ModuleRecord.EvaluationOrder);
        Assert.True(rootModule.ModuleRecord.EvaluationOrder < runtimeModule.ModuleRecord.EvaluationOrder);
    }
}
