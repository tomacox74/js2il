using System.Threading.Tasks;

namespace Js2IL.Tests.Array
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Array") { }

        // Array execution tests
        [Fact]
        public Task Array_LengthProperty_ReturnsCount() { var testName = nameof(Array_LengthProperty_ReturnsCount); return ExecutionTest(testName); }

        [Fact]
        public Task Array_EmptyLength_IsZero() { var testName = nameof(Array_EmptyLength_IsZero); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Sort_Basic() { var testName = nameof(Array_Sort_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Map_Basic() { var testName = nameof(Array_Map_Basic); return ExecutionTest(testName); }
    }
}
