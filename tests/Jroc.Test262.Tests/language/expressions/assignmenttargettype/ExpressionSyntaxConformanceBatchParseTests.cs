using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.assignmenttargettype;

public class ExpressionSyntaxConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformanceBatchParseTests() : base("language/expressions/assignmenttargettype", "language.expressions.assignmenttargettype") { }

    [Fact(DisplayName = "direct-lefthandsideexpression-plus-plus.js")]
    public Task direct_lefthandsideexpression_plus_plus()
        => CompilationFailureTest("direct-lefthandsideexpression-plus-plus");

    [Fact(DisplayName = "direct-logicalandexpression-logical-and-bitwiseorexpression-0.js")]
    public Task direct_logicalandexpression_logical_and_bitwiseorexpression_0()
        => CompilationFailureTest("direct-logicalandexpression-logical-and-bitwiseorexpression-0");

    [Fact(DisplayName = "direct-logicalandexpression-logical-and-bitwiseorexpression-1.js")]
    public Task direct_logicalandexpression_logical_and_bitwiseorexpression_1()
        => CompilationFailureTest("direct-logicalandexpression-logical-and-bitwiseorexpression-1");

    [Fact(DisplayName = "direct-logicalandexpression-logical-and-bitwiseorexpression-2.js")]
    public Task direct_logicalandexpression_logical_and_bitwiseorexpression_2()
        => CompilationFailureTest("direct-logicalandexpression-logical-and-bitwiseorexpression-2");

    [Fact(DisplayName = "direct-logicalorexpression-logical-or-logicalandexpression-0.js")]
    public Task direct_logicalorexpression_logical_or_logicalandexpression_0()
        => CompilationFailureTest("direct-logicalorexpression-logical-or-logicalandexpression-0");

    [Fact(DisplayName = "direct-logicalorexpression-logical-or-logicalandexpression-1.js")]
    public Task direct_logicalorexpression_logical_or_logicalandexpression_1()
        => CompilationFailureTest("direct-logicalorexpression-logical-or-logicalandexpression-1");

    [Fact(DisplayName = "direct-logicalorexpression-logical-or-logicalandexpression-2.js")]
    public Task direct_logicalorexpression_logical_or_logicalandexpression_2()
        => CompilationFailureTest("direct-logicalorexpression-logical-or-logicalandexpression-2");

    [Fact(DisplayName = "direct-memberexpression-templateliteral.js")]
    public Task direct_memberexpression_templateliteral()
        => CompilationFailureTest("direct-memberexpression-templateliteral");

    [Fact(DisplayName = "direct-minus-minus-unaryexpression.js")]
    public Task direct_minus_minus_unaryexpression()
        => CompilationFailureTest("direct-minus-minus-unaryexpression");

    [Fact(DisplayName = "direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-0.js")]
    public Task direct_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_0()
        => CompilationFailureTest("direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-0");

    [Fact(DisplayName = "direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-1.js")]
    public Task direct_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_1()
        => CompilationFailureTest("direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-1");

    [Fact(DisplayName = "direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-2.js")]
    public Task direct_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_2()
        => CompilationFailureTest("direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-2");

    [Fact(DisplayName = "direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-3.js")]
    public Task direct_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_3()
        => CompilationFailureTest("direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-3");

    [Fact(DisplayName = "direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-4.js")]
    public Task direct_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_4()
        => CompilationFailureTest("direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-4");

    [Fact(DisplayName = "direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-5.js")]
    public Task direct_multiplicativeexpression_multiplicativeoperator_exponentiationexpression_5()
        => CompilationFailureTest("direct-multiplicativeexpression-multiplicativeoperator-exponentiationexpression-5");

    [Fact(DisplayName = "direct-new-memberexpression-arguments.js")]
    public Task direct_new_memberexpression_arguments()
        => CompilationFailureTest("direct-new-memberexpression-arguments");

    [Fact(DisplayName = "direct-new-newexpression.js")]
    public Task direct_new_newexpression()
        => CompilationFailureTest("direct-new-newexpression");

    [Fact(DisplayName = "direct-optionalexpression.js")]
    public Task direct_optionalexpression()
        => CompilationFailureTest("direct-optionalexpression");

    [Fact(DisplayName = "direct-plus-plus-unaryexpression.js")]
    public Task direct_plus_plus_unaryexpression()
        => CompilationFailureTest("direct-plus-plus-unaryexpression");

    [Fact(DisplayName = "direct-primaryexpression-asyncfunctionexpression.js")]
    public Task direct_primaryexpression_asyncfunctionexpression()
        => CompilationFailureTest("direct-primaryexpression-asyncfunctionexpression");

    [Fact(DisplayName = "direct-primaryexpression-asyncgeneratorexpression.js")]
    public Task direct_primaryexpression_asyncgeneratorexpression()
        => CompilationFailureTest("direct-primaryexpression-asyncgeneratorexpression");

    [Fact(DisplayName = "direct-primaryexpression-classexpression.js")]
    public Task direct_primaryexpression_classexpression()
        => CompilationFailureTest("direct-primaryexpression-classexpression");

    [Fact(DisplayName = "direct-primaryexpression-functionexpression.js")]
    public Task direct_primaryexpression_functionexpression()
        => CompilationFailureTest("direct-primaryexpression-functionexpression");

    [Fact(DisplayName = "direct-primaryexpression-generatorexpression.js")]
    public Task direct_primaryexpression_generatorexpression()
        => CompilationFailureTest("direct-primaryexpression-generatorexpression");

    [Fact(DisplayName = "direct-primaryexpression-literal-boolean.js")]
    public Task direct_primaryexpression_literal_boolean()
        => CompilationFailureTest("direct-primaryexpression-literal-boolean");

    [Fact(DisplayName = "direct-primaryexpression-literal-null.js")]
    public Task direct_primaryexpression_literal_null()
        => CompilationFailureTest("direct-primaryexpression-literal-null");

    [Fact(DisplayName = "direct-primaryexpression-literal-numeric.js")]
    public Task direct_primaryexpression_literal_numeric()
        => CompilationFailureTest("direct-primaryexpression-literal-numeric");

    [Fact(DisplayName = "direct-primaryexpression-literal-string.js")]
    public Task direct_primaryexpression_literal_string()
        => CompilationFailureTest("direct-primaryexpression-literal-string");

    [Fact(DisplayName = "direct-primaryexpression-objectliteral.js")]
    public Task direct_primaryexpression_objectliteral()
        => CompilationFailureTest("direct-primaryexpression-objectliteral");

    [Fact(DisplayName = "direct-primaryexpression-regularexpressionliteral.js")]
    public Task direct_primaryexpression_regularexpressionliteral()
        => CompilationFailureTest("direct-primaryexpression-regularexpressionliteral");

    [Fact(DisplayName = "direct-primaryexpression-templateliteral.js")]
    public Task direct_primaryexpression_templateliteral()
        => CompilationFailureTest("direct-primaryexpression-templateliteral");

    [Fact(DisplayName = "direct-primaryexpression-this.js")]
    public Task direct_primaryexpression_this()
        => CompilationFailureTest("direct-primaryexpression-this");

    [Fact(DisplayName = "direct-relationalexpression-greater-than-or-equal-to-shiftexpression-0.js")]
    public Task direct_relationalexpression_greater_than_or_equal_to_shiftexpression_0()
        => CompilationFailureTest("direct-relationalexpression-greater-than-or-equal-to-shiftexpression-0");

    [Fact(DisplayName = "direct-relationalexpression-greater-than-or-equal-to-shiftexpression-1.js")]
    public Task direct_relationalexpression_greater_than_or_equal_to_shiftexpression_1()
        => CompilationFailureTest("direct-relationalexpression-greater-than-or-equal-to-shiftexpression-1");

    [Fact(DisplayName = "direct-relationalexpression-greater-than-or-equal-to-shiftexpression-2.js")]
    public Task direct_relationalexpression_greater_than_or_equal_to_shiftexpression_2()
        => CompilationFailureTest("direct-relationalexpression-greater-than-or-equal-to-shiftexpression-2");

    [Fact(DisplayName = "direct-relationalexpression-greater-than-shiftexpression-0.js")]
    public Task direct_relationalexpression_greater_than_shiftexpression_0()
        => CompilationFailureTest("direct-relationalexpression-greater-than-shiftexpression-0");

    [Fact(DisplayName = "direct-relationalexpression-greater-than-shiftexpression-1.js")]
    public Task direct_relationalexpression_greater_than_shiftexpression_1()
        => CompilationFailureTest("direct-relationalexpression-greater-than-shiftexpression-1");

    [Fact(DisplayName = "direct-relationalexpression-greater-than-shiftexpression-2.js")]
    public Task direct_relationalexpression_greater_than_shiftexpression_2()
        => CompilationFailureTest("direct-relationalexpression-greater-than-shiftexpression-2");

    [Fact(DisplayName = "direct-relationalexpression-in-shiftexpression-0.js")]
    public Task direct_relationalexpression_in_shiftexpression_0()
        => CompilationFailureTest("direct-relationalexpression-in-shiftexpression-0");

    [Fact(DisplayName = "direct-relationalexpression-in-shiftexpression-1.js")]
    public Task direct_relationalexpression_in_shiftexpression_1()
        => CompilationFailureTest("direct-relationalexpression-in-shiftexpression-1");

    [Fact(DisplayName = "direct-relationalexpression-in-shiftexpression-2.js")]
    public Task direct_relationalexpression_in_shiftexpression_2()
        => CompilationFailureTest("direct-relationalexpression-in-shiftexpression-2");

    [Fact(DisplayName = "direct-relationalexpression-instanceof-shiftexpression-0.js")]
    public Task direct_relationalexpression_instanceof_shiftexpression_0()
        => CompilationFailureTest("direct-relationalexpression-instanceof-shiftexpression-0");

    [Fact(DisplayName = "direct-relationalexpression-instanceof-shiftexpression-1.js")]
    public Task direct_relationalexpression_instanceof_shiftexpression_1()
        => CompilationFailureTest("direct-relationalexpression-instanceof-shiftexpression-1");

    [Fact(DisplayName = "direct-relationalexpression-instanceof-shiftexpression-2.js")]
    public Task direct_relationalexpression_instanceof_shiftexpression_2()
        => CompilationFailureTest("direct-relationalexpression-instanceof-shiftexpression-2");

    [Fact(DisplayName = "direct-relationalexpression-less-than-or-equal-to-shiftexpression-0.js")]
    public Task direct_relationalexpression_less_than_or_equal_to_shiftexpression_0()
        => CompilationFailureTest("direct-relationalexpression-less-than-or-equal-to-shiftexpression-0");

    [Fact(DisplayName = "direct-relationalexpression-less-than-or-equal-to-shiftexpression-1.js")]
    public Task direct_relationalexpression_less_than_or_equal_to_shiftexpression_1()
        => CompilationFailureTest("direct-relationalexpression-less-than-or-equal-to-shiftexpression-1");

    [Fact(DisplayName = "direct-relationalexpression-less-than-or-equal-to-shiftexpression-2.js")]
    public Task direct_relationalexpression_less_than_or_equal_to_shiftexpression_2()
        => CompilationFailureTest("direct-relationalexpression-less-than-or-equal-to-shiftexpression-2");

    [Fact(DisplayName = "direct-relationalexpression-less-than-shiftexpression-0.js")]
    public Task direct_relationalexpression_less_than_shiftexpression_0()
        => CompilationFailureTest("direct-relationalexpression-less-than-shiftexpression-0");

    [Fact(DisplayName = "direct-relationalexpression-less-than-shiftexpression-1.js")]
    public Task direct_relationalexpression_less_than_shiftexpression_1()
        => CompilationFailureTest("direct-relationalexpression-less-than-shiftexpression-1");

    [Fact(DisplayName = "direct-relationalexpression-less-than-shiftexpression-2.js")]
    public Task direct_relationalexpression_less_than_shiftexpression_2()
        => CompilationFailureTest("direct-relationalexpression-less-than-shiftexpression-2");

    [Fact(DisplayName = "direct-shiftexpression-bitwise-left-additiveexpression-0.js")]
    public Task direct_shiftexpression_bitwise_left_additiveexpression_0()
        => CompilationFailureTest("direct-shiftexpression-bitwise-left-additiveexpression-0");

    [Fact(DisplayName = "direct-shiftexpression-bitwise-left-additiveexpression-1.js")]
    public Task direct_shiftexpression_bitwise_left_additiveexpression_1()
        => CompilationFailureTest("direct-shiftexpression-bitwise-left-additiveexpression-1");

    [Fact(DisplayName = "direct-shiftexpression-bitwise-left-additiveexpression-2.js")]
    public Task direct_shiftexpression_bitwise_left_additiveexpression_2()
        => CompilationFailureTest("direct-shiftexpression-bitwise-left-additiveexpression-2");

    [Fact(DisplayName = "direct-shiftexpression-bitwise-right-additiveexpression-0.js")]
    public Task direct_shiftexpression_bitwise_right_additiveexpression_0()
        => CompilationFailureTest("direct-shiftexpression-bitwise-right-additiveexpression-0");

    [Fact(DisplayName = "direct-shiftexpression-bitwise-right-additiveexpression-1.js")]
    public Task direct_shiftexpression_bitwise_right_additiveexpression_1()
        => CompilationFailureTest("direct-shiftexpression-bitwise-right-additiveexpression-1");

    [Fact(DisplayName = "direct-shiftexpression-bitwise-right-additiveexpression-2.js")]
    public Task direct_shiftexpression_bitwise_right_additiveexpression_2()
        => CompilationFailureTest("direct-shiftexpression-bitwise-right-additiveexpression-2");

    [Fact(DisplayName = "direct-shiftexpression-unsigned-bitwise-right-additiveexpression-0.js")]
    public Task direct_shiftexpression_unsigned_bitwise_right_additiveexpression_0()
        => CompilationFailureTest("direct-shiftexpression-unsigned-bitwise-right-additiveexpression-0");

    [Fact(DisplayName = "direct-shiftexpression-unsigned-bitwise-right-additiveexpression-1.js")]
    public Task direct_shiftexpression_unsigned_bitwise_right_additiveexpression_1()
        => CompilationFailureTest("direct-shiftexpression-unsigned-bitwise-right-additiveexpression-1");

    [Fact(DisplayName = "direct-shiftexpression-unsigned-bitwise-right-additiveexpression-2.js")]
    public Task direct_shiftexpression_unsigned_bitwise_right_additiveexpression_2()
        => CompilationFailureTest("direct-shiftexpression-unsigned-bitwise-right-additiveexpression-2");

    [Fact(DisplayName = "direct-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-0.js")]
    public Task direct_shortcircuitexpression_question_assignmentexpression_else_assignmentexpression_0()
        => CompilationFailureTest("direct-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-0");

    [Fact(DisplayName = "direct-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-1.js")]
    public Task direct_shortcircuitexpression_question_assignmentexpression_else_assignmentexpression_1()
        => CompilationFailureTest("direct-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-1");

    [Fact(DisplayName = "direct-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-2.js")]
    public Task direct_shortcircuitexpression_question_assignmentexpression_else_assignmentexpression_2()
        => CompilationFailureTest("direct-shortcircuitexpression-question-assignmentexpression-else-assignmentexpression-2");

    [Fact(DisplayName = "direct-unaryexpression-delete-unaryexpression.js")]
    public Task direct_unaryexpression_delete_unaryexpression()
        => CompilationFailureTest("direct-unaryexpression-delete-unaryexpression");

    [Fact(DisplayName = "direct-unaryexpression-exclamation-unaryexpression-0.js")]
    public Task direct_unaryexpression_exclamation_unaryexpression_0()
        => CompilationFailureTest("direct-unaryexpression-exclamation-unaryexpression-0");

    [Fact(DisplayName = "direct-unaryexpression-exclamation-unaryexpression-1.js")]
    public Task direct_unaryexpression_exclamation_unaryexpression_1()
        => CompilationFailureTest("direct-unaryexpression-exclamation-unaryexpression-1");

    [Fact(DisplayName = "direct-unaryexpression-exclamation-unaryexpression-2.js")]
    public Task direct_unaryexpression_exclamation_unaryexpression_2()
        => CompilationFailureTest("direct-unaryexpression-exclamation-unaryexpression-2");

    [Fact(DisplayName = "direct-unaryexpression-minus-unaryexpression-0.js")]
    public Task direct_unaryexpression_minus_unaryexpression_0()
        => CompilationFailureTest("direct-unaryexpression-minus-unaryexpression-0");

    [Fact(DisplayName = "direct-unaryexpression-minus-unaryexpression-1.js")]
    public Task direct_unaryexpression_minus_unaryexpression_1()
        => CompilationFailureTest("direct-unaryexpression-minus-unaryexpression-1");

    [Fact(DisplayName = "direct-unaryexpression-minus-unaryexpression-2.js")]
    public Task direct_unaryexpression_minus_unaryexpression_2()
        => CompilationFailureTest("direct-unaryexpression-minus-unaryexpression-2");

    [Fact(DisplayName = "direct-unaryexpression-plus-unaryexpression-0.js")]
    public Task direct_unaryexpression_plus_unaryexpression_0()
        => CompilationFailureTest("direct-unaryexpression-plus-unaryexpression-0");

    [Fact(DisplayName = "direct-unaryexpression-plus-unaryexpression-1.js")]
    public Task direct_unaryexpression_plus_unaryexpression_1()
        => CompilationFailureTest("direct-unaryexpression-plus-unaryexpression-1");

    [Fact(DisplayName = "direct-unaryexpression-plus-unaryexpression-2.js")]
    public Task direct_unaryexpression_plus_unaryexpression_2()
        => CompilationFailureTest("direct-unaryexpression-plus-unaryexpression-2");

    [Fact(DisplayName = "direct-unaryexpression-tilde-unaryexpression-0.js")]
    public Task direct_unaryexpression_tilde_unaryexpression_0()
        => CompilationFailureTest("direct-unaryexpression-tilde-unaryexpression-0");

    [Fact(DisplayName = "direct-unaryexpression-tilde-unaryexpression-1.js")]
    public Task direct_unaryexpression_tilde_unaryexpression_1()
        => CompilationFailureTest("direct-unaryexpression-tilde-unaryexpression-1");

    [Fact(DisplayName = "direct-unaryexpression-tilde-unaryexpression-2.js")]
    public Task direct_unaryexpression_tilde_unaryexpression_2()
        => CompilationFailureTest("direct-unaryexpression-tilde-unaryexpression-2");

    [Fact(DisplayName = "direct-unaryexpression-typeof-unaryexpression-0.js")]
    public Task direct_unaryexpression_typeof_unaryexpression_0()
        => CompilationFailureTest("direct-unaryexpression-typeof-unaryexpression-0");

    [Fact(DisplayName = "direct-unaryexpression-typeof-unaryexpression-1.js")]
    public Task direct_unaryexpression_typeof_unaryexpression_1()
        => CompilationFailureTest("direct-unaryexpression-typeof-unaryexpression-1");

    [Fact(DisplayName = "direct-unaryexpression-typeof-unaryexpression-2.js")]
    public Task direct_unaryexpression_typeof_unaryexpression_2()
        => CompilationFailureTest("direct-unaryexpression-typeof-unaryexpression-2");

    [Fact(DisplayName = "direct-unaryexpression-void-unaryexpression-0.js")]
    public Task direct_unaryexpression_void_unaryexpression_0()
        => CompilationFailureTest("direct-unaryexpression-void-unaryexpression-0");

    [Fact(DisplayName = "direct-unaryexpression-void-unaryexpression-1.js")]
    public Task direct_unaryexpression_void_unaryexpression_1()
        => CompilationFailureTest("direct-unaryexpression-void-unaryexpression-1");

    [Fact(DisplayName = "direct-unaryexpression-void-unaryexpression-2.js")]
    public Task direct_unaryexpression_void_unaryexpression_2()
        => CompilationFailureTest("direct-unaryexpression-void-unaryexpression-2");

    [Fact(DisplayName = "direct-updateexpression-star-star-exponentiationexpression-0.js")]
    public Task direct_updateexpression_star_star_exponentiationexpression_0()
        => CompilationFailureTest("direct-updateexpression-star-star-exponentiationexpression-0");

    [Fact(DisplayName = "direct-updateexpression-star-star-exponentiationexpression-1.js")]
    public Task direct_updateexpression_star_star_exponentiationexpression_1()
        => CompilationFailureTest("direct-updateexpression-star-star-exponentiationexpression-1");

    [Fact(DisplayName = "direct-updateexpression-star-star-exponentiationexpression-2.js")]
    public Task direct_updateexpression_star_star_exponentiationexpression_2()
        => CompilationFailureTest("direct-updateexpression-star-star-exponentiationexpression-2");

    [Fact(DisplayName = "direct-yieldexpression-0.js")]
    public Task direct_yieldexpression_0()
        => CompilationFailureTest("direct-yieldexpression-0");

    [Fact(DisplayName = "direct-yieldexpression-1.js")]
    public Task direct_yieldexpression_1()
        => CompilationFailureTest("direct-yieldexpression-1");

    [Fact(DisplayName = "parenthesized-additiveexpression-minus-multiplicativeexpression-0.js")]
    public Task parenthesized_additiveexpression_minus_multiplicativeexpression_0()
        => CompilationFailureTest("parenthesized-additiveexpression-minus-multiplicativeexpression-0");

    [Fact(DisplayName = "parenthesized-additiveexpression-minus-multiplicativeexpression-1.js")]
    public Task parenthesized_additiveexpression_minus_multiplicativeexpression_1()
        => CompilationFailureTest("parenthesized-additiveexpression-minus-multiplicativeexpression-1");

    [Fact(DisplayName = "parenthesized-additiveexpression-minus-multiplicativeexpression-2.js")]
    public Task parenthesized_additiveexpression_minus_multiplicativeexpression_2()
        => CompilationFailureTest("parenthesized-additiveexpression-minus-multiplicativeexpression-2");

    [Fact(DisplayName = "parenthesized-additiveexpression-plus-multiplicativeexpression-0.js")]
    public Task parenthesized_additiveexpression_plus_multiplicativeexpression_0()
        => CompilationFailureTest("parenthesized-additiveexpression-plus-multiplicativeexpression-0");

    [Fact(DisplayName = "parenthesized-additiveexpression-plus-multiplicativeexpression-1.js")]
    public Task parenthesized_additiveexpression_plus_multiplicativeexpression_1()
        => CompilationFailureTest("parenthesized-additiveexpression-plus-multiplicativeexpression-1");

    [Fact(DisplayName = "parenthesized-additiveexpression-plus-multiplicativeexpression-2.js")]
    public Task parenthesized_additiveexpression_plus_multiplicativeexpression_2()
        => CompilationFailureTest("parenthesized-additiveexpression-plus-multiplicativeexpression-2");

    [Fact(DisplayName = "parenthesized-arrowfunction-0.js")]
    public Task parenthesized_arrowfunction_0()
        => CompilationFailureTest("parenthesized-arrowfunction-0");

    [Fact(DisplayName = "parenthesized-arrowfunction-1.js")]
    public Task parenthesized_arrowfunction_1()
        => CompilationFailureTest("parenthesized-arrowfunction-1");

    [Fact(DisplayName = "parenthesized-arrowfunction-2.js")]
    public Task parenthesized_arrowfunction_2()
        => CompilationFailureTest("parenthesized-arrowfunction-2");

    [Fact(DisplayName = "parenthesized-arrowfunction-3.js")]
    public Task parenthesized_arrowfunction_3()
        => CompilationFailureTest("parenthesized-arrowfunction-3");

    [Fact(DisplayName = "parenthesized-arrowfunction-4.js")]
    public Task parenthesized_arrowfunction_4()
        => CompilationFailureTest("parenthesized-arrowfunction-4");

    [Fact(DisplayName = "parenthesized-arrowfunction-5.js")]
    public Task parenthesized_arrowfunction_5()
        => CompilationFailureTest("parenthesized-arrowfunction-5");

    [Fact(DisplayName = "parenthesized-arrowfunction-6.js")]
    public Task parenthesized_arrowfunction_6()
        => CompilationFailureTest("parenthesized-arrowfunction-6");

    [Fact(DisplayName = "parenthesized-asyncarrowfunction-0.js")]
    public Task parenthesized_asyncarrowfunction_0()
        => CompilationFailureTest("parenthesized-asyncarrowfunction-0");

    [Fact(DisplayName = "parenthesized-asyncarrowfunction-1.js")]
    public Task parenthesized_asyncarrowfunction_1()
        => CompilationFailureTest("parenthesized-asyncarrowfunction-1");

    [Fact(DisplayName = "parenthesized-asyncarrowfunction-2.js")]
    public Task parenthesized_asyncarrowfunction_2()
        => CompilationFailureTest("parenthesized-asyncarrowfunction-2");

    [Fact(DisplayName = "parenthesized-asyncarrowfunction-3.js")]
    public Task parenthesized_asyncarrowfunction_3()
        => CompilationFailureTest("parenthesized-asyncarrowfunction-3");

    [Fact(DisplayName = "parenthesized-asyncarrowfunction-4.js")]
    public Task parenthesized_asyncarrowfunction_4()
        => CompilationFailureTest("parenthesized-asyncarrowfunction-4");

    [Fact(DisplayName = "parenthesized-asyncarrowfunction-5.js")]
    public Task parenthesized_asyncarrowfunction_5()
        => CompilationFailureTest("parenthesized-asyncarrowfunction-5");

    [Fact(DisplayName = "parenthesized-asyncarrowfunction-6.js")]
    public Task parenthesized_asyncarrowfunction_6()
        => CompilationFailureTest("parenthesized-asyncarrowfunction-6");

    [Fact(DisplayName = "parenthesized-bitwiseandexpression-bitwise-and-equalityexpression-0.js")]
    public Task parenthesized_bitwiseandexpression_bitwise_and_equalityexpression_0()
        => CompilationFailureTest("parenthesized-bitwiseandexpression-bitwise-and-equalityexpression-0");

    [Fact(DisplayName = "parenthesized-bitwiseandexpression-bitwise-and-equalityexpression-1.js")]
    public Task parenthesized_bitwiseandexpression_bitwise_and_equalityexpression_1()
        => CompilationFailureTest("parenthesized-bitwiseandexpression-bitwise-and-equalityexpression-1");

    [Fact(DisplayName = "parenthesized-bitwiseandexpression-bitwise-and-equalityexpression-2.js")]
    public Task parenthesized_bitwiseandexpression_bitwise_and_equalityexpression_2()
        => CompilationFailureTest("parenthesized-bitwiseandexpression-bitwise-and-equalityexpression-2");

    [Fact(DisplayName = "parenthesized-bitwiseorexpression-bitwise-or-bitwisexorexpression-0.js")]
    public Task parenthesized_bitwiseorexpression_bitwise_or_bitwisexorexpression_0()
        => CompilationFailureTest("parenthesized-bitwiseorexpression-bitwise-or-bitwisexorexpression-0");

    [Fact(DisplayName = "parenthesized-bitwiseorexpression-bitwise-or-bitwisexorexpression-1.js")]
    public Task parenthesized_bitwiseorexpression_bitwise_or_bitwisexorexpression_1()
        => CompilationFailureTest("parenthesized-bitwiseorexpression-bitwise-or-bitwisexorexpression-1");

    [Fact(DisplayName = "parenthesized-bitwiseorexpression-bitwise-or-bitwisexorexpression-2.js")]
    public Task parenthesized_bitwiseorexpression_bitwise_or_bitwisexorexpression_2()
        => CompilationFailureTest("parenthesized-bitwiseorexpression-bitwise-or-bitwisexorexpression-2");

    [Fact(DisplayName = "parenthesized-bitwisexorexpression-bitwise-xor-bitwiseandexpression-0.js")]
    public Task parenthesized_bitwisexorexpression_bitwise_xor_bitwiseandexpression_0()
        => CompilationFailureTest("parenthesized-bitwisexorexpression-bitwise-xor-bitwiseandexpression-0");

    [Fact(DisplayName = "parenthesized-bitwisexorexpression-bitwise-xor-bitwiseandexpression-1.js")]
    public Task parenthesized_bitwisexorexpression_bitwise_xor_bitwiseandexpression_1()
        => CompilationFailureTest("parenthesized-bitwisexorexpression-bitwise-xor-bitwiseandexpression-1");

    [Fact(DisplayName = "parenthesized-bitwisexorexpression-bitwise-xor-bitwiseandexpression-2.js")]
    public Task parenthesized_bitwisexorexpression_bitwise_xor_bitwiseandexpression_2()
        => CompilationFailureTest("parenthesized-bitwisexorexpression-bitwise-xor-bitwiseandexpression-2");

    [Fact(DisplayName = "parenthesized-callexpression-in-compound-assignment.js")]
    public Task parenthesized_callexpression_in_compound_assignment()
        => CompilationFailureTest("parenthesized-callexpression-in-compound-assignment");

    [Fact(DisplayName = "parenthesized-callexpression-in-logical-assignment.js")]
    public Task parenthesized_callexpression_in_logical_assignment()
        => CompilationFailureTest("parenthesized-callexpression-in-logical-assignment");

    [Fact(DisplayName = "parenthesized-callexpression-templateliteral.js")]
    public Task parenthesized_callexpression_templateliteral()
        => CompilationFailureTest("parenthesized-callexpression-templateliteral");

    [Fact(DisplayName = "parenthesized-callexpression.js")]
    public Task parenthesized_callexpression()
        => CompilationFailureTest("parenthesized-callexpression");

    [Fact(DisplayName = "parenthesized-coalesceexpressionhead-coalesce-bitwiseorexpression-0.js")]
    public Task parenthesized_coalesceexpressionhead_coalesce_bitwiseorexpression_0()
        => CompilationFailureTest("parenthesized-coalesceexpressionhead-coalesce-bitwiseorexpression-0");

    [Fact(DisplayName = "parenthesized-coalesceexpressionhead-coalesce-bitwiseorexpression-1.js")]
    public Task parenthesized_coalesceexpressionhead_coalesce_bitwiseorexpression_1()
        => CompilationFailureTest("parenthesized-coalesceexpressionhead-coalesce-bitwiseorexpression-1");

    [Fact(DisplayName = "parenthesized-coalesceexpressionhead-coalesce-bitwiseorexpression-2.js")]
    public Task parenthesized_coalesceexpressionhead_coalesce_bitwiseorexpression_2()
        => CompilationFailureTest("parenthesized-coalesceexpressionhead-coalesce-bitwiseorexpression-2");

    [Fact(DisplayName = "parenthesized-equalityexpression-abstract-equal-relationalexpression-0.js")]
    public Task parenthesized_equalityexpression_abstract_equal_relationalexpression_0()
        => CompilationFailureTest("parenthesized-equalityexpression-abstract-equal-relationalexpression-0");

    [Fact(DisplayName = "parenthesized-equalityexpression-abstract-equal-relationalexpression-1.js")]
    public Task parenthesized_equalityexpression_abstract_equal_relationalexpression_1()
        => CompilationFailureTest("parenthesized-equalityexpression-abstract-equal-relationalexpression-1");

    [Fact(DisplayName = "parenthesized-equalityexpression-abstract-equal-relationalexpression-2.js")]
    public Task parenthesized_equalityexpression_abstract_equal_relationalexpression_2()
        => CompilationFailureTest("parenthesized-equalityexpression-abstract-equal-relationalexpression-2");

    [Fact(DisplayName = "parenthesized-equalityexpression-abstract-not-equal-relationalexpression-0.js")]
    public Task parenthesized_equalityexpression_abstract_not_equal_relationalexpression_0()
        => CompilationFailureTest("parenthesized-equalityexpression-abstract-not-equal-relationalexpression-0");

    [Fact(DisplayName = "parenthesized-equalityexpression-abstract-not-equal-relationalexpression-1.js")]
    public Task parenthesized_equalityexpression_abstract_not_equal_relationalexpression_1()
        => CompilationFailureTest("parenthesized-equalityexpression-abstract-not-equal-relationalexpression-1");

    [Fact(DisplayName = "parenthesized-equalityexpression-abstract-not-equal-relationalexpression-2.js")]
    public Task parenthesized_equalityexpression_abstract_not_equal_relationalexpression_2()
        => CompilationFailureTest("parenthesized-equalityexpression-abstract-not-equal-relationalexpression-2");

    [Fact(DisplayName = "parenthesized-equalityexpression-strict-equal-relationalexpression-0.js")]
    public Task parenthesized_equalityexpression_strict_equal_relationalexpression_0()
        => CompilationFailureTest("parenthesized-equalityexpression-strict-equal-relationalexpression-0");

    [Fact(DisplayName = "parenthesized-equalityexpression-strict-equal-relationalexpression-1.js")]
    public Task parenthesized_equalityexpression_strict_equal_relationalexpression_1()
        => CompilationFailureTest("parenthesized-equalityexpression-strict-equal-relationalexpression-1");

    [Fact(DisplayName = "parenthesized-equalityexpression-strict-equal-relationalexpression-2.js")]
    public Task parenthesized_equalityexpression_strict_equal_relationalexpression_2()
        => CompilationFailureTest("parenthesized-equalityexpression-strict-equal-relationalexpression-2");

    [Fact(DisplayName = "parenthesized-equalityexpression-strict-not-equal-relationalexpression-0.js")]
    public Task parenthesized_equalityexpression_strict_not_equal_relationalexpression_0()
        => CompilationFailureTest("parenthesized-equalityexpression-strict-not-equal-relationalexpression-0");

    [Fact(DisplayName = "parenthesized-equalityexpression-strict-not-equal-relationalexpression-1.js")]
    public Task parenthesized_equalityexpression_strict_not_equal_relationalexpression_1()
        => CompilationFailureTest("parenthesized-equalityexpression-strict-not-equal-relationalexpression-1");

    [Fact(DisplayName = "parenthesized-equalityexpression-strict-not-equal-relationalexpression-2.js")]
    public Task parenthesized_equalityexpression_strict_not_equal_relationalexpression_2()
        => CompilationFailureTest("parenthesized-equalityexpression-strict-not-equal-relationalexpression-2");

    [Fact(DisplayName = "parenthesized-expression-comma-assignmentexpression-0.js")]
    public Task parenthesized_expression_comma_assignmentexpression_0()
        => CompilationFailureTest("parenthesized-expression-comma-assignmentexpression-0");

    [Fact(DisplayName = "parenthesized-expression-comma-assignmentexpression-1.js")]
    public Task parenthesized_expression_comma_assignmentexpression_1()
        => CompilationFailureTest("parenthesized-expression-comma-assignmentexpression-1");

    [Fact(DisplayName = "parenthesized-expression-comma-assignmentexpression-2.js")]
    public Task parenthesized_expression_comma_assignmentexpression_2()
        => CompilationFailureTest("parenthesized-expression-comma-assignmentexpression-2");

    [Fact(DisplayName = "parenthesized-identifierreference-arguments-strict.js")]
    public Task parenthesized_identifierreference_arguments_strict()
        => CompilationFailureTest("parenthesized-identifierreference-arguments-strict");

    [Fact(DisplayName = "parenthesized-identifierreference-eval-strict.js")]
    public Task parenthesized_identifierreference_eval_strict()
        => CompilationFailureTest("parenthesized-identifierreference-eval-strict");

    [Fact(DisplayName = "parenthesized-import.meta.js")]
    public Task parenthesized_import_meta()
        => CompilationFailureTest("parenthesized-import.meta");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-assignment-assignmentexpression-0.js")]
    public Task parenthesized_lefthandsideexpression_assignment_assignmentexpression_0()
        => CompilationFailureTest("parenthesized-lefthandsideexpression-assignment-assignmentexpression-0");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-assignment-assignmentexpression-1.js")]
    public Task parenthesized_lefthandsideexpression_assignment_assignmentexpression_1()
        => CompilationFailureTest("parenthesized-lefthandsideexpression-assignment-assignmentexpression-1");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-assignment-assignmentexpression-2.js")]
    public Task parenthesized_lefthandsideexpression_assignment_assignmentexpression_2()
        => CompilationFailureTest("parenthesized-lefthandsideexpression-assignment-assignmentexpression-2");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-coalesce-assignment-assignmentexpression-0.js")]
    public Task parenthesized_lefthandsideexpression_coalesce_assignment_assignmentexpression_0()
        => CompilationFailureTest("parenthesized-lefthandsideexpression-coalesce-assignment-assignmentexpression-0");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-coalesce-assignment-assignmentexpression-1.js")]
    public Task parenthesized_lefthandsideexpression_coalesce_assignment_assignmentexpression_1()
        => CompilationFailureTest("parenthesized-lefthandsideexpression-coalesce-assignment-assignmentexpression-1");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-coalesce-assignment-assignmentexpression-2.js")]
    public Task parenthesized_lefthandsideexpression_coalesce_assignment_assignmentexpression_2()
        => CompilationFailureTest("parenthesized-lefthandsideexpression-coalesce-assignment-assignmentexpression-2");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-logical-and-assignment-assignmentexpression-0.js")]
    public Task parenthesized_lefthandsideexpression_logical_and_assignment_assignmentexpression_0()
        => CompilationFailureTest("parenthesized-lefthandsideexpression-logical-and-assignment-assignmentexpression-0");

    [Fact(DisplayName = "parenthesized-lefthandsideexpression-logical-and-assignment-assignmentexpression-1.js")]
    public Task parenthesized_lefthandsideexpression_logical_and_assignment_assignmentexpression_1()
        => CompilationFailureTest("parenthesized-lefthandsideexpression-logical-and-assignment-assignmentexpression-1");

}
