namespace Jroc.Test262.Tests.language.import.import_bytes;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_bytes") { }

    [Fact(DisplayName = "bytes-from-empty.js")]
    public Task bytes_from_empty()
        => ExecutionTest("bytes-from-empty");

    [Fact(DisplayName = "bytes-from-js.js")]
    public Task bytes_from_js()
        => ExecutionTest("bytes-from-js");

    [Fact(DisplayName = "bytes-from-json.js")]
    public Task bytes_from_json()
        => ExecutionTest("bytes-from-json");

    [Fact(DisplayName = "bytes-from-png.js")]
    public Task bytes_from_png()
        => ExecutionTest("bytes-from-png");

    [Fact(DisplayName = "bytes-from-txt.js")]
    public Task bytes_from_txt()
        => ExecutionTest("bytes-from-txt");
}
