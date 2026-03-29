using System.Threading.Tasks;

namespace Js2IL.Tests.Set
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Set") { }

        [Fact]
        public Task Set_Constructor_Prototype_Surface() { var testName = nameof(Set_Constructor_Prototype_Surface); return ExecutionTest(testName); }

        [Fact]
        public Task Set_Constructor_Iterable() { var testName = nameof(Set_Constructor_Iterable); return ExecutionTest(testName); }

        [Fact]
        public Task Set_Core_Methods() { var testName = nameof(Set_Core_Methods); return ExecutionTest(testName); }

        [Fact]
        public Task Set_Entries_Keys_Values() { var testName = nameof(Set_Entries_Keys_Values); return ExecutionTest(testName); }

        [Fact]
        public Task Set_ForEach_Basic() { var testName = nameof(Set_ForEach_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Set_Symbol_Iterator() { var testName = nameof(Set_Symbol_Iterator); return ExecutionTest(testName); }

        [Fact]
        public Task Set_Algebra_Methods() { var testName = nameof(Set_Algebra_Methods); return ExecutionTest(testName); }
    }
}
