using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Jroc.Tests.Integration;

public class CheerioGeneratorTests
{
    [Fact]
    public void Compile_Cheerio_1_2_0()
    {
        var fixtureDirectory = Path.Combine(
            FindRepositoryRoot(),
            "tests",
            "Jroc.Tests",
            "Integration",
            "Fixtures",
            "Cheerio");

        var root = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            nameof(Compile_Cheerio_1_2_0),
            Guid.NewGuid().ToString("N"));
        var outputDirectory = Path.Combine(root, "out");

        try
        {
            Directory.CreateDirectory(root);
            File.Copy(
                Path.Combine(fixtureDirectory, "package.json"),
                Path.Combine(root, "package.json"));
            File.Copy(
                Path.Combine(fixtureDirectory, "package-lock.json"),
                Path.Combine(root, "package-lock.json"));
            File.WriteAllText(
                Path.Combine(root, "main.js"),
                "\"use strict\";\n" +
                "const cheerio = require(\"cheerio\");\n" +
                "const $ = cheerio.load(\"<p>test</p>\");\n" +
                "console.log($(\"p\").text());\n");

            InstallFixture(root);

            var logger = new TestLogger();
            var options = new CompilerOptions
            {
                OutputDirectory = outputDirectory,
                EmitPdb = true
            };
            using var serviceProvider = CompilerServices.BuildServiceProvider(
                options,
                fileSystem: new FileSystem(),
                compilerOutput: logger);
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            Assert.True(
                compiler.Compile(Path.Combine(root, "main.js")),
                $"Cheerio 1.2.0 compilation failed.{Environment.NewLine}{logger.Errors}");
            Assert.True(
                File.Exists(Path.Combine(outputDirectory, "main.dll")),
                "Expected JROC to emit the compiled Cheerio entry assembly.");
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    private static void InstallFixture(string root)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "npm",
            Arguments = "ci --ignore-scripts --no-audit --no-fund",
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start npm.");
        if (!process.WaitForExit(120_000))
        {
            process.Kill();
            throw new TimeoutException("Timed out installing the Cheerio generator fixture.");
        }

        var standardOutput = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();
        Assert.True(
            process.ExitCode == 0,
            $"npm ci failed with exit code {process.ExitCode}.{Environment.NewLine}" +
            $"STDOUT:{Environment.NewLine}{standardOutput}{Environment.NewLine}" +
            $"STDERR:{Environment.NewLine}{standardError}");
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(
            Path.GetDirectoryName(typeof(CheerioGeneratorTests).Assembly.Location)!);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "jroc.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate the JROC repository root.");
    }
}
