using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.identifiers;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/identifiers", "language.identifiers") { }

    [Fact(DisplayName = "other_id_continue-escaped.js")]
    public Task other_id_continue_escaped()
        => ExecutionTest("other_id_continue-escaped");

    [Fact(DisplayName = "other_id_continue.js")]
    public Task other_id_continue()
        => ExecutionTest("other_id_continue");

    [Fact(DisplayName = "other_id_start-escaped.js")]
    public Task other_id_start_escaped()
        => ExecutionTest("other_id_start-escaped");

    [Fact(DisplayName = "other_id_start.js")]
    public Task other_id_start()
        => ExecutionTest("other_id_start");

    [Fact(DisplayName = "part-unicode-10.0.0-class-escaped.js")]
    public Task part_unicode_10_0_0_class_escaped()
        => ExecutionTest("part-unicode-10.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-10.0.0-class.js")]
    public Task part_unicode_10_0_0_class()
        => ExecutionTest("part-unicode-10.0.0-class");

    [Fact(DisplayName = "part-unicode-10.0.0-escaped.js")]
    public Task part_unicode_10_0_0_escaped()
        => ExecutionTest("part-unicode-10.0.0-escaped");

    [Fact(DisplayName = "part-unicode-10.0.0.js")]
    public Task part_unicode_10_0_0()
        => ExecutionTest("part-unicode-10.0.0");

    [Fact(DisplayName = "part-unicode-11.0.0-class-escaped.js")]
    public Task part_unicode_11_0_0_class_escaped()
        => ExecutionTest("part-unicode-11.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-11.0.0-class.js")]
    public Task part_unicode_11_0_0_class()
        => ExecutionTest("part-unicode-11.0.0-class");

    [Fact(DisplayName = "part-unicode-11.0.0-escaped.js")]
    public Task part_unicode_11_0_0_escaped()
        => ExecutionTest("part-unicode-11.0.0-escaped");

    [Fact(DisplayName = "part-unicode-11.0.0.js")]
    public Task part_unicode_11_0_0()
        => ExecutionTest("part-unicode-11.0.0");

    [Fact(DisplayName = "part-unicode-12.0.0-class-escaped.js")]
    public Task part_unicode_12_0_0_class_escaped()
        => ExecutionTest("part-unicode-12.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-12.0.0-class.js")]
    public Task part_unicode_12_0_0_class()
        => ExecutionTest("part-unicode-12.0.0-class");

    [Fact(DisplayName = "part-unicode-12.0.0-escaped.js")]
    public Task part_unicode_12_0_0_escaped()
        => ExecutionTest("part-unicode-12.0.0-escaped");

    [Fact(DisplayName = "part-unicode-12.0.0.js")]
    public Task part_unicode_12_0_0()
        => ExecutionTest("part-unicode-12.0.0");

    [Fact(DisplayName = "part-unicode-13.0.0-class-escaped.js")]
    public Task part_unicode_13_0_0_class_escaped()
        => ExecutionTest("part-unicode-13.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-13.0.0-class.js")]
    public Task part_unicode_13_0_0_class()
        => ExecutionTest("part-unicode-13.0.0-class");

    [Fact(DisplayName = "part-unicode-13.0.0-escaped.js")]
    public Task part_unicode_13_0_0_escaped()
        => ExecutionTest("part-unicode-13.0.0-escaped");

    [Fact(DisplayName = "part-unicode-13.0.0.js")]
    public Task part_unicode_13_0_0()
        => ExecutionTest("part-unicode-13.0.0");

    [Fact(DisplayName = "part-unicode-14.0.0-class-escaped.js")]
    public Task part_unicode_14_0_0_class_escaped()
        => ExecutionTest("part-unicode-14.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-14.0.0-class.js")]
    public Task part_unicode_14_0_0_class()
        => ExecutionTest("part-unicode-14.0.0-class");

    [Fact(DisplayName = "part-unicode-14.0.0-escaped.js")]
    public Task part_unicode_14_0_0_escaped()
        => ExecutionTest("part-unicode-14.0.0-escaped");

    [Fact(DisplayName = "part-unicode-14.0.0.js")]
    public Task part_unicode_14_0_0()
        => ExecutionTest("part-unicode-14.0.0");

    [Fact(DisplayName = "part-unicode-15.0.0-class-escaped.js")]
    public Task part_unicode_15_0_0_class_escaped()
        => ExecutionTest("part-unicode-15.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-15.0.0-class.js")]
    public Task part_unicode_15_0_0_class()
        => ExecutionTest("part-unicode-15.0.0-class");

    [Fact(DisplayName = "part-unicode-15.0.0-escaped.js")]
    public Task part_unicode_15_0_0_escaped()
        => ExecutionTest("part-unicode-15.0.0-escaped");

    [Fact(DisplayName = "part-unicode-15.0.0.js")]
    public Task part_unicode_15_0_0()
        => ExecutionTest("part-unicode-15.0.0");

    [Fact(DisplayName = "part-unicode-15.1.0-class-escaped.js")]
    public Task part_unicode_15_1_0_class_escaped()
        => ExecutionTest("part-unicode-15.1.0-class-escaped");

    [Fact(DisplayName = "part-unicode-15.1.0-class.js")]
    public Task part_unicode_15_1_0_class()
        => ExecutionTest("part-unicode-15.1.0-class");

    [Fact(DisplayName = "part-unicode-15.1.0-escaped.js")]
    public Task part_unicode_15_1_0_escaped()
        => ExecutionTest("part-unicode-15.1.0-escaped");

    [Fact(DisplayName = "part-unicode-15.1.0.js")]
    public Task part_unicode_15_1_0()
        => ExecutionTest("part-unicode-15.1.0");

    [Fact(DisplayName = "part-unicode-16.0.0-class-escaped.js")]
    public Task part_unicode_16_0_0_class_escaped()
        => ExecutionTest("part-unicode-16.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-16.0.0-class.js")]
    public Task part_unicode_16_0_0_class()
        => ExecutionTest("part-unicode-16.0.0-class");

    [Fact(DisplayName = "part-unicode-16.0.0-escaped.js")]
    public Task part_unicode_16_0_0_escaped()
        => ExecutionTest("part-unicode-16.0.0-escaped");

    [Fact(DisplayName = "part-unicode-16.0.0.js")]
    public Task part_unicode_16_0_0()
        => ExecutionTest("part-unicode-16.0.0");

    [Fact(DisplayName = "part-unicode-17.0.0-class-escaped.js")]
    public Task part_unicode_17_0_0_class_escaped()
        => ExecutionTest("part-unicode-17.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-17.0.0-class.js")]
    public Task part_unicode_17_0_0_class()
        => ExecutionTest("part-unicode-17.0.0-class");

    [Fact(DisplayName = "part-unicode-17.0.0-escaped.js")]
    public Task part_unicode_17_0_0_escaped()
        => ExecutionTest("part-unicode-17.0.0-escaped");

    [Fact(DisplayName = "part-unicode-17.0.0.js")]
    public Task part_unicode_17_0_0()
        => ExecutionTest("part-unicode-17.0.0");

    [Fact(DisplayName = "part-unicode-5.2.0-class-escaped.js")]
    public Task part_unicode_5_2_0_class_escaped()
        => ExecutionTest("part-unicode-5.2.0-class-escaped");

    [Fact(DisplayName = "part-unicode-5.2.0-class.js")]
    public Task part_unicode_5_2_0_class()
        => ExecutionTest("part-unicode-5.2.0-class");

    [Fact(DisplayName = "part-unicode-5.2.0-escaped.js")]
    public Task part_unicode_5_2_0_escaped()
        => ExecutionTest("part-unicode-5.2.0-escaped");

    [Fact(DisplayName = "part-unicode-5.2.0.js")]
    public Task part_unicode_5_2_0()
        => ExecutionTest("part-unicode-5.2.0");

    [Fact(DisplayName = "part-unicode-6.0.0-class-escaped.js")]
    public Task part_unicode_6_0_0_class_escaped()
        => ExecutionTest("part-unicode-6.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-6.0.0-class.js")]
    public Task part_unicode_6_0_0_class()
        => ExecutionTest("part-unicode-6.0.0-class");

    [Fact(DisplayName = "part-unicode-6.0.0-escaped.js")]
    public Task part_unicode_6_0_0_escaped()
        => ExecutionTest("part-unicode-6.0.0-escaped");

    [Fact(DisplayName = "part-unicode-6.0.0.js")]
    public Task part_unicode_6_0_0()
        => ExecutionTest("part-unicode-6.0.0");

    [Fact(DisplayName = "part-unicode-6.1.0-class-escaped.js")]
    public Task part_unicode_6_1_0_class_escaped()
        => ExecutionTest("part-unicode-6.1.0-class-escaped");

    [Fact(DisplayName = "part-unicode-6.1.0-class.js")]
    public Task part_unicode_6_1_0_class()
        => ExecutionTest("part-unicode-6.1.0-class");

    [Fact(DisplayName = "part-unicode-6.1.0-escaped.js")]
    public Task part_unicode_6_1_0_escaped()
        => ExecutionTest("part-unicode-6.1.0-escaped");

    [Fact(DisplayName = "part-unicode-6.1.0.js")]
    public Task part_unicode_6_1_0()
        => ExecutionTest("part-unicode-6.1.0");

    [Fact(DisplayName = "part-unicode-7.0.0-class-escaped.js")]
    public Task part_unicode_7_0_0_class_escaped()
        => ExecutionTest("part-unicode-7.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-7.0.0-class.js")]
    public Task part_unicode_7_0_0_class()
        => ExecutionTest("part-unicode-7.0.0-class");

    [Fact(DisplayName = "part-unicode-7.0.0-escaped.js")]
    public Task part_unicode_7_0_0_escaped()
        => ExecutionTest("part-unicode-7.0.0-escaped");

    [Fact(DisplayName = "part-unicode-7.0.0.js")]
    public Task part_unicode_7_0_0()
        => ExecutionTest("part-unicode-7.0.0");

    [Fact(DisplayName = "part-unicode-8.0.0-class-escaped.js")]
    public Task part_unicode_8_0_0_class_escaped()
        => ExecutionTest("part-unicode-8.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-8.0.0-class.js")]
    public Task part_unicode_8_0_0_class()
        => ExecutionTest("part-unicode-8.0.0-class");

    [Fact(DisplayName = "part-unicode-8.0.0-escaped.js")]
    public Task part_unicode_8_0_0_escaped()
        => ExecutionTest("part-unicode-8.0.0-escaped");

    [Fact(DisplayName = "part-unicode-8.0.0.js")]
    public Task part_unicode_8_0_0()
        => ExecutionTest("part-unicode-8.0.0");

    [Fact(DisplayName = "part-unicode-9.0.0-class-escaped.js")]
    public Task part_unicode_9_0_0_class_escaped()
        => ExecutionTest("part-unicode-9.0.0-class-escaped");

    [Fact(DisplayName = "part-unicode-9.0.0-class.js")]
    public Task part_unicode_9_0_0_class()
        => ExecutionTest("part-unicode-9.0.0-class");

    [Fact(DisplayName = "part-unicode-9.0.0-escaped.js")]
    public Task part_unicode_9_0_0_escaped()
        => ExecutionTest("part-unicode-9.0.0-escaped");

    [Fact(DisplayName = "part-unicode-9.0.0.js")]
    public Task part_unicode_9_0_0()
        => ExecutionTest("part-unicode-9.0.0");

    [Fact(DisplayName = "part-zwj-zwnj-escaped.js")]
    public Task part_zwj_zwnj_escaped()
        => ExecutionTest("part-zwj-zwnj-escaped");

    [Fact(DisplayName = "start-dollar-sign.js")]
    public Task start_dollar_sign()
        => ExecutionTest("start-dollar-sign");

    [Fact(DisplayName = "start-escape-seq.js")]
    public Task start_escape_seq()
        => ExecutionTest("start-escape-seq");

    [Fact(DisplayName = "start-underscore.js")]
    public Task start_underscore()
        => ExecutionTest("start-underscore");

    [Fact(DisplayName = "start-unicode-10.0.0-class-escaped.js")]
    public Task start_unicode_10_0_0_class_escaped()
        => ExecutionTest("start-unicode-10.0.0-class-escaped");

    [Fact(DisplayName = "start-unicode-10.0.0-class.js")]
    public Task start_unicode_10_0_0_class()
        => ExecutionTest("start-unicode-10.0.0-class");

    [Fact(DisplayName = "start-unicode-10.0.0-escaped.js")]
    public Task start_unicode_10_0_0_escaped()
        => ExecutionTest("start-unicode-10.0.0-escaped");

    [Fact(DisplayName = "start-unicode-10.0.0.js")]
    public Task start_unicode_10_0_0()
        => ExecutionTest("start-unicode-10.0.0");

    [Fact(DisplayName = "start-unicode-11.0.0-class-escaped.js")]
    public Task start_unicode_11_0_0_class_escaped()
        => ExecutionTest("start-unicode-11.0.0-class-escaped");

    [Fact(DisplayName = "start-unicode-11.0.0-class.js")]
    public Task start_unicode_11_0_0_class()
        => ExecutionTest("start-unicode-11.0.0-class");

    [Fact(DisplayName = "start-unicode-11.0.0-escaped.js")]
    public Task start_unicode_11_0_0_escaped()
        => ExecutionTest("start-unicode-11.0.0-escaped");

    [Fact(DisplayName = "start-unicode-11.0.0.js")]
    public Task start_unicode_11_0_0()
        => ExecutionTest("start-unicode-11.0.0");

    [Fact(DisplayName = "start-unicode-12.0.0-class-escaped.js")]
    public Task start_unicode_12_0_0_class_escaped()
        => ExecutionTest("start-unicode-12.0.0-class-escaped");

    [Fact(DisplayName = "start-unicode-12.0.0-class.js")]
    public Task start_unicode_12_0_0_class()
        => ExecutionTest("start-unicode-12.0.0-class");

    [Fact(DisplayName = "start-unicode-12.0.0-escaped.js")]
    public Task start_unicode_12_0_0_escaped()
        => ExecutionTest("start-unicode-12.0.0-escaped");

    [Fact(DisplayName = "start-unicode-12.0.0.js")]
    public Task start_unicode_12_0_0()
        => ExecutionTest("start-unicode-12.0.0");

    [Fact(DisplayName = "start-unicode-13.0.0-class-escaped.js")]
    public Task start_unicode_13_0_0_class_escaped()
        => ExecutionTest("start-unicode-13.0.0-class-escaped");

    [Fact(DisplayName = "start-unicode-13.0.0-class.js")]
    public Task start_unicode_13_0_0_class()
        => ExecutionTest("start-unicode-13.0.0-class");

    [Fact(DisplayName = "start-unicode-13.0.0-escaped.js")]
    public Task start_unicode_13_0_0_escaped()
        => ExecutionTest("start-unicode-13.0.0-escaped");

    [Fact(DisplayName = "start-unicode-13.0.0.js")]
    public Task start_unicode_13_0_0()
        => ExecutionTest("start-unicode-13.0.0");
}
