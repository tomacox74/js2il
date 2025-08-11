# ECMAScript 2025 Feature Coverage

[ECMAScript® 2025 Language Specification](https://tc39.es/ecma262/)

This file is auto-generated from ECMAScript2025_FeatureCoverage.json.

## [ECMAScript Function Objects](https://tc39.es/ecma262/#sec-ecmascript-function-objects)

### [Function Objects](https://tc39.es/ecma262/#sec-ecmascript-function-objects)

#### [Function closures](https://tc39.es/ecma262/#sec-ecmascript-function-objects)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Function closures | Supported | `Js2IL.Tests/JavaScript/Function_NestedFunctionLogsOuterParameter.js`<br>`Js2IL.Tests/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js` |  | 8.1.2 |
| Global function calls global function | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionCallsGlobalFunction.js` |  | 8.1.2 |
| Global function with parameter | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionWithParameter.js` |  | 8.1.2 |
| Global function with array iteration | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionWithArrayIteration.js` |  | 8.1.2 |
| Global function returns nested function, logs param and global | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js` |  | 8.1.2 |
| Global function logs global variable | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionLogsGlobalVariable.js` |  | 8.1.2 |
| Global function declares and calls nested function | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionDeclaresAndCallsNestedFunction.js` |  | 8.1.2 |
| Global function changes global variable value | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionChangesGlobalVariableValue.js` |  | 8.1.2 |
| Returns static value and logs | Supported | `Js2IL.Tests/JavaScript/Function_ReturnsStaticValueAndLogs.js` |  | 8.1.2 |
| Hello World function | Supported | `Js2IL.Tests/JavaScript/Function_HelloWorld.js` |  | 8.1.2 |


### [Unary Operators](https://tc39.es/ecma262/#sec-ecmascript-language-expressions)

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
| Binary + (Addition) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_AddNumberNumber.js`<br>`Js2IL.Tests/JavaScript/BinaryOperator_AddStringNumber.js`<br>`Js2IL.Tests/JavaScript/BinaryOperator_AddStringString.js` |  | 13.5.1 |


#### [Subtraction operator (-)](https://tc39.es/ecma262/#sec-subtraction-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary - (Subtraction) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_SubNumberNumber.js` |  | 13.5.2 |


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
| Binary == (Equality) | Supported | `Js2IL.Tests/JavaScript/BinaryOperator_Equal.js` |  | 13.5.9 |


### [Control Flow Statements](https://tc39.es/ecma262/#sec-ecmascript-language-statements-and-declarations)

#### [if statement](https://tc39.es/ecma262/#sec-if-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| if statement (LessThan) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_If_LessThan.js` |  | 13.6.7 |


#### [for statement](https://tc39.es/ecma262/#sec-for-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| for loop (CountToFive) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_CountToFive.js` |  | 14.7.4 |
| for loop (CountDownFromFive) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_CountDownFromFive.js` |  | 14.7.4 |
| for loop (LessThanOrEqual) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_LessThanOrEqual.js` |  | 14.7.4 |
| for loop (GreaterThanOrEqual) | Supported | `Js2IL.Tests/JavaScript/ControlFlow_ForLoop_GreaterThanOrEqual.js` |  | 14.7.4 |


## [Statements and Declarations](https://tc39.es/ecma262/#sec-ecmascript-language-statements-and-declarations)

### [Declarations](https://tc39.es/ecma262/#sec-declarations)

#### [let/const](https://tc39.es/ecma262/#sec-let-and-const-declarations)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| let/const | Not Supported |  |  | 13.2.1 |


#### [var](https://tc39.es/ecma262/#sec-variable-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| var | Supported | `Js2IL.Tests/JavaScript/Function_GlobalFunctionChangesGlobalVariableValue.js`<br>`Js2IL.Tests/JavaScript/Function_GlobalFunctionDeclaresAndCallsNestedFunction.js`<br>`Js2IL.Tests/JavaScript/Function_GlobalFunctionLogsGlobalVariable.js` |  | 13.2.1 |


#### [Function declarations](https://tc39.es/ecma262/#sec-function-definitions)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Function declarations | Supported | `Js2IL.Tests/JavaScript/Function_HelloWorld.js` |  | 13.2.3 |
| Arrow functions | Not Supported |  |  | 13.2.3 |


#### [Default parameters, Rest parameters](https://tc39.es/ecma262/#sec-function-definitions-runtime-semantics-evaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Default parameters | Not Supported |  |  | 13.2.3.1 |
| Rest parameters | Not Supported |  |  | 13.2.3.1 |


#### [Spread syntax](https://tc39.es/ecma262/#sec-argument-lists-runtime-semantics-argumentlistevaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Spread syntax | Not Supported |  |  | 13.2.5 |


## [Statements and Declarations](https://tc39.es/ecma262/#sec-ecmascript-language-statements-and-declarations)

### [Array Initializer (ArrayLiteral)](https://tc39.es/ecma262/#sec-array-initializer)

#### [ArrayLiteral](https://tc39.es/ecma262/#sec-array-initializer)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array literal basic construction | Supported | `Js2IL.Tests/JavaScript/ArrayLiteral.js` | Covers creation, element access, and length property. See also verified output in GeneratorTests.ArrayLiteral.verified.txt. | 13.2.4.1 |


### [Object Initializer (ObjectLiteral)](https://tc39.es/ecma262/#sec-object-initializer)

#### [ObjectLiteral](https://tc39.es/ecma262/#sec-object-initializer)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Object literal basic construction | Supported | `Js2IL.Tests/JavaScript/ObjectLiteral.js` | Covers creation and property access. See also verified output in GeneratorTests.ObjectLiteral.verified.txt. | 13.2.5.1 |

