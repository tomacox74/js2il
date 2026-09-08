using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.reserved_words;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/reserved-words", "language.reserved_words") { }

    [Fact(DisplayName = "await-script.js")]
    public Task await_script()
        => ExecutionTest("await-script");

    [Fact(DisplayName = "ident-name-keyword-accessor.js")]
    public Task ident_name_keyword_accessor()
        => ExecutionTest("ident-name-keyword-accessor");

    [Fact(DisplayName = "ident-name-keyword-memberexpr-str.js")]
    public Task ident_name_keyword_memberexpr_str()
        => ExecutionTest("ident-name-keyword-memberexpr-str");

    [Fact(DisplayName = "ident-name-keyword-memberexpr.js")]
    public Task ident_name_keyword_memberexpr()
        => ExecutionTest("ident-name-keyword-memberexpr");

    [Fact(DisplayName = "ident-name-keyword-prop-name.js")]
    public Task ident_name_keyword_prop_name()
        => ExecutionTest("ident-name-keyword-prop-name");

    [Fact(DisplayName = "unreserved-words.js")]
    public Task unreserved_words()
        => ExecutionTest("unreserved-words");
}
