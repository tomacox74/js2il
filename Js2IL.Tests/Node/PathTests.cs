using System;
using IOPath = System.IO.Path;
using NodePath = JavaScriptRuntime.Node.Path;
using Xunit;

namespace Js2IL.Tests.Node
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
        public void Join_NoArgs_FollowsCombineBehavior()
        {
            var p = new NodePath();
            var result = p.join();
            Assert.Equal(string.Empty, result);
        }
    }
}
