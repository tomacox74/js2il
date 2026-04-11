using System;
using IOPath = System.IO.Path;
using NodePath = JavaScriptRuntime.Node.Path;
using Xunit;

namespace Js2IL.Tests.Node.Path
{
    public class PathTests
    {
        [Fact]
        public void Join_Basic_ReturnsCombined()
        {
            var p = new NodePath();
            var result = p.join("usr", "local", "bin");
            Assert.Equal(IOPath.Combine("usr", "local", "bin"), result);
        }

        [Fact]
        public void Join_TreatsNullAsEmpty()
        {
            var p = new NodePath();
            var result = p.join(new object[] { "a", null!, "b" });
            Assert.Equal(IOPath.Combine("a", string.Empty, "b"), result);
        }

        [Fact]
        public void Join_WithTrailingSeparators_Normalized()
        {
            var p = new NodePath();
            var result = p.join($"a{IOPath.DirectorySeparatorChar}", "b", "c.txt");
            Assert.Equal(IOPath.Combine($"a{IOPath.DirectorySeparatorChar}", "b", "c.txt"), result);
        }

        [Fact]
        public void Join_WithRootedPath_ResetsBase()
        {
            var rooted = IOPath.Combine(IOPath.GetPathRoot(IOPath.GetFullPath(".")) ?? string.Empty, "root");
            var p = new NodePath();
            var result = p.join("ignored", rooted, "file.txt");
            Assert.Equal(IOPath.Combine("ignored", rooted, "file.txt"), result);
        }

        [Fact]
        public void Join_NoArgs_ReturnsDot()
        {
            var p = new NodePath();
            var result = p.join();
            Assert.Equal(".", result);
        }

        [Fact]
        public void Resolve_Basic_ReturnsAbsoluteNormalized()
        {
            var p = new NodePath();
            var result = p.resolve("a", "b", "c.txt");
            var expected = IOPath.GetFullPath(IOPath.Combine("a", "b", "c.txt"));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Resolve_LastArgAbsolute_ResetsBase()
        {
            var root = IOPath.GetPathRoot(IOPath.GetFullPath(".")) ?? string.Empty;
            var absolute = IOPath.Combine(root, "absbase");
            var p = new NodePath();
            var result = p.resolve("ignored", absolute, "file.txt");
            var expected = IOPath.GetFullPath(IOPath.Combine("ignored", absolute, "file.txt"));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Resolve_NoArgs_ReturnsCwd()
        {
            var p = new NodePath();
            var result = p.resolve();
            var expected = IOPath.GetFullPath(".");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Resolve_TreatsNullAsEmpty()
        {
            var p = new NodePath();
            var result = p.resolve("a", null!, "b");
            var expected = IOPath.GetFullPath(IOPath.Combine("a", string.Empty, "b"));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Relative_Basic()
        {
            var cwd = IOPath.GetFullPath(".");
            var from = IOPath.Combine(cwd, "a", "b");
            var to = IOPath.Combine(cwd, "a", "b", "c", "d.txt");
            var p = new NodePath();
            var result = p.relative(from, to);
            var expected = IOPath.GetRelativePath(from, to);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Relative_SamePath_ReturnsEmptyString()
        {
            var cwd = IOPath.GetFullPath(".");
            var p = new NodePath();
            var result = p.relative(cwd, cwd);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Relative_TreatsNullAsDot()
        {
            var cwd = IOPath.GetFullPath(".");
            var sub = IOPath.Combine(cwd, "sub");
            var p = new NodePath();
            var result = p.relative(null!, sub);
            var expected = IOPath.GetRelativePath(".", sub);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Delimiter_ExposesPlatformPathSeparator()
        {
            var p = new NodePath();
            Assert.Equal(IOPath.PathSeparator.ToString(), p.delimiter);
        }

        [Fact]
        public void ToNamespacedPath_ReturnsInputForCurrentCompatibilityMode()
        {
            var p = new NodePath();
            Assert.Equal("/tmp/demo", p.toNamespacedPath("/tmp/demo"));
            Assert.Equal("relative/path", p.toNamespacedPath("relative/path"));
        }
    }
}
