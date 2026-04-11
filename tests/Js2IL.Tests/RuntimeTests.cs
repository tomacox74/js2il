using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using JavaScriptRuntime;

namespace Js2IL.Tests
{
    public class RuntimeTests
    {
        private readonly VerifySettings _verifySettings = new();

        public RuntimeTests()
        {
            _verifySettings.DisableDiff();
        }


        [Fact]
        public Task ToString_ObjectLiteral()
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            expandoObject["name"] = "Alice";
            expandoObject["age"] = 31;

            var result = DotNet2JSConversions.ToString(expandoObject);
            return VerifyWithSnapshot(result);
        }

        [Fact]
        public void ObjectGetOwnPropertyNames_NonGenericDictionary_IsDeterministic()
        {
            var table = new Hashtable
            {
                ["b"] = 1,
                ["10"] = 2,
                ["a"] = 3,
                ["2"] = 4
            };

            var names = Assert.IsType<JavaScriptRuntime.Array>(JavaScriptRuntime.Object.getOwnPropertyNames(table));
            Assert.Equal(new object?[] { "2", "10", "a", "b" }, names.ToArray());

            var keys = Assert.IsType<JavaScriptRuntime.Array>(JavaScriptRuntime.Object.keys(table));
            Assert.Equal(new object?[] { "2", "10", "a", "b" }, keys.ToArray());
        }

        private Task VerifyWithSnapshot(object value, [CallerFilePath] string sourceFilePath = "")
        {
            var settings = new VerifySettings(_verifySettings);
            var directory = Path.GetDirectoryName(sourceFilePath)!;
            var snapshotsDirectory = Path.Combine(directory, "Snapshots");
            Directory.CreateDirectory(snapshotsDirectory);
            settings.UseDirectory(snapshotsDirectory);
            return Verify(value, settings);
        }
    }
}
