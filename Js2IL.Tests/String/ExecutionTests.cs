using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.String
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("String") {}

        [Fact]
        public Task String_LocaleCompare_Numeric()
        {
            return ExecutionTest(nameof(String_LocaleCompare_Numeric));
        }

        [Fact]
        public Task String_PlusEquals_Append()
        {
            return ExecutionTest(nameof(String_PlusEquals_Append));
        }

        [Fact]
        public Task String_Replace_CallOnExpression()
        {
            // Repro for calling a member on a non-identifier receiver: (String('...')).replace(...)
            return ExecutionTest(nameof(String_Replace_CallOnExpression));
        }

        [Fact]
        public Task String_Replace_Regex_Global()
        {
            // Uses embedded resource JavaScript/String_Replace_Regex_Global.js
            return ExecutionTest(nameof(String_Replace_Regex_Global));
        }

        [Fact]
        public Task String_Match_NonGlobal()
        {
            return ExecutionTest(nameof(String_Match_NonGlobal));
        }

        [Fact]
        public Task String_Match_Global()
        {
            return ExecutionTest(nameof(String_Match_Global));
        }

        [Fact]
        public Task String_RegExp_Exec_LastIndex_Global()
        {
            return ExecutionTest(nameof(String_RegExp_Exec_LastIndex_Global));
        }

        [Fact]
        public Task String_RegExp_Exec_LastIndex_EmptyMatch_Global()
        {
            return ExecutionTest(nameof(String_RegExp_Exec_LastIndex_EmptyMatch_Global));
        }

        [Fact]
        public Task String_Substring()
        {
            return ExecutionTest(nameof(String_Substring));
        }

        [Fact]
        public Task String_Split_Basic()
        {
            return ExecutionTest(nameof(String_Split_Basic));
        }

        [Fact]
        public Task String_StartsWith_Basic()
        {
            return ExecutionTest(nameof(String_StartsWith_Basic));
        }

        [Fact]
        public Task String_StartsWith_NestedParam()
        {
            // Nested function calls startsWith on a string parameter (slow path via Object.CallMember)
            return ExecutionTest(nameof(String_StartsWith_NestedParam));
        }

        [Fact]
        public Task String_TemplateLiteral_Basic()
        {
            return ExecutionTest(nameof(String_TemplateLiteral_Basic));
        }

        [Fact]
        public Task String_New_Sugar()
        {
            var testName = nameof(String_New_Sugar);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task String_CharCodeAt_Basic()
        {
            return ExecutionTest(nameof(String_CharCodeAt_Basic));
        }

        [Fact]
        public Task String_ToLowerCase_ToUpperCase_Basic()
        {
            return ExecutionTest(nameof(String_ToLowerCase_ToUpperCase_Basic));
        }

        [Fact]
        public Task String_TaggedTemplate_Basic()
        {
            return ExecutionTest(nameof(String_TaggedTemplate_Basic));
        }

        [Fact]
        public Task String_TaggedTemplate_RawStrings()
        {
            return ExecutionTest(nameof(String_TaggedTemplate_RawStrings));
        }

        [Fact]
        public Task String_TaggedTemplate_EvaluationOrder()
        {
            return ExecutionTest(nameof(String_TaggedTemplate_EvaluationOrder));
        }

        [Fact]
        public Task String_TaggedTemplate_NoSubstitutions()
        {
            return ExecutionTest(nameof(String_TaggedTemplate_NoSubstitutions));
        }

        [Fact]
        public Task String_MemberCall_Arity2_Substring() { var testName = nameof(String_MemberCall_Arity2_Substring); return ExecutionTest(testName); }

        [Fact]
        public Task String_MemberCall_Arity3_Replace() { var testName = nameof(String_MemberCall_Arity3_Replace); return ExecutionTest(testName); }

        [Fact]
        public Task String_MemberCall_FastPath_CommonMethods()
        {
            return ExecutionTest(nameof(String_MemberCall_FastPath_CommonMethods));
        }
    }
}
