# ECMAScript 2025 Feature Coverage

[ECMAScript® 2025 Language Specification](https://tc39.es/ecma262/)

This file is auto-generated from ECMAScript2025_FeatureCoverage.json.

## [ECMAScript Language: Expressions](https://tc39.es/ecma262/#sec-ecmascript-language-expressions)

### [Primary Expressions](https://tc39.es/ecma262/#sec-primary-expression)

#### [NumericLiteral](https://tc39.es/ecma262/#sec-numeric-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Numeric literals (integer and decimal) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_AddNumberNumber.js`<br>`Js2IL.Tests/JavaScript/BinaryOperator_MulNumberNumber.js` | Numbers are represented as double and used pervasively across arithmetic, comparison, and control-flow tests. | 13.1.1 |


#### [StringLiteral](https://tc39.es/ecma262/#sec-string-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String literals (single/double quotes; escapes) | Supported | `Js2IL.Tests/JavaScript/String_StartsWith_Basic.js`<br>`Js2IL.Tests/JavaScript/String_Replace_Regex_Global.js` | Backed by .NET System.String; values are boxed/unboxed where needed in member calls and concatenation. | 13.1.2 |


#### [BooleanLiteral](https://tc39.es/ecma262/#sec-boolean-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Boolean literals (true/false) | Supported | `Js2IL.Tests/JavaScript/UnaryOperator_Typeof.js`<br>`Js2IL.Tests/JavaScript/JSON_Parse_SimpleObject.js` | Emits proper IL for true/false and boxes when needed in arrays/log calls. See generator snapshot: Js2IL.Tests/Literals/GeneratorTests.BooleanLiteral.verified.txt. | 13.1.3 |


#### [Template Literals](https://tc39.es/ecma262/#sec-template-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Template literals (basic interpolation) | Supported | `Js2IL.Tests/JavaScript/String_TemplateLiteral_Basic.js` | Concatenates quasis and expressions via runtime Operators.Add with JS string/number coercion. Tagged templates are not yet supported. | 13.1.4 |


#### [NullLiteral](https://tc39.es/ecma262/#sec-null-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| null literal | Supported | `Js2IL.Tests/JavaScript/Literals_NullAndUndefined.js`<br>`Js2IL.Tests/JavaScript/JSON_Parse_SimpleObject.js` | null emission validated in literals and variable tests; see execution snapshot Js2IL.Tests/Literals/ExecutionTests.Literals_NullAndUndefined.verified.txt. | 13.1.5 |


#### [undefined (Identifier)](https://tc39.es/ecma262/#sec-undefined)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| undefined identifier usage | Supported | `Js2IL.Tests/JavaScript/Literals_NullAndUndefined.js` | Handled as the ECMAScript undefined value and participates in JS truthiness; see execution snapshot Js2IL.Tests/Literals/ExecutionTests.Literals_NullAndUndefined.verified.txt. | 13.1.6 |


### [Declarations](https://tc39.es/ecma262/#sec-declarations)

#### [let/const](https://tc39.es/ecma262/#sec-let-and-const-declarations)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| let/const | Partially Supported | `Js2IL.Tests/JavaScript/Variable_LetBlockScope.js`<br>`Js2IL.Tests/JavaScript/Variable_LetShadowing.js`<br>`Js2IL.Tests/JavaScript/Variable_LetNestedShadowingChain.js`<br>`Js2IL.Tests/JavaScript/Variable_LetFunctionNestedShadowing.js`<br>`Js2IL.Tests/JavaScript/Variable_ConstSimple.js` | Block scoping, shadowing chain, nested function capture, and simple const initialization implemented. Temporal dead zone access error (Variable_TemporalDeadZoneAccess.js) and reads before initialization are still pending. | 13.2.1 |
| Const reassignment throws TypeError | Supported | `Js2IL.Tests/JavaScript/Variable_ConstReassignmentError.js` | Assignment to a const Identifier and ++/-- on const emit a runtime TypeError; error is catchable via try/catch. | 13.2.1 |


#### [var](https://tc39.es/ecma262/#sec-variable-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| var | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionChangesGlobalVariableValue.js`<br>`Js2IL.Tests/JavaScript/Function_GlobalFunctionDeclaresAndCallsNestedFunction.js`<br>`Js2IL.Tests/JavaScript/Function_GlobalFunctionLogsGlobalVariable.js` |  | 13.2.1 |


#### [Binding patterns (destructuring)](https://tc39.es/ecma262/#sec-destructuring-binding-patterns)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Object destructuring in variable declarations (const/let) — basic | Partially Supported | `Js2IL.Tests/Node/JavaScript/PerfHooks_PerformanceNow_Basic.js` | Supports simple object patterns in variable declarations, e.g., const { performance } = require('perf_hooks'); The initializer is evaluated once and properties are bound via direct typed getters when available (for known intrinsics) or via JavaScriptRuntime.Object.GetProperty. Not yet supported: nested patterns, default values, renaming (alias), rest properties, array destructuring, and parameter/assignment destructuring. | 13.2.2 |


#### [Function declarations](https://tc39.es/ecma262/#sec-function-definitions)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Function declarations | Supported | `Js2IL.Tests/JavaScript/Function_HelloWorld.js` |  | 13.2.3 |
| Arrow functions | Partially Supported | `Js2IL.Tests/JavaScript/ArrowFunction_SimpleExpression.js`<br>`Js2IL.Tests/JavaScript/ArrowFunction_BlockBody_Return.js`<br>`Js2IL.Tests/JavaScript/ArrowFunction_GlobalFunctionCallsGlobalFunction.js`<br>`Js2IL.Tests/JavaScript/ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js`<br>`Js2IL.Tests/JavaScript/ArrowFunction_GlobalFunctionWithMultipleParameters.js`<br>`Js2IL.Tests/JavaScript/ArrowFunction_NestedFunctionAccessesMultipleScopes.js`<br>`Js2IL.Tests/JavaScript/ArrowFunction_CapturesOuterVariable.js` | Covers expression- and block-bodied arrows, multiple parameters, nested functions, and closure capture across scopes (including returning functions that capture globals/locals). Not yet supported: default/rest parameters, parameter destructuring, lexical this/arguments semantics, and spread at call sites. | 13.2.3 |


#### [Default parameters, Rest parameters](https://tc39.es/ecma262/#sec-function-definitions-runtime-semantics-evaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Default parameters | Not Supported |  |  | 13.2.3.1 |
| Rest parameters | Not Supported |  |  | 13.2.3.1 |


#### [Spread syntax](https://tc39.es/ecma262/#sec-argument-lists-runtime-semantics-argumentlistevaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Spread syntax | Not Supported |  |  | 13.2.5 |


### [Array Initializer (ArrayLiteral)](https://tc39.es/ecma262/#sec-array-initializer)

#### [ArrayLiteral](https://tc39.es/ecma262/#sec-array-initializer)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array literal basic construction | Supported | `Js2IL.Tests/JavaScript/ArrayLiteral.js` | Covers creation, element access, and length property. See also verified output in Literals/GeneratorTests.ArrayLiteral.verified.txt. | 13.2.4.1 |
| Array literal spread (copy elements) | Supported | `Js2IL.Tests/JavaScript/Array_Spread_Copy.js` | Spread elements in array literals are emitted via JavaScriptRuntime.Array.PushRange; supports copying from another array. | 13.2.4.1 |


### [Object Initializer (ObjectLiteral)](https://tc39.es/ecma262/#sec-object-initializer)

#### [ObjectLiteral](https://tc39.es/ecma262/#sec-object-initializer)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Object literal basic construction | Supported | `Js2IL.Tests/JavaScript/ObjectLiteral.js` | Covers creation and property access. See also verified output in Literals/GeneratorTests.ObjectLiteral.verified.txt. | 13.2.5.1 |


### [Unary Operators](https://tc39.es/ecma262/#sec-ecmascript-language-expressions)

#### [typeof operator](https://tc39.es/ecma262/#sec-typeof-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| typeof | Supported | `Js2IL.Tests/JavaScript/UnaryOperator_Typeof.js` | Implemented via JavaScriptRuntime.TypeUtilities::Typeof and IL emission for UnaryExpression(typeof). typeof null returns 'object'; functions report 'function'; objects report 'object'. | 13.4.3 |


#### [Logical not operator (!)](https://tc39.es/ecma262/#sec-logical-not-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary ! (Logical not) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_If_NotFlag.js` | Branch inversion supported in conditionals (e.g., if (!x) ...), and value negation in non-branching contexts. | 13.4.7 |


#### [Postfix increment operator (++)](https://tc39.es/ecma262/#sec-postfix-increment-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary ++ (Postfix increment) | Supported | `Js2IL.Tests/JavaScript/UnaryOperator_PlusPlusPostfix.js` |  | 13.4.9 |


#### [Postfix decrement operator (--)](https://tc39.es/ecma262/#sec-postfix-decrement-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary -- (Postfix decrement) | Supported | `Js2IL.Tests/JavaScript/UnaryOperator_MinusMinusPostfix.js` |  | 13.4.10 |


### [Binary Operators](https://tc39.es/ecma262/#sec-ecmascript-language-expressions)

#### [Addition operator (+)](https://tc39.es/ecma262/#sec-additive-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary + (Addition) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_AddNumberNumber.js`<br>`Js2IL.Tests/JavaScript/BinaryOperator_AddStringNumber.js`<br>`Js2IL.Tests/JavaScript/BinaryOperator_AddStringString.js`<br>`Js2IL.Tests/JavaScript/Classes_ClassConstructor_TwoParams_AddMethod.js` | Fast-path string concat; general '+' follows JS coercion via runtime helper. | 13.5.1 |


#### [Subtraction operator (-)](https://tc39.es/ecma262/#sec-subtraction-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary - (Subtraction) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_SubNumberNumber.js`<br>`Js2IL.Tests/JavaScript/Classes_ClassConstructor_TwoParams_SubtractMethod.js` | Numeric subtraction; matches JS semantics for non-numeric via coercion helpers where applicable. | 13.5.2 |


#### [Multiplicative operators (*, /, %)](https://tc39.es/ecma262/#sec-multiplicative-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary * (Multiplication) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_MulNumberNumber.js` |  | 13.5.3 |
| Binary / (Division) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_DivNumberNumber.js` |  | 13.5.3 |
| Binary % (Remainder) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_ModNumberNumber.js` |  | 13.5.3 |


#### [Exponentiation operator (**) ](https://tc39.es/ecma262/#sec-exp-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary ** (Exponentiation) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_ExpNumberNumber.js` |  | 13.5.4 |


#### [Bitwise operators (&, |, ^)](https://tc39.es/ecma262/#sec-binary-bitwise-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary & (Bitwise AND) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_BitwiseAndNumberNumber.js` |  | 13.5.6 |
| Binary \| (Bitwise OR) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_BitwiseOrNumberNumber.js` |  | 13.5.6 |
| Binary ^ (Bitwise XOR) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_BitwiseXorNumberNumber.js` |  | 13.5.6 |


#### [Shift operators (<<, >>, >>>)](https://tc39.es/ecma262/#sec-left-shift-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary << (Left shift) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_LeftShiftNumberNumber.js` |  | 13.5.7 |
| Binary >> (Signed right shift) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_RightShiftNumberNumber.js` |  | 13.5.7 |
| Binary >>> (Unsigned right shift) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_UnsignedRightShiftNumberNumber.js` |  | 13.5.7 |


#### [Relational operators (<, <=, >, >=)](https://tc39.es/ecma262/#sec-relational-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary < (Less than) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_LessThan.js` |  | 13.5.8 |
| Binary <= (Less than or equal) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_LessThanOrEqual.js` |  | 13.5.8 |
| Binary > (Greater than) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_GreaterThan.js` |  | 13.5.8 |
| Binary >= (Greater than or equal) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_GreaterThanOrEqual.js` |  | 13.5.8 |


#### [Equality operators (==)](https://tc39.es/ecma262/#sec-equality-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary == (Equality) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_Equal.js`<br>`Js2IL.Tests/JavaScript/Function_IsEven_CompareResultToTrue.js` | Covers numeric and boolean equality, including comparisons against literals and function-returned booleans with selective boxing/unboxing. See also generator snapshot: Js2IL.Tests/BinaryOperator/GeneratorTests.BinaryOperator_EqualBoolean.verified.txt. | 13.5.9 |


#### [Logical operators (||, &&)](https://tc39.es/ecma262/#sec-binary-logical-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary \|\| (Logical OR) with short-circuit | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_LogicalOr_Value.js` | Value form returns left if truthy, otherwise right; branching form uses JS ToBoolean for conditions. Right-hand side is not evaluated when short-circuited. | 13.5.10 |
| Binary && (Logical AND) with short-circuit | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_LogicalAnd_Value.js` | Value form returns left if falsy, otherwise right; branching form uses JS ToBoolean for conditions. Right-hand side is not evaluated when short-circuited. | 13.5.10 |


#### [Conditional (ternary) operator (?:)](https://tc39.es/ecma262/#sec-conditional-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Conditional operator (?:) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_Conditional_Ternary.js` | Expression-level branching with both arms coerced to object where needed. Verified via generator and execution tests in ControlFlow subgroup. | 13.5.16 |


### [Assignment Operators](https://tc39.es/ecma262/#sec-assignment-operators)

#### [Assignment Operators (+=, -=, ...)](https://tc39.es/ecma262/#sec-assignment-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Compound assignment += with strings | Supported | `Js2IL.Tests/JavaScript/String_PlusEquals_Append.js` | += on identifiers uses JavaScriptRuntime.Operators.Add for JS coercion and stores back to the same binding; validated by generator snapshot. | 13.15.2 |


## [ECMAScript Language: Statements and Declarations](https://tc39.es/ecma262/#sec-ecmascript-language-statements-and-declarations)

### [Return Statement](https://tc39.es/ecma262/#sec-return-statement)

#### [Runtime Semantics: ReturnStatement Evaluation](https://tc39.es/ecma262/#sec-return-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| return statement (basic) | Supported | `Js2IL.Tests/JavaScript/Function_ReturnsStaticValueAndLogs.js` | Function returns propagate values (boxed) to callers; validated by execution snapshot showing returned value. | 14.1.1 |


### [The if Statement](https://tc39.es/ecma262/#sec-if-statement)

#### [Runtime Semantics: Evaluation](https://tc39.es/ecma262/#sec-if-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| if statement (LessThan) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_If_LessThan.js` |  | 14.6.2 |
| if statement (!flag) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_If_NotFlag.js` | Logical not in conditional test supported. | 14.6.2 |
| if statement (result == true) | Supported | `Js2IL.Tests/JavaScript/Function_IsEven_CompareResultToTrue.js` | Compares function-returned boolean to true and branches accordingly. | 14.6.2 |
| if condition truthiness (non-boolean) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_If_Truthiness.js` | Conditions like if (url) are coerced via JS ToBoolean semantics (empty string/0/NaN/undefined/null => false; others => true). | 14.6.2 |


### [The do-while Statement](https://tc39.es/ecma262/#sec-do-while-statement)

#### [Runtime Semantics: DoWhileStatement Evaluation](https://tc39.es/ecma262/#sec-do-while-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| do-while loop (CountDownFromFive) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_DoWhile_CountDownFromFive.js` |  | 14.7.1.1 |
| do-while loop: continue (skip even) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_DoWhile_Continue_SkipEven.js` | continue branches to the post-body test point (LoopContext). | 14.7.1.1 |
| do-while loop: break | Supported | `Js2IL.Tests/JavaScript/ControlFlow_DoWhile_Break_AtThree.js` | break branches to loop end (LoopContext). | 14.7.1.1 |


### [The while Statement](https://tc39.es/ecma262/#sec-while-statement)

#### [Runtime Semantics: WhileStatement Evaluation](https://tc39.es/ecma262/#sec-while-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| while loop (CountDownFromFive) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_While_CountDownFromFive.js` |  | 14.7.2.1 |
| while loop: continue (skip even) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_While_Continue_SkipEven.js` | continue branches to loop head (LoopContext). | 14.7.2.1 |
| while loop: break | Supported | `Js2IL.Tests/JavaScript/ControlFlow_While_Break_AtThree.js` | break branches to loop end (LoopContext). | 14.7.2.1 |


### [The for Statement](https://tc39.es/ecma262/#sec-for-statement)

#### [Runtime Semantics: ForLoopEvaluation](https://tc39.es/ecma262/#sec-runtime-semantics-forloopevaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| for loop (CountToFive) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_CountToFive.js` |  | 14.7.4.2 |
| for loop (CountDownFromFive) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_CountDownFromFive.js` |  | 14.7.4.2 |
| for loop (LessThanOrEqual) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_LessThanOrEqual.js` |  | 14.7.4.2 |
| for loop (GreaterThanOrEqual) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_GreaterThanOrEqual.js` |  | 14.7.4.2 |
| for loop: continue | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_Continue_SkipEven.js` | Implements continue by branching to the update expression (LoopContext). | 14.7.4.2 |
| for loop: break | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_Break_AtThree.js` | Implements break by branching to loop end label (LoopContext). | 14.7.4.2 |


### [The for-of Statement](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements)

#### [Runtime Semantics: ForInOfBodyEvaluation (for-of)](https://tc39.es/ecma262/#sec-runtime-semantics-forinofbodyevaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| for-of over arrays (enumerate values) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForOf_Array_Basic.js` | Emitted by indexing iterable via JavaScriptRuntime.Object.GetLength(object) and GetItem(object, double); uses object locals for iterator state. | 14.7.5.1 |


### [The try Statement](https://tc39.es/ecma262/#sec-try-statement)

#### [Runtime Semantics: TryStatement Evaluation](https://tc39.es/ecma262/#sec-try-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| throw statement | Supported | `Js2IL.Tests/JavaScript/TryCatch_NoBinding.js` | Emits throw of JavaScriptRuntime.Error; used in try/catch tests. | 14.16.1 |
| try/catch (no binding) | Supported | `Js2IL.Tests/JavaScript/TryCatch_NoBinding.js`<br>`Js2IL.Tests/JavaScript/TryCatch_NoBinding_NoThrow.js` | Catch blocks currently handle only JavaScriptRuntime.Error thrown within the try; exceptions thrown later (after returning a closure) are not caught by the earlier catch. | 14.16.1 |
| try/finally (no catch) | Partially Supported | `Js2IL.Tests/JavaScript/TryFinally_NoCatch.js`<br>`Js2IL.Tests/JavaScript/TryFinally_NoCatch_Throw.js` | Finally emission is in place. Execution test for throw is skipped pending unhandled Error semantics at top-level; generator snapshot verifies structure. | 14.16.1 |


## [ECMAScript Language: Classes](https://tc39.es/ecma262/#sec-ecmascript-language-classes)

### [Class Definitions](https://tc39.es/ecma262/#sec-class-definitions)

#### [Basic class features](https://tc39.es/ecma262/#sec-class-definitions)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Class declaration (empty) | Supported | `Js2IL.Tests/JavaScript/Classes_DeclareEmptyClass.js` |  | 15.1.1 |
| Instance method (declare and call) | Supported | `Js2IL.Tests/JavaScript/Classes_ClassWithMethod_HelloWorld.js` |  | 15.1.1 |
| Static method (declare and call) | Supported | `Js2IL.Tests/JavaScript/Classes_ClassWithStaticMethod_HelloWorld.js` |  | 15.1.1 |
| Instance field initializer (public property default) | Supported | `Js2IL.Tests/JavaScript/Classes_ClassProperty_DefaultAndLog.js` | Emitted by assigning defaults in the generated .ctor. | 15.1.1 |
| Static field initializer (static property default) | Supported | `Js2IL.Tests/JavaScript/Classes_ClassWithStaticProperty_DefaultAndLog.js` | Emitted as a static field initialized in a synthesized .cctor; accessed via ldsfld. | 15.1.1 |
| Constructor with parameter and this.field assignment; method reads field | Supported | `Js2IL.Tests/JavaScript/Classes_ClassConstructor_Param_Field_Log.js` |  | 15.1.1 |
| Constructor with multiple parameters; method uses fields | Supported | `Js2IL.Tests/JavaScript/Classes_ClassConstructor_WithMultipleParameters.js`<br>`Js2IL.Tests/JavaScript/Classes_ClassConstructor_TwoParams_AddMethod.js`<br>`Js2IL.Tests/JavaScript/Classes_ClassConstructor_TwoParams_SubtractMethod.js` | Covers multi-parameter constructors and arithmetic in instance methods. | 15.1.1 |
| Private instance field (#) with helper method access | Supported | `Js2IL.Tests/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js` | Generated as a private .NET field with a mangled name; accessible only within the class. | 15.1.1 |


## [The Math Object](https://tc39.es/ecma262/#sec-math-object)

### [Value Properties of the Math Object](https://tc39.es/ecma262/#sec-value-properties-of-the-math-object)

### [Function Properties of the Math Object](https://tc39.es/ecma262/#sec-function-properties-of-the-math-object)

#### [Math.ceil](https://tc39.es/ecma262/#sec-math.ceil)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.ceil(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js` | Implements ceiling for numbers represented as double; arguments coerced via minimal ToNumber semantics. Returns NaN for NaN/undefined or negative zero preserved via .NET semantics. | 20.2.2.9 |


#### [Math.sqrt](https://tc39.es/ecma262/#sec-math.sqrt)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.sqrt(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js` | Returns the square root for non-negative inputs; negative or NaN yields NaN; Infinity maps to Infinity. | 20.2.2.24 |


## [Date Objects](https://tc39.es/ecma262/#sec-date-objects)

### [The Date Constructor](https://tc39.es/ecma262/#sec-date-constructor)

#### [Date constructor](https://tc39.es/ecma262/#sec-date-constructor)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| new Date() (current time) | Supported | `Js2IL.Tests/Date/ExecutionTests.cs` | Constructs a Date representing now (UTC). Stores milliseconds since Unix epoch internally. | 21.4.1 |
| new Date(milliseconds) | Supported | `Js2IL.Tests/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js` | Constructs from milliseconds since Unix epoch; numeric argument is coerced per JS ToNumber minimal behavior. | 21.4.1 |


### [Properties of the Date Constructor](https://tc39.es/ecma262/#sec-properties-of-the-date-constructor)

#### [Date.now](https://tc39.es/ecma262/#sec-date.now)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Date.now() | Supported | `Js2IL.Tests/Date/ExecutionTests.cs` | Returns current time in milliseconds since Unix epoch as a number (boxed double). | 21.4.2.1 |


#### [Date.parse](https://tc39.es/ecma262/#sec-date.parse)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Date.parse(string) | Supported | `Js2IL.Tests/Date/ExecutionTests.cs`<br>`Js2IL.Tests/Date/ExecutionTests.Date_Parse_IsoString.verified.txt` | Parses an ISO-like string to milliseconds since Unix epoch, or NaN on failure; returns a number (boxed double). | 21.4.2.2 |


### [Properties of the Date Prototype Object](https://tc39.es/ecma262/#sec-properties-of-the-date-prototype-object)

#### [Date.prototype.getTime](https://tc39.es/ecma262/#sec-date.prototype.gettime)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Date.prototype.getTime | Supported | `Js2IL.Tests/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js`<br>`Js2IL.Tests/Date/ExecutionTests.Date_Construct_FromMs_GetTime_ToISOString.verified.txt` | Returns milliseconds since Unix epoch as a number (boxed double). | 21.4.3.5 |


#### [Date.prototype.toISOString](https://tc39.es/ecma262/#sec-date.prototype.toisostring)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Date.prototype.toISOString | Supported | `Js2IL.Tests/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js`<br>`Js2IL.Tests/Date/ExecutionTests.Date_Construct_FromMs_GetTime_ToISOString.verified.txt` | Returns a UTC ISO 8601 string with millisecond precision and trailing 'Z'. | 21.4.3.27 |


## [Array Objects](https://tc39.es/ecma262/#sec-array-objects)

### [Properties of Array Instances](https://tc39.es/ecma262/#sec-properties-of-array-instances)

#### [length](https://tc39.es/ecma262/#sec-properties-of-array-instances)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.length property (read) | Supported | `Js2IL.Tests/JavaScript/Array_LengthProperty_ReturnsCount.js`<br>`Js2IL.Tests/JavaScript/Array_EmptyLength_IsZero.js` | length getter returns number of elements; emitted via JavaScriptRuntime.Object.GetLength(object). Used by for-of implementation. | 23.1.2.1 |


#### [Array.prototype.join](https://tc39.es/ecma262/#sec-array.prototype.join)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.join | Supported | `Js2IL.Tests/JavaScript/Array_Join_Basic.js` | Elements are stringified via DotNet2JSConversions.ToString and joined with a separator (default ','). Codegen dispatches to JavaScriptRuntime.Array.join(object[]). | 23.1.3.13 |


#### [Array.prototype.pop](https://tc39.es/ecma262/#sec-array.prototype.pop)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.pop | Supported | `Js2IL.Tests/JavaScript/Array_Pop_Basic.js` | Removes and returns the last element; when empty returns undefined (represented as null in this runtime). | 23.1.3.20 |


#### [Array.prototype.push](https://tc39.es/ecma262/#sec-array.prototype.push)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.push | Supported | `Js2IL.Tests/JavaScript/Array_Push_Basic.js` | Appends items to the end of the array and returns the new length (as a JS number). | 23.1.3.22 |


#### [Array.prototype.map](https://tc39.es/ecma262/#sec-array.prototype.map)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.map | Partially Supported | `Js2IL.Tests/JavaScript/Array_Map_Basic.js` | Supports basic value-callback mapping and returns a new array. thisArg and additional callback parameters (index, array) are not yet supported. | 23.1.3.25 |


#### [Array.prototype.sort](https://tc39.es/ecma262/#sec-array.prototype.sort)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.sort (default comparator) | Partially Supported | `Js2IL.Tests/JavaScript/Array_Sort_Basic.js` | Default lexicographic sort implemented in JavaScriptRuntime.Array.sort(); comparator function parameter is not yet supported. Returns the array instance. | 23.1.3.27 |


#### [Array.prototype.slice](https://tc39.es/ecma262/#sec-array.prototype.slice)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.slice | Supported | `Js2IL.Tests/JavaScript/Array_Slice_Basic.js` | Returns a shallow copy; handles negative indices and undefined end per spec. | 23.1.3.28 |


#### [Array.prototype.splice](https://tc39.es/ecma262/#sec-array.prototype.splice)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.splice | Supported | `Js2IL.Tests/JavaScript/Array_Splice_Basic.js`<br>`Js2IL.Tests/JavaScript/Array_Splice_InsertAndDelete.js` | Mutates the array by removing and/or inserting elements; returns an array of removed elements. | 23.1.3.31 |


### [Properties of the Array Constructor](https://tc39.es/ecma262/#sec-properties-of-the-array-constructor)

#### [Array.isArray](https://tc39.es/ecma262/#sec-array.isarray)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.isArray | Supported | `Js2IL.Tests/JavaScript/Array_IsArray_Basic.js` | Returns true for JavaScriptRuntime.Array instances; false otherwise. | 23.1.2 |


## [TypedArray Objects](https://tc39.es/ecma262/#sec-typedarray-objects)

### [Int32Array](https://tc39.es/ecma262/#sec-typedarray-objects)

#### [Constructor and basic semantics](https://tc39.es/ecma262/#sec-typedarray-objects)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Int32Array: new Int32Array(length), length, index get/set (basic) | Supported | `Js2IL.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js`<br>`Js2IL.Tests/TypedArray/ExecutionTests.Int32Array_Construct_Length.verified.txt` | Minimal typed array: exposes numeric length, supports indexing; out-of-bounds reads return 0 and out-of-bounds writes are ignored. | 23.2.1.1 |
| Int32Array from array-like (copy and ToInt32 coercion) | Supported | `Js2IL.Tests/TypedArray/JavaScript/Int32Array_FromArray_CopyAndCoerce.js`<br>`Js2IL.Tests/TypedArray/ExecutionTests.Int32Array_FromArray_CopyAndCoerce.verified.txt` | Copies from array-like and coerces values using ToInt32-style truncation; NaN/Infinity/±0 become 0. | 23.2.1.1 |
| Int32Array.prototype.set(source[, offset]) | Supported | `Js2IL.Tests/TypedArray/JavaScript/Int32Array_Set_FromArray_WithOffset.js`<br>`Js2IL.Tests/TypedArray/ExecutionTests.Int32Array_Set_FromArray_WithOffset.verified.txt` | Copies elements from array-like or another Int32Array with optional offset; elements are coerced via ToInt32; offset < 0 treated as 0; copies stop at destination length. | 23.2.1.1 |


## [Text Processing](https://tc39.es/ecma262/#sec-text-processing)

### [String Objects](https://tc39.es/ecma262/#sec-string-objects)

#### [String.prototype.startsWith](https://tc39.es/ecma262/#sec-string.prototype.startswith)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.startsWith | Supported | `Js2IL.Tests/JavaScript/String_StartsWith_Basic.js` | Reflection-based string dispatch routes CLR string receivers to JavaScriptRuntime.String.StartsWith with optional position argument. Returns a boolean value (boxed). | 24.1.3 |


#### [String.prototype.replace](https://tc39.es/ecma262/#sec-string.prototype.replace)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.replace (regex literal, string replacement) | Partially Supported | `Js2IL.Tests/JavaScript/String_Replace_Regex_Global.js` | Supported when the receiver is String(x), the pattern is a regular expression literal, and the replacement is a string. Global (g) and ignoreCase (i) flags are honored. Function replacement, non-regex patterns, and other flags are not yet implemented. Implemented via host intrinsic JavaScriptRuntime.String.Replace and dynamic resolution in IL generator. | 24.1.3 |


#### [String.prototype.localeCompare](https://tc39.es/ecma262/#sec-string.prototype.localecompare)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.localeCompare (numeric compare) | Supported | `Js2IL.Tests/JavaScript/String_LocaleCompare_Numeric.js` | Returns a number (boxed double) consistent with ECMAScript compare semantics; numeric option supported. | 24.1.4 |


### [JSON Object](https://tc39.es/ecma262/#sec-json-object)

#### [JSON.parse](https://tc39.es/ecma262/#sec-json.parse)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| JSON.parse | Partially Supported | `Js2IL.Tests/JSONRuntimeTests.cs` | Implemented via host intrinsic JavaScriptRuntime.JSON.Parse(string). Maps invalid input to SyntaxError and non-string input to TypeError. Reviver parameter is not supported. Objects become ExpandoObject, arrays use JavaScriptRuntime.Array, numbers use double. | 24.5.1 |

