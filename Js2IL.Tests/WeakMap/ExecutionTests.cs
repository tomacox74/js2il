using System.Threading.Tasks;

namespace Js2IL.Tests.WeakMap
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("WeakMap") { }

        [Fact]
        public Task WeakMap_Constructor_Empty() { var testName = nameof(WeakMap_Constructor_Empty); return ExecutionTest(testName); }

        [Fact]
        public Task WeakMap_Set_Get_Basic() { var testName = nameof(WeakMap_Set_Get_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task WeakMap_Has_Basic() { var testName = nameof(WeakMap_Has_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task WeakMap_Delete_Basic() { var testName = nameof(WeakMap_Delete_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task WeakMap_Object_Keys() { var testName = nameof(WeakMap_Object_Keys); return ExecutionTest(testName); }
    }
}
