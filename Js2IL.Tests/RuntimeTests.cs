using System;
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
