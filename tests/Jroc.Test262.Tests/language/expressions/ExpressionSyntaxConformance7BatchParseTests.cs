using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions;

public class ExpressionSyntaxConformance7BatchParseTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformance7BatchParseTests() : base("language/expressions", "language.expressions") { }

    [Fact(DisplayName = "target-cover-yieldexpr.js")]
    public Task test_assignment_target_cover_yieldexpr()
        => CompilationFailureTest("assignment/target-cover-yieldexpr");

    [Fact(DisplayName = "target-newtarget.js")]
    public Task test_assignment_target_newtarget()
        => CompilationFailureTest("assignment/target-newtarget");

    [Fact(DisplayName = "target-null.js")]
    public Task test_assignment_target_null()
        => CompilationFailureTest("assignment/target-null");

    [Fact(DisplayName = "target-number.js")]
    public Task test_assignment_target_number()
        => CompilationFailureTest("assignment/target-number");

    [Fact(DisplayName = "target-string.js")]
    public Task test_assignment_target_string()
        => CompilationFailureTest("assignment/target-string");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-logical-and-assignment-assignmentexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_lefthandsideexpression_logical_and_assignment_assignmentexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-lefthandsideexpression-logical-and-assignment-assignmentexpression-2");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-logical-or-assignment-assignmentexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_lefthandsideexpression_logical_or_assignment_assignmentexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-lefthandsideexpression-logical-or-assignment-assignmentexpression-0");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-logical-or-assignment-assignmentexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_lefthandsideexpression_logical_or_assignment_assignmentexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-lefthandsideexpression-logical-or-assignment-assignmentexpression-1");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-logical-or-assignment-assignmentexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_lefthandsideexpression_logical_or_assignment_assignmentexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-lefthandsideexpression-logical-or-assignment-assignmentexpression-2");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-minus-minus.js")]
    public Task test_assignmenttargettype_parenthesized_lefthandsideexpression_minus_minus()
        => CompilationFailureTest("assignmenttargettype/parenthesized-lefthandsideexpression-minus-minus");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-plus-plus.js")]
    public Task test_assignmenttargettype_parenthesized_lefthandsideexpression_plus_plus()
        => CompilationFailureTest("assignmenttargettype/parenthesized-lefthandsideexpression-plus-plus");

    [Fact(DisplayName = "parenthesized-logicalandexpression-logical-and-bitwiseorexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_logicalandexpression_logical_and_bitwiseorexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-logicalandexpression-logical-and-bitwiseorexpression-0");

    [Fact(DisplayName = "parenthesized-logicalandexpression-logical-and-bitwiseorexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_logicalandexpression_logical_and_bitwiseorexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-logicalandexpression-logical-and-bitwiseorexpression-1");

    [Fact(DisplayName = "parenthesized-logicalandexpression-logical-and-bitwiseorexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_logicalandexpression_logical_and_bitwiseorexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-logicalandexpression-logical-and-bitwiseorexpression-2");

    [Fact(DisplayName = "parenthesized-logicalorexpression-logical-or-logicalandexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_logicalorexpression_logical_or_logicalandexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-logicalorexpression-logical-or-logicalandexpression-0");

    [Fact(DisplayName = "parenthesized-logicalorexpression-logical-or-logicalandexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_logicalorexpression_logical_or_logicalandexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-logicalorexpression-logical-or-logicalandexpression-1");

    [Fact(DisplayName = "parenthesized-logicalorexpression-logical-or-logicalandexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_logicalorexpression_logical_or_logicalandexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-logicalorexpression-logical-or-logicalandexpression-2");

    [Fact(DisplayName = "parenthesized-memberexpression-templateliteral.js")]
    public Task test_assignmenttargettype_parenthesized_memberexpression_templateliteral()
        => CompilationFailureTest("assignmenttargettype/parenthesized-memberexpression-templateliteral");

    [Fact(DisplayName = "parenthesized-minus-minus-unaryexpression.js")]
    public Task test_assignmenttargettype_parenthesized_minus_minus_unaryexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-minus-minus-unaryexpression");

    [Fact(DisplayName = "parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-0");

    [Fact(DisplayName = "parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-1");

    [Fact(DisplayName = "parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-2");

    [Fact(DisplayName = "parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-3.js")]
    public Task test_assignmenttargettype_parenthesized_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_3()
        => CompilationFailureTest("assignmenttargettype/parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-3");

    [Fact(DisplayName = "parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-4.js")]
    public Task test_assignmenttargettype_parenthesized_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_4()
        => CompilationFailureTest("assignmenttargettype/parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-4");

    [Fact(DisplayName = "parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-5.js")]
    public Task test_assignmenttargettype_parenthesized_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_5()
        => CompilationFailureTest("assignmenttargettype/parenthesized-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-5");

    [Fact(DisplayName = "parenthesized-new-memberexpression-arguments.js")]
    public Task test_assignmenttargettype_parenthesized_new_memberexpression_arguments()
        => CompilationFailureTest("assignmenttargettype/parenthesized-new-memberexpression-arguments");

    [Fact(DisplayName = "parenthesized-new-newexpression.js")]
    public Task test_assignmenttargettype_parenthesized_new_newexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-new-newexpression");

    [Fact(DisplayName = "parenthesized-optionalexpression.js")]
    public Task test_assignmenttargettype_parenthesized_optionalexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-optionalexpression");

    [Fact(DisplayName = "parenthesized-plus-plus-unaryexpression.js")]
    public Task test_assignmenttargettype_parenthesized_plus_plus_unaryexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-plus-plus-unaryexpression");

    [Fact(DisplayName = "parenthesized-primaryexpression-asyncfunctionexpression.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_asyncfunctionexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-asyncfunctionexpression");

    [Fact(DisplayName = "parenthesized-primaryexpression-asyncgeneratorexpression.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_asyncgeneratorexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-asyncgeneratorexpression");

    [Fact(DisplayName = "parenthesized-primaryexpression-classexpression.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_classexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-classexpression");

    [Fact(DisplayName = "parenthesized-primaryexpression-functionexpression.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_functionexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-functionexpression");

    [Fact(DisplayName = "parenthesized-primaryexpression-generatorexpression.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_generatorexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-generatorexpression");

    [Fact(DisplayName = "parenthesized-primaryexpression-literal-boolean.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_literal_boolean()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-literal-boolean");

    [Fact(DisplayName = "parenthesized-primaryexpression-literal-null.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_literal_null()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-literal-null");

    [Fact(DisplayName = "parenthesized-primaryexpression-literal-numeric.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_literal_numeric()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-literal-numeric");

    [Fact(DisplayName = "parenthesized-primaryexpression-literal-string.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_literal_string()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-literal-string");

    [Fact(DisplayName = "parenthesized-primaryexpression-objectliteral.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_objectliteral()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-objectliteral");

    [Fact(DisplayName = "parenthesized-primaryexpression-regularexpressionliteral.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_regularexpressionliteral()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-regularexpressionliteral");

    [Fact(DisplayName = "parenthesized-primaryexpression-templateliteral.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_templateliteral()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-templateliteral");

    [Fact(DisplayName = "parenthesized-primaryexpression-this.js")]
    public Task test_assignmenttargettype_parenthesized_primaryexpression_this()
        => CompilationFailureTest("assignmenttargettype/parenthesized-primaryexpression-this");

    [Fact(DisplayName = "parenthesized-relationalexpression-greater-than-or-equal-to-shiftexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_greater_than_or_equal_to_shiftexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-greater-than-or-equal-to-shiftexpression-0");

    [Fact(DisplayName = "parenthesized-relationalexpression-greater-than-or-equal-to-shiftexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_greater_than_or_equal_to_shiftexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-greater-than-or-equal-to-shiftexpression-1");

    [Fact(DisplayName = "parenthesized-relationalexpression-greater-than-or-equal-to-shiftexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_greater_than_or_equal_to_shiftexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-greater-than-or-equal-to-shiftexpression-2");

    [Fact(DisplayName = "parenthesized-relationalexpression-greater-than-shiftexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_greater_than_shiftexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-greater-than-shiftexpression-0");

    [Fact(DisplayName = "parenthesized-relationalexpression-greater-than-shiftexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_greater_than_shiftexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-greater-than-shiftexpression-1");

    [Fact(DisplayName = "parenthesized-relationalexpression-greater-than-shiftexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_greater_than_shiftexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-greater-than-shiftexpression-2");

    [Fact(DisplayName = "parenthesized-relationalexpression-in-shiftexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_in_shiftexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-in-shiftexpression-0");

    [Fact(DisplayName = "parenthesized-relationalexpression-in-shiftexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_in_shiftexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-in-shiftexpression-1");

    [Fact(DisplayName = "parenthesized-relationalexpression-in-shiftexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_in_shiftexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-in-shiftexpression-2");

    [Fact(DisplayName = "parenthesized-relationalexpression-instanceof-shiftexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_instanceof_shiftexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-instanceof-shiftexpression-0");

    [Fact(DisplayName = "parenthesized-relationalexpression-instanceof-shiftexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_instanceof_shiftexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-instanceof-shiftexpression-1");

    [Fact(DisplayName = "parenthesized-relationalexpression-instanceof-shiftexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_instanceof_shiftexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-instanceof-shiftexpression-2");

    [Fact(DisplayName = "parenthesized-relationalexpression-less-than-or-equal-to-shiftexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_less_than_or_equal_to_shiftexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-less-than-or-equal-to-shiftexpression-0");

    [Fact(DisplayName = "parenthesized-relationalexpression-less-than-or-equal-to-shiftexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_less_than_or_equal_to_shiftexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-less-than-or-equal-to-shiftexpression-1");

    [Fact(DisplayName = "parenthesized-relationalexpression-less-than-or-equal-to-shiftexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_less_than_or_equal_to_shiftexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-less-than-or-equal-to-shiftexpression-2");

    [Fact(DisplayName = "parenthesized-relationalexpression-less-than-shiftexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_less_than_shiftexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-less-than-shiftexpression-0");

    [Fact(DisplayName = "parenthesized-relationalexpression-less-than-shiftexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_less_than_shiftexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-less-than-shiftexpression-1");

    [Fact(DisplayName = "parenthesized-relationalexpression-less-than-shiftexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_relationalexpression_less_than_shiftexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-relationalexpression-less-than-shiftexpression-2");

    [Fact(DisplayName = "parenthesized-shiftexpression-bitwise-left-additiveexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_bitwise_left_additiveexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-bitwise-left-additiveexpression-0");

    [Fact(DisplayName = "parenthesized-shiftexpression-bitwise-left-additiveexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_bitwise_left_additiveexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-bitwise-left-additiveexpression-1");

    [Fact(DisplayName = "parenthesized-shiftexpression-bitwise-left-additiveexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_bitwise_left_additiveexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-bitwise-left-additiveexpression-2");

    [Fact(DisplayName = "parenthesized-shiftexpression-bitwise-right-additiveexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_bitwise_right_additiveexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-bitwise-right-additiveexpression-0");

    [Fact(DisplayName = "parenthesized-shiftexpression-bitwise-right-additiveexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_bitwise_right_additiveexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-bitwise-right-additiveexpression-1");

    [Fact(DisplayName = "parenthesized-shiftexpression-bitwise-right-additiveexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_bitwise_right_additiveexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-bitwise-right-additiveexpression-2");

    [Fact(DisplayName = "parenthesized-shiftexpression-unsigned-bitwise-right-additiveexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_unsigned_bitwise_right_additiveexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-unsigned-bitwise-right-additiveexpression-0");

    [Fact(DisplayName = "parenthesized-shiftexpression-unsigned-bitwise-right-additiveexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_unsigned_bitwise_right_additiveexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-unsigned-bitwise-right-additiveexpression-1");

    [Fact(DisplayName = "parenthesized-shiftexpression-unsigned-bitwise-right-additiveexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_shiftexpression_unsigned_bitwise_right_additiveexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shiftexpression-unsigned-bitwise-right-additiveexpression-2");

    [Fact(DisplayName = "parenthesized-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_shortcircuitexpression_question_assignmentexpression_else_assignmentexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-0");

    [Fact(DisplayName = "parenthesized-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_shortcircuitexpression_question_assignmentexpression_else_assignmentexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-1");

    [Fact(DisplayName = "parenthesized-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_shortcircuitexpression_question_assignmentexpression_else_assignmentexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-2");

    [Fact(DisplayName = "parenthesized-unaryexpression-delete-unaryexpression.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_delete_unaryexpression()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-delete-unaryexpression");

    [Fact(DisplayName = "parenthesized-unaryexpression-exclamation-unaryexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_exclamation_unaryexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-exclamation-unaryexpression-0");

    [Fact(DisplayName = "parenthesized-unaryexpression-exclamation-unaryexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_exclamation_unaryexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-exclamation-unaryexpression-1");

    [Fact(DisplayName = "parenthesized-unaryexpression-exclamation-unaryexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_exclamation_unaryexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-exclamation-unaryexpression-2");

    [Fact(DisplayName = "parenthesized-unaryexpression-minus-unaryexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_minus_unaryexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-minus-unaryexpression-0");

    [Fact(DisplayName = "parenthesized-unaryexpression-minus-unaryexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_minus_unaryexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-minus-unaryexpression-1");

    [Fact(DisplayName = "parenthesized-unaryexpression-minus-unaryexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_minus_unaryexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-minus-unaryexpression-2");

    [Fact(DisplayName = "parenthesized-unaryexpression-plus-unaryexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_plus_unaryexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-plus-unaryexpression-0");

    [Fact(DisplayName = "parenthesized-unaryexpression-plus-unaryexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_plus_unaryexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-plus-unaryexpression-1");

    [Fact(DisplayName = "parenthesized-unaryexpression-plus-unaryexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_plus_unaryexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-plus-unaryexpression-2");

    [Fact(DisplayName = "parenthesized-unaryexpression-tilde-unaryexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_tilde_unaryexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-tilde-unaryexpression-0");

    [Fact(DisplayName = "parenthesized-unaryexpression-tilde-unaryexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_tilde_unaryexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-tilde-unaryexpression-1");

    [Fact(DisplayName = "parenthesized-unaryexpression-tilde-unaryexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_tilde_unaryexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-tilde-unaryexpression-2");

    [Fact(DisplayName = "parenthesized-unaryexpression-typeof-unaryexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_typeof_unaryexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-typeof-unaryexpression-0");

    [Fact(DisplayName = "parenthesized-unaryexpression-typeof-unaryexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_typeof_unaryexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-typeof-unaryexpression-1");

    [Fact(DisplayName = "parenthesized-unaryexpression-typeof-unaryexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_typeof_unaryexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-typeof-unaryexpression-2");

    [Fact(DisplayName = "parenthesized-unaryexpression-void-unaryexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_void_unaryexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-void-unaryexpression-0");

    [Fact(DisplayName = "parenthesized-unaryexpression-void-unaryexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_void_unaryexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-void-unaryexpression-1");

    [Fact(DisplayName = "parenthesized-unaryexpression-void-unaryexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_unaryexpression_void_unaryexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-unaryexpression-void-unaryexpression-2");

    [Fact(DisplayName = "parenthesized-updateexpression-star-star-exponentiationexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_updateexpression_star_star_exponentiationexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-updateexpression-star-star-exponentiationexpression-0");

    [Fact(DisplayName = "parenthesized-updateexpression-star-star-exponentiationexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_updateexpression_star_star_exponentiationexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-updateexpression-star-star-exponentiationexpression-1");

    [Fact(DisplayName = "parenthesized-updateexpression-star-star-exponentiationexpression-2.js")]
    public Task test_assignmenttargettype_parenthesized_updateexpression_star_star_exponentiationexpression_2()
        => CompilationFailureTest("assignmenttargettype/parenthesized-updateexpression-star-star-exponentiationexpression-2");

    [Fact(DisplayName = "parenthesized-yieldexpression-0.js")]
    public Task test_assignmenttargettype_parenthesized_yieldexpression_0()
        => CompilationFailureTest("assignmenttargettype/parenthesized-yieldexpression-0");

    [Fact(DisplayName = "parenthesized-yieldexpression-1.js")]
    public Task test_assignmenttargettype_parenthesized_yieldexpression_1()
        => CompilationFailureTest("assignmenttargettype/parenthesized-yieldexpression-1");

    [Fact(DisplayName = "early-errors-expression-body-contains-super-call.js")]
    public Task test_async_generator_early_errors_expression_body_contains_super_call()
        => CompilationFailureTest("async-generator/early-errors-expression-body-contains-super-call");

    [Fact(DisplayName = "early-errors-expression-body-contains-super-property.js")]
    public Task test_async_generator_early_errors_expression_body_contains_super_property()
        => CompilationFailureTest("async-generator/early-errors-expression-body-contains-super-property");

    [Fact(DisplayName = "early-errors-expression-eval-in-formal-parameters.js")]
    public Task test_async_generator_early_errors_expression_eval_in_formal_parameters()
        => CompilationFailureTest("async-generator/early-errors-expression-eval-in-formal-parameters");

    [Fact(DisplayName = "early-errors-expression-formals-body-duplicate-const.js")]
    public Task test_async_generator_early_errors_expression_formals_body_duplicate_const()
        => CompilationFailureTest("async-generator/early-errors-expression-formals-body-duplicate-const");

    [Fact(DisplayName = "early-errors-expression-formals-body-duplicate-let.js")]
    public Task test_async_generator_early_errors_expression_formals_body_duplicate_let()
        => CompilationFailureTest("async-generator/early-errors-expression-formals-body-duplicate-let");

    [Fact(DisplayName = "early-errors-expression-formals-contains-await-expr.js")]
    public Task test_async_generator_early_errors_expression_formals_contains_await_expr()
        => CompilationFailureTest("async-generator/early-errors-expression-formals-contains-await-expr");

    [Fact(DisplayName = "early-errors-expression-formals-contains-await.js")]
    public Task test_async_generator_early_errors_expression_formals_contains_await()
        => CompilationFailureTest("async-generator/early-errors-expression-formals-contains-await");

    [Fact(DisplayName = "early-errors-expression-formals-contains-super-call.js")]
    public Task test_async_generator_early_errors_expression_formals_contains_super_call()
        => CompilationFailureTest("async-generator/early-errors-expression-formals-contains-super-call");

    [Fact(DisplayName = "early-errors-expression-formals-contains-super-property.js")]
    public Task test_async_generator_early_errors_expression_formals_contains_super_property()
        => CompilationFailureTest("async-generator/early-errors-expression-formals-contains-super-property");

    [Fact(DisplayName = "early-errors-expression-formals-contains-yield-expr.js")]
    public Task test_async_generator_early_errors_expression_formals_contains_yield_expr()
        => CompilationFailureTest("async-generator/early-errors-expression-formals-contains-yield-expr");

    [Fact(DisplayName = "early-errors-expression-formals-contains-yield.js")]
    public Task test_async_generator_early_errors_expression_formals_contains_yield()
        => CompilationFailureTest("async-generator/early-errors-expression-formals-contains-yield");

    [Fact(DisplayName = "early-errors-expression-label-name-await.js")]
    public Task test_async_generator_early_errors_expression_label_name_await()
        => CompilationFailureTest("async-generator/early-errors-expression-label-name-await");

    [Fact(DisplayName = "early-errors-expression-label-name-yield.js")]
    public Task test_async_generator_early_errors_expression_label_name_yield()
        => CompilationFailureTest("async-generator/early-errors-expression-label-name-yield");

    [Fact(DisplayName = "early-errors-expression-not-simple-assignment-target.js")]
    public Task test_async_generator_early_errors_expression_not_simple_assignment_target()
        => CompilationFailureTest("async-generator/early-errors-expression-not-simple-assignment-target");

    [Fact(DisplayName = "early-errors-expression-yield-as-function-binding-identifier.js")]
    public Task test_async_generator_early_errors_expression_yield_as_function_binding_identifier()
        => CompilationFailureTest("async-generator/early-errors-expression-yield-as-function-binding-identifier");

    [Fact(DisplayName = "early-errors-expression-yield-star-after-newline.js")]
    public Task test_async_generator_early_errors_expression_yield_star_after_newline()
        => CompilationFailureTest("async-generator/early-errors-expression-yield-star-after-newline");

    [Fact(DisplayName = "escaped-async.js")]
    public Task test_async_generator_escaped_async()
        => CompilationFailureTest("async-generator/escaped-async");

    [Fact(DisplayName = "named-array-destructuring-param-strict-body.js")]
    public Task test_async_generator_named_array_destructuring_param_strict_body()
        => CompilationFailureTest("async-generator/named-array-destructuring-param-strict-body");

    [Fact(DisplayName = "named-await-as-binding-identifier-escaped.js")]
    public Task test_async_generator_named_await_as_binding_identifier_escaped()
        => CompilationFailureTest("async-generator/named-await-as-binding-identifier-escaped");

    [Fact(DisplayName = "named-await-as-binding-identifier.js")]
    public Task test_async_generator_named_await_as_binding_identifier()
        => CompilationFailureTest("async-generator/named-await-as-binding-identifier");

    [Fact(DisplayName = "named-await-as-identifier-reference-escaped.js")]
    public Task test_async_generator_named_await_as_identifier_reference_escaped()
        => CompilationFailureTest("async-generator/named-await-as-identifier-reference-escaped");

    [Fact(DisplayName = "named-await-as-identifier-reference.js")]
    public Task test_async_generator_named_await_as_identifier_reference()
        => CompilationFailureTest("async-generator/named-await-as-identifier-reference");

    [Fact(DisplayName = "named-await-as-label-identifier-escaped.js")]
    public Task test_async_generator_named_await_as_label_identifier_escaped()
        => CompilationFailureTest("async-generator/named-await-as-label-identifier-escaped");

    [Fact(DisplayName = "named-await-as-label-identifier.js")]
    public Task test_async_generator_named_await_as_label_identifier()
        => CompilationFailureTest("async-generator/named-await-as-label-identifier");

    [Fact(DisplayName = "named-dflt-params-duplicates.js")]
    public Task test_async_generator_named_dflt_params_duplicates()
        => CompilationFailureTest("async-generator/named-dflt-params-duplicates");

    [Fact(DisplayName = "named-dflt-params-rest.js")]
    public Task test_async_generator_named_dflt_params_rest()
        => CompilationFailureTest("async-generator/named-dflt-params-rest");

    [Fact(DisplayName = "named-object-destructuring-param-strict-body.js")]
    public Task test_async_generator_named_object_destructuring_param_strict_body()
        => CompilationFailureTest("async-generator/named-object-destructuring-param-strict-body");

    [Fact(DisplayName = "named-rest-param-strict-body.js")]
    public Task test_async_generator_named_rest_param_strict_body()
        => CompilationFailureTest("async-generator/named-rest-param-strict-body");

    [Fact(DisplayName = "named-rest-params-trailing-comma-early-error.js")]
    public Task test_async_generator_named_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("async-generator/named-rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "named-yield-as-binding-identifier-escaped.js")]
    public Task test_async_generator_named_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("async-generator/named-yield-as-binding-identifier-escaped");

    [Fact(DisplayName = "named-yield-as-binding-identifier.js")]
    public Task test_async_generator_named_yield_as_binding_identifier()
        => CompilationFailureTest("async-generator/named-yield-as-binding-identifier");

    [Fact(DisplayName = "named-yield-as-identifier-reference-escaped.js")]
    public Task test_async_generator_named_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("async-generator/named-yield-as-identifier-reference-escaped");

    [Fact(DisplayName = "named-yield-as-identifier-reference.js")]
    public Task test_async_generator_named_yield_as_identifier_reference()
        => CompilationFailureTest("async-generator/named-yield-as-identifier-reference");

    [Fact(DisplayName = "named-yield-as-label-identifier-escaped.js")]
    public Task test_async_generator_named_yield_as_label_identifier_escaped()
        => CompilationFailureTest("async-generator/named-yield-as-label-identifier-escaped");

    [Fact(DisplayName = "named-yield-as-label-identifier.js")]
    public Task test_async_generator_named_yield_as_label_identifier()
        => CompilationFailureTest("async-generator/named-yield-as-label-identifier");

    [Fact(DisplayName = "named-yield-identifier-spread-strict.js")]
    public Task test_async_generator_named_yield_identifier_spread_strict()
        => CompilationFailureTest("async-generator/named-yield-identifier-spread-strict");

    [Fact(DisplayName = "named-yield-identifier-strict.js")]
    public Task test_async_generator_named_yield_identifier_strict()
        => CompilationFailureTest("async-generator/named-yield-identifier-strict");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_async_generator_object_destructuring_param_strict_body()
        => CompilationFailureTest("async-generator/object-destructuring-param-strict-body");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_async_generator_rest_param_strict_body()
        => CompilationFailureTest("async-generator/rest-param-strict-body");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_async_generator_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("async-generator/rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "yield-as-binding-identifier-escaped.js")]
    public Task test_async_generator_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("async-generator/yield-as-binding-identifier-escaped");

    [Fact(DisplayName = "yield-as-binding-identifier.js")]
    public Task test_async_generator_yield_as_binding_identifier()
        => CompilationFailureTest("async-generator/yield-as-binding-identifier");

    [Fact(DisplayName = "yield-as-identifier-reference-escaped.js")]
    public Task test_async_generator_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("async-generator/yield-as-identifier-reference-escaped");

    [Fact(DisplayName = "yield-as-identifier-reference.js")]
    public Task test_async_generator_yield_as_identifier_reference()
        => CompilationFailureTest("async-generator/yield-as-identifier-reference");

    [Fact(DisplayName = "yield-as-label-identifier-escaped.js")]
    public Task test_async_generator_yield_as_label_identifier_escaped()
        => CompilationFailureTest("async-generator/yield-as-label-identifier-escaped");

    [Fact(DisplayName = "yield-as-label-identifier.js")]
    public Task test_async_generator_yield_as_label_identifier()
        => CompilationFailureTest("async-generator/yield-as-label-identifier");

    [Fact(DisplayName = "yield-identifier-spread-strict.js")]
    public Task test_async_generator_yield_identifier_spread_strict()
        => CompilationFailureTest("async-generator/yield-identifier-spread-strict");

    [Fact(DisplayName = "yield-identifier-strict.js")]
    public Task test_async_generator_yield_identifier_strict()
        => CompilationFailureTest("async-generator/yield-identifier-strict");

    [Fact(DisplayName = "S11.2.4_A1.3_T1.js")]
    public Task test_call_S11_2_4_A1_3_T1()
        => CompilationFailureTest("call/S11.2.4_A1.3_T1");

    [Fact(DisplayName = "gen-method-param-dflt-yield.js")]
    public Task test_class_gen_method_param_dflt_yield()
        => CompilationFailureTest("class/gen-method-param-dflt-yield");

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_class_gen_method_static_array_destructuring_param_strict_body()
        => CompilationFailureTest("class/gen-method-static/array-destructuring-param-strict-body");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_class_gen_method_static_dflt_params_duplicates()
        => CompilationFailureTest("class/gen-method-static/dflt-params-duplicates");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_class_gen_method_static_dflt_params_rest()
        => CompilationFailureTest("class/gen-method-static/dflt-params-rest");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_class_gen_method_static_object_destructuring_param_strict_body()
        => CompilationFailureTest("class/gen-method-static/object-destructuring-param-strict-body");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_class_gen_method_static_rest_param_strict_body()
        => CompilationFailureTest("class/gen-method-static/rest-param-strict-body");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_class_gen_method_static_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("class/gen-method-static/rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "yield-as-binding-identifier-escaped.js")]
    public Task test_class_gen_method_static_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("class/gen-method-static/yield-as-binding-identifier-escaped");

    [Fact(DisplayName = "yield-as-binding-identifier.js")]
    public Task test_class_gen_method_static_yield_as_binding_identifier()
        => CompilationFailureTest("class/gen-method-static/yield-as-binding-identifier");

    [Fact(DisplayName = "yield-as-identifier-reference-escaped.js")]
    public Task test_class_gen_method_static_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("class/gen-method-static/yield-as-identifier-reference-escaped");

    [Fact(DisplayName = "yield-as-identifier-reference.js")]
    public Task test_class_gen_method_static_yield_as_identifier_reference()
        => CompilationFailureTest("class/gen-method-static/yield-as-identifier-reference");

    [Fact(DisplayName = "yield-as-label-identifier-escaped.js")]
    public Task test_class_gen_method_static_yield_as_label_identifier_escaped()
        => CompilationFailureTest("class/gen-method-static/yield-as-label-identifier-escaped");

    [Fact(DisplayName = "yield-as-label-identifier.js")]
    public Task test_class_gen_method_static_yield_as_label_identifier()
        => CompilationFailureTest("class/gen-method-static/yield-as-label-identifier");

    [Fact(DisplayName = "yield-identifier-spread-strict.js")]
    public Task test_class_gen_method_static_yield_identifier_spread_strict()
        => CompilationFailureTest("class/gen-method-static/yield-identifier-spread-strict");

    [Fact(DisplayName = "yield-identifier-strict.js")]
    public Task test_class_gen_method_static_yield_identifier_strict()
        => CompilationFailureTest("class/gen-method-static/yield-identifier-strict");

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_class_gen_method_array_destructuring_param_strict_body()
        => CompilationFailureTest("class/gen-method/array-destructuring-param-strict-body");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_class_gen_method_dflt_params_duplicates()
        => CompilationFailureTest("class/gen-method/dflt-params-duplicates");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_class_gen_method_dflt_params_rest()
        => CompilationFailureTest("class/gen-method/dflt-params-rest");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_class_gen_method_object_destructuring_param_strict_body()
        => CompilationFailureTest("class/gen-method/object-destructuring-param-strict-body");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_class_gen_method_rest_param_strict_body()
        => CompilationFailureTest("class/gen-method/rest-param-strict-body");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_class_gen_method_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("class/gen-method/rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "yield-as-binding-identifier-escaped.js")]
    public Task test_class_gen_method_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("class/gen-method/yield-as-binding-identifier-escaped");

    [Fact(DisplayName = "yield-as-binding-identifier.js")]
    public Task test_class_gen_method_yield_as_binding_identifier()
        => CompilationFailureTest("class/gen-method/yield-as-binding-identifier");

    [Fact(DisplayName = "yield-as-identifier-reference-escaped.js")]
    public Task test_class_gen_method_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("class/gen-method/yield-as-identifier-reference-escaped");

    [Fact(DisplayName = "yield-as-identifier-reference.js")]
    public Task test_class_gen_method_yield_as_identifier_reference()
        => CompilationFailureTest("class/gen-method/yield-as-identifier-reference");

    [Fact(DisplayName = "yield-as-label-identifier-escaped.js")]
    public Task test_class_gen_method_yield_as_label_identifier_escaped()
        => CompilationFailureTest("class/gen-method/yield-as-label-identifier-escaped");

    [Fact(DisplayName = "yield-as-label-identifier.js")]
    public Task test_class_gen_method_yield_as_label_identifier()
        => CompilationFailureTest("class/gen-method/yield-as-label-identifier");

    [Fact(DisplayName = "yield-identifier-spread-strict.js")]
    public Task test_class_gen_method_yield_identifier_spread_strict()
        => CompilationFailureTest("class/gen-method/yield-identifier-spread-strict");

    [Fact(DisplayName = "yield-identifier-strict.js")]
    public Task test_class_gen_method_yield_identifier_strict()
        => CompilationFailureTest("class/gen-method/yield-identifier-strict");

    [Fact(DisplayName = "getter-param-dflt.js")]
    public Task test_class_getter_param_dflt()
        => CompilationFailureTest("class/getter-param-dflt");

    [Fact(DisplayName = "method-param-dflt-yield.js")]
    public Task test_class_method_param_dflt_yield()
        => CompilationFailureTest("class/method-param-dflt-yield");

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_class_method_static_array_destructuring_param_strict_body()
        => CompilationFailureTest("class/method-static/array-destructuring-param-strict-body");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_class_method_static_dflt_params_duplicates()
        => CompilationFailureTest("class/method-static/dflt-params-duplicates");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_class_method_static_dflt_params_rest()
        => CompilationFailureTest("class/method-static/dflt-params-rest");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_class_method_static_object_destructuring_param_strict_body()
        => CompilationFailureTest("class/method-static/object-destructuring-param-strict-body");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_class_method_static_rest_param_strict_body()
        => CompilationFailureTest("class/method-static/rest-param-strict-body");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_class_method_static_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("class/method-static/rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_class_method_array_destructuring_param_strict_body()
        => CompilationFailureTest("class/method/array-destructuring-param-strict-body");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_class_method_dflt_params_duplicates()
        => CompilationFailureTest("class/method/dflt-params-duplicates");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_class_method_dflt_params_rest()
        => CompilationFailureTest("class/method/dflt-params-rest");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_class_method_object_destructuring_param_strict_body()
        => CompilationFailureTest("class/method/object-destructuring-param-strict-body");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_class_method_rest_param_strict_body()
        => CompilationFailureTest("class/method/rest-param-strict-body");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_class_method_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("class/method/rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "static-gen-method-param-dflt-yield.js")]
    public Task test_class_static_gen_method_param_dflt_yield()
        => CompilationFailureTest("class/static-gen-method-param-dflt-yield");

    [Fact(DisplayName = "static-init-await-binding.js")]
    public Task test_class_static_init_await_binding()
        => CompilationFailureTest("class/static-init-await-binding");

    [Fact(DisplayName = "static-method-param-dflt-yield.js")]
    public Task test_class_static_method_param_dflt_yield()
        => CompilationFailureTest("class/static-method-param-dflt-yield");

    [Fact(DisplayName = "cannot-chain-head-with-logical-and.js")]
    public Task test_coalesce_cannot_chain_head_with_logical_and()
        => CompilationFailureTest("coalesce/cannot-chain-head-with-logical-and");

    [Fact(DisplayName = "cannot-chain-head-with-logical-or.js")]
    public Task test_coalesce_cannot_chain_head_with_logical_or()
        => CompilationFailureTest("coalesce/cannot-chain-head-with-logical-or");

    [Fact(DisplayName = "cannot-chain-tail-with-logical-and.js")]
    public Task test_coalesce_cannot_chain_tail_with_logical_and()
        => CompilationFailureTest("coalesce/cannot-chain-tail-with-logical-and");

    [Fact(DisplayName = "cannot-chain-tail-with-logical-or.js")]
    public Task test_coalesce_cannot_chain_tail_with_logical_or()
        => CompilationFailureTest("coalesce/cannot-chain-tail-with-logical-or");

    [Fact(DisplayName = "in-branch-2.js")]
    public Task test_conditional_in_branch_2()
        => CompilationFailureTest("conditional/in-branch-2");

    [Fact(DisplayName = "in-condition.js")]
    public Task test_conditional_in_condition()
        => CompilationFailureTest("conditional/in-condition");

    [Fact(DisplayName = "identifier-strict-recursive.js")]
    public Task test_delete_identifier_strict_recursive()
        => CompilationFailureTest("delete/identifier-strict-recursive");

    [Fact(DisplayName = "identifier-strict.js")]
    public Task test_delete_identifier_strict()
        => CompilationFailureTest("delete/identifier-strict");

    [Fact(DisplayName = "escape-sequence-import.js")]
    public Task test_dynamic_import_escape_sequence_import()
        => CompilationFailureTest("dynamic-import/escape-sequence-import");

    [Fact(DisplayName = "2nd-param-yield-ident-invalid.js")]
    public Task test_dynamic_import_import_attributes_2nd_param_yield_ident_invalid()
        => CompilationFailureTest("dynamic-import/import-attributes/2nd-param-yield-ident-invalid");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-1-update-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_1_update_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-1-update-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-10-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_10_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-10-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-11-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_11_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-11-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-12-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_12_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-12-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-13-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_13_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-13-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-14-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_14_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-14-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-15-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_15_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-15-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-16-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_16_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-16-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-17-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_17_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-17-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-2-update-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_2_update_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-2-update-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-3-update-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_3_update_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-3-update-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-4-update-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_4_update_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-4-update-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-5-lhs-equals-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_5_lhs_equals_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-5-lhs-equals-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-6-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_6_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-6-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-7-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_7_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-7-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-8-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_8_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-8-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "invalid-assignmenttargettype-syntax-error-9-lhs-assignment-operator-assignment-expression.js")]
    public Task test_dynamic_import_syntax_invalid_invalid_assignmenttargettype_syntax_error_9_lhs_assignment_operator_assignment_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/invalid-assignmenttargettype-syntax-error-9-lhs-assignment-operator-assignment-expression");

    [Fact(DisplayName = "nested-arrow-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-arrow-assignment-expression-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-import-call-unknown");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-import-source-no-rest-param");

    [Fact(DisplayName = "nested-arrow-assignment-expression-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-no-new-call-expression");

    [Fact(DisplayName = "nested-arrow-assignment-expression-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-no-rest-param");

    [Fact(DisplayName = "nested-arrow-assignment-expression-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-not-extensible-args");

    [Fact(DisplayName = "nested-arrow-assignment-expression-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-arrow-assignment-expression-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-typeof-import-source");

    [Fact(DisplayName = "nested-arrow-assignment-expression-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_assignment_expression_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-assignment-expression-typeof-import");

    [Fact(DisplayName = "nested-arrow-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-import-call-unknown");

    [Fact(DisplayName = "nested-arrow-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-arrow-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-arrow-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-arrow-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-arrow-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-arrow-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-import-source-no-rest-param");

    [Fact(DisplayName = "nested-arrow-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-no-new-call-expression");

    [Fact(DisplayName = "nested-arrow-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-no-rest-param");

    [Fact(DisplayName = "nested-arrow-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-not-extensible-args");

    [Fact(DisplayName = "nested-arrow-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-arrow-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-typeof-import-source");

    [Fact(DisplayName = "nested-arrow-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_arrow_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-arrow-typeof-import");

    [Fact(DisplayName = "nested-async-arrow-function-await-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-import-call-unknown");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-import-source-no-rest-param");

    [Fact(DisplayName = "nested-async-arrow-function-await-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-no-new-call-expression");

    [Fact(DisplayName = "nested-async-arrow-function-await-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-no-rest-param");

    [Fact(DisplayName = "nested-async-arrow-function-await-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-not-extensible-args");

    [Fact(DisplayName = "nested-async-arrow-function-await-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-async-arrow-function-await-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-typeof-import-source");

    [Fact(DisplayName = "nested-async-arrow-function-await-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_await_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-await-typeof-import");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-import-call-unknown");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-import-source-no-rest-param");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-no-new-call-expression");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-no-rest-param");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-not-extensible-args");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-typeof-import-source");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_arrow_function_return_await_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-arrow-function-return-await-typeof-import");

    [Fact(DisplayName = "nested-async-function-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-await-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-await-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-import-call-unknown");

    [Fact(DisplayName = "nested-async-function-await-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-await-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-await-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-async-function-await-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-await-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-await-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-import-source-no-rest-param");

    [Fact(DisplayName = "nested-async-function-await-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-await-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-no-rest-param");

    [Fact(DisplayName = "nested-async-function-await-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-not-extensible-args");

    [Fact(DisplayName = "nested-async-function-await-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-async-function-await-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-typeof-import-source");

    [Fact(DisplayName = "nested-async-function-await-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_await_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-await-typeof-import");

    [Fact(DisplayName = "nested-async-function-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-import-call-unknown");

    [Fact(DisplayName = "nested-async-function-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-async-function-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-import-source-no-rest-param");

    [Fact(DisplayName = "nested-async-function-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-no-rest-param");

    [Fact(DisplayName = "nested-async-function-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-not-extensible-args");

    [Fact(DisplayName = "nested-async-function-return-await-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-return-await-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-import-call-unknown");

    [Fact(DisplayName = "nested-async-function-return-await-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-return-await-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-return-await-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-async-function-return-await-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-function-return-await-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-return-await-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-import-source-no-rest-param");

    [Fact(DisplayName = "nested-async-function-return-await-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-no-new-call-expression");

    [Fact(DisplayName = "nested-async-function-return-await-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-no-rest-param");

    [Fact(DisplayName = "nested-async-function-return-await-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-not-extensible-args");

    [Fact(DisplayName = "nested-async-function-return-await-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-async-function-return-await-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-typeof-import-source");

    [Fact(DisplayName = "nested-async-function-return-await-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_return_await_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-return-await-typeof-import");

    [Fact(DisplayName = "nested-async-function-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-async-function-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-typeof-import-source");

    [Fact(DisplayName = "nested-async-function-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_function_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-function-typeof-import");

    [Fact(DisplayName = "nested-async-gen-await-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-gen-await-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-import-call-unknown");

    [Fact(DisplayName = "nested-async-gen-await-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-gen-await-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-async-gen-await-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-async-gen-await-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-async-gen-await-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-async-gen-await-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-import-source-no-rest-param");

    [Fact(DisplayName = "nested-async-gen-await-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-no-new-call-expression");

    [Fact(DisplayName = "nested-async-gen-await-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-no-rest-param");

    [Fact(DisplayName = "nested-async-gen-await-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-not-extensible-args");

    [Fact(DisplayName = "nested-async-gen-await-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-async-gen-await-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-typeof-import-source");

    [Fact(DisplayName = "nested-async-gen-await-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_async_gen_await_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-async-gen-await-typeof-import");

    [Fact(DisplayName = "nested-block-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-block-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-import-call-unknown");

    [Fact(DisplayName = "nested-block-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-block-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-block-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-block-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-block-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-block-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-import-source-no-rest-param");

    [Fact(DisplayName = "nested-block-labeled-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-block-labeled-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-import-call-unknown");

    [Fact(DisplayName = "nested-block-labeled-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-block-labeled-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-block-labeled-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-block-labeled-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-block-labeled-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-block-labeled-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-import-source-no-rest-param");

    [Fact(DisplayName = "nested-block-labeled-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-no-new-call-expression");

    [Fact(DisplayName = "nested-block-labeled-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-no-rest-param");

    [Fact(DisplayName = "nested-block-labeled-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-not-extensible-args");

    [Fact(DisplayName = "nested-block-labeled-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-block-labeled-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-typeof-import-source");

    [Fact(DisplayName = "nested-block-labeled-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_labeled_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-labeled-typeof-import");

    [Fact(DisplayName = "nested-block-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-no-new-call-expression");

    [Fact(DisplayName = "nested-block-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-no-rest-param");

    [Fact(DisplayName = "nested-block-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-not-extensible-args");

    [Fact(DisplayName = "nested-block-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-block-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-typeof-import-source");

    [Fact(DisplayName = "nested-block-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_block_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-block-typeof-import");

    [Fact(DisplayName = "nested-do-while-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-do-while-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-import-call-unknown");

    [Fact(DisplayName = "nested-do-while-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-do-while-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-do-while-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-do-while-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-do-while-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-do-while-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-import-source-no-rest-param");

    [Fact(DisplayName = "nested-do-while-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-no-new-call-expression");

    [Fact(DisplayName = "nested-do-while-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-no-rest-param");

    [Fact(DisplayName = "nested-do-while-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-not-extensible-args");

    [Fact(DisplayName = "nested-do-while-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-do-while-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-typeof-import-source");

    [Fact(DisplayName = "nested-do-while-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_do_while_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-do-while-typeof-import");

    [Fact(DisplayName = "nested-else-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-else-braceless-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-else-braceless-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-import-call-unknown");

    [Fact(DisplayName = "nested-else-braceless-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-else-braceless-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-else-braceless-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-else-braceless-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-else-braceless-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-else-braceless-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-import-source-no-rest-param");

    [Fact(DisplayName = "nested-else-braceless-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-no-new-call-expression");

    [Fact(DisplayName = "nested-else-braceless-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-no-rest-param");

    [Fact(DisplayName = "nested-else-braceless-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-not-extensible-args");

    [Fact(DisplayName = "nested-else-braceless-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-else-braceless-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-typeof-import-source");

    [Fact(DisplayName = "nested-else-braceless-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_braceless_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-braceless-typeof-import");

    [Fact(DisplayName = "nested-else-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-import-call-unknown");

    [Fact(DisplayName = "nested-else-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-else-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-else-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-else-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-else-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-else-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-import-source-no-rest-param");

    [Fact(DisplayName = "nested-else-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-no-new-call-expression");

    [Fact(DisplayName = "nested-else-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-no-rest-param");

    [Fact(DisplayName = "nested-else-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-not-extensible-args");

    [Fact(DisplayName = "nested-else-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-else-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-typeof-import-source");

    [Fact(DisplayName = "nested-else-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_else_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-else-typeof-import");

    [Fact(DisplayName = "nested-function-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-function-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-import-call-unknown");

    [Fact(DisplayName = "nested-function-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-function-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-function-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-function-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-function-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-function-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-import-source-no-rest-param");

    [Fact(DisplayName = "nested-function-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-no-new-call-expression");

    [Fact(DisplayName = "nested-function-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-no-rest-param");

    [Fact(DisplayName = "nested-function-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-not-extensible-args");

    [Fact(DisplayName = "nested-function-return-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-function-return-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-import-call-unknown");

    [Fact(DisplayName = "nested-function-return-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-function-return-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-function-return-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-function-return-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-function-return-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-function-return-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-import-source-no-rest-param");

    [Fact(DisplayName = "nested-function-return-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-no-new-call-expression");

    [Fact(DisplayName = "nested-function-return-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-no-rest-param");

    [Fact(DisplayName = "nested-function-return-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-not-extensible-args");

    [Fact(DisplayName = "nested-function-return-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-function-return-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-typeof-import-source");

    [Fact(DisplayName = "nested-function-return-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_return_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-return-typeof-import");

    [Fact(DisplayName = "nested-function-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-function-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-typeof-import-source");

    [Fact(DisplayName = "nested-function-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_function_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-function-typeof-import");

    [Fact(DisplayName = "nested-if-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-if-braceless-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-if-braceless-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-import-call-unknown");

    [Fact(DisplayName = "nested-if-braceless-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-if-braceless-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-if-braceless-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-if-braceless-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-if-braceless-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-if-braceless-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-import-source-no-rest-param");

    [Fact(DisplayName = "nested-if-braceless-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-no-new-call-expression");

    [Fact(DisplayName = "nested-if-braceless-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-no-rest-param");

    [Fact(DisplayName = "nested-if-braceless-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-not-extensible-args");

    [Fact(DisplayName = "nested-if-braceless-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-if-braceless-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-typeof-import-source");

    [Fact(DisplayName = "nested-if-braceless-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_braceless_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-braceless-typeof-import");

    [Fact(DisplayName = "nested-if-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-import-call-unknown");

    [Fact(DisplayName = "nested-if-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-if-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-if-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-if-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-if-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-if-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-import-source-no-rest-param");

    [Fact(DisplayName = "nested-if-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-no-new-call-expression");

    [Fact(DisplayName = "nested-if-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-no-rest-param");

    [Fact(DisplayName = "nested-if-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-not-extensible-args");

    [Fact(DisplayName = "nested-if-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-if-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-typeof-import-source");

    [Fact(DisplayName = "nested-if-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_if_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-if-typeof-import");

    [Fact(DisplayName = "nested-while-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-while-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-import-call-unknown");

    [Fact(DisplayName = "nested-while-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-while-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-while-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-while-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-while-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-while-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-import-source-no-rest-param");

    [Fact(DisplayName = "nested-while-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-no-new-call-expression");

    [Fact(DisplayName = "nested-while-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-no-rest-param");

    [Fact(DisplayName = "nested-while-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-not-extensible-args");

    [Fact(DisplayName = "nested-while-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-while-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-typeof-import-source");

    [Fact(DisplayName = "nested-while-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_while_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-while-typeof-import");

    [Fact(DisplayName = "nested-with-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-with-expression-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-with-expression-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-import-call-unknown");

    [Fact(DisplayName = "nested-with-expression-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-with-expression-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-with-expression-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-with-expression-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-with-expression-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-with-expression-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-import-source-no-rest-param");

    [Fact(DisplayName = "nested-with-expression-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-no-new-call-expression");

    [Fact(DisplayName = "nested-with-expression-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-no-rest-param");

    [Fact(DisplayName = "nested-with-expression-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-not-extensible-args");

    [Fact(DisplayName = "nested-with-expression-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-with-expression-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-typeof-import-source");

    [Fact(DisplayName = "nested-with-expression-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_expression_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-expression-typeof-import");

    [Fact(DisplayName = "nested-with-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-import-call-unknown");

    [Fact(DisplayName = "nested-with-import-defer-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_import_defer_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-import-defer-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-with-import-defer-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_import_defer_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-import-defer-no-new-call-expression");

    [Fact(DisplayName = "nested-with-import-defer-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_import_defer_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-import-defer-no-rest-param");

    [Fact(DisplayName = "nested-with-import-source-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_import_source_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-import-source-assignment-expr-not-optional");

    [Fact(DisplayName = "nested-with-import-source-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_import_source_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-import-source-no-new-call-expression");

    [Fact(DisplayName = "nested-with-import-source-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_import_source_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-import-source-no-rest-param");

    [Fact(DisplayName = "nested-with-no-new-call-expression.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_no_new_call_expression()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-no-new-call-expression");

    [Fact(DisplayName = "nested-with-no-rest-param.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_no_rest_param()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-no-rest-param");

    [Fact(DisplayName = "nested-with-not-extensible-args.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_not_extensible_args()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-not-extensible-args");

    [Fact(DisplayName = "nested-with-typeof-import-call-source-property.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_typeof_import_call_source_property()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-typeof-import-call-source-property");

    [Fact(DisplayName = "nested-with-typeof-import-source.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_typeof_import_source()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-typeof-import-source");

    [Fact(DisplayName = "nested-with-typeof-import.js")]
    public Task test_dynamic_import_syntax_invalid_nested_with_typeof_import()
        => CompilationFailureTest("dynamic-import/syntax/invalid/nested-with-typeof-import");

    [Fact(DisplayName = "top-level-assignment-expr-not-optional.js")]
    public Task test_dynamic_import_syntax_invalid_top_level_assignment_expr_not_optional()
        => CompilationFailureTest("dynamic-import/syntax/invalid/top-level-assignment-expr-not-optional");

    [Fact(DisplayName = "top-level-import-call-unknown.js")]
    public Task test_dynamic_import_syntax_invalid_top_level_import_call_unknown()
        => CompilationFailureTest("dynamic-import/syntax/invalid/top-level-import-call-unknown");

}
