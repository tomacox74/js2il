using System;
using System.Collections.Generic;
using System.Linq;
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
            return Verify(result, _verifySettings);
        }
    }
}
