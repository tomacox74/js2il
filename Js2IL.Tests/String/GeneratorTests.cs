using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.String
{
    public class GeneratorTests : GeneratorTestsBase
    {
    public GeneratorTests() : base("String") { }
        [Fact]
        public Task String_LocaleCompare_Numeric()
        {
            var testName = nameof(String_LocaleCompare_Numeric);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_LastIndexOf_Basic()
        {
            var testName = nameof(String_LastIndexOf_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_PlusEquals_Append()
        {
            var testName = nameof(String_PlusEquals_Append);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Replace_CallOnExpression()
        {
            var testName = nameof(String_Replace_CallOnExpression);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Replace_Regex_Global()
        {
            var testName = nameof(String_Replace_Regex_Global);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Match_NonGlobal()
        {
            var testName = nameof(String_Match_NonGlobal);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Match_Global()
        {
            var testName = nameof(String_Match_Global);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Search_Basic()
        {
            var testName = nameof(String_Search_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_RegExp_Exec_LastIndex_Global()
        {
            var testName = nameof(String_RegExp_Exec_LastIndex_Global);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_RegExp_Exec_LastIndex_EmptyMatch_Global()
        {
            var testName = nameof(String_RegExp_Exec_LastIndex_EmptyMatch_Global);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Substring()
        {
            var testName = nameof(String_Substring);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Split_Basic()
        {
            var testName = nameof(String_Split_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_StartsWith_Basic()
        {
            var testName = nameof(String_StartsWith_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_StartsWith_NestedParam()
        {
            var testName = nameof(String_StartsWith_NestedParam);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_TemplateLiteral_Basic()
        {
            var testName = nameof(String_TemplateLiteral_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_New_Sugar()
        {
            var testName = nameof(String_New_Sugar);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_CharCodeAt_Basic()
        {
            var testName = nameof(String_CharCodeAt_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_FromCharCode_Basic()
        {
            var testName = nameof(String_FromCharCode_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_ToLowerCase_ToUpperCase_Basic()
        {
            var testName = nameof(String_ToLowerCase_ToUpperCase_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_TaggedTemplate_Basic()
        {
            var testName = nameof(String_TaggedTemplate_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_TaggedTemplate_RawStrings()
        {
            var testName = nameof(String_TaggedTemplate_RawStrings);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_TaggedTemplate_EvaluationOrder()
        {
            var testName = nameof(String_TaggedTemplate_EvaluationOrder);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_TaggedTemplate_NoSubstitutions()
        {
            var testName = nameof(String_TaggedTemplate_NoSubstitutions);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_MemberCall_Arity2_Substring() { var testName = nameof(String_MemberCall_Arity2_Substring); return GenerateTest(testName); }

        [Fact]
        public Task String_MemberCall_Arity3_Replace() { var testName = nameof(String_MemberCall_Arity3_Replace); return GenerateTest(testName); }

        [Fact]
        public Task String_MemberCall_FastPath_CommonMethods()
        {
            var testName = nameof(String_MemberCall_FastPath_CommonMethods);
            return GenerateTest(testName);
        }
    }
}
