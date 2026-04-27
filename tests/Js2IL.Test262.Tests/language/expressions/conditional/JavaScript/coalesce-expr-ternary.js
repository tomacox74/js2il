// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: >
    ShortCircuitExpression in the Conditional Expression (? :)
esid: sec-conditional-operator
info: |
    ShortCircuitExpression :
        LogicalORExpression
        CoalesceExpression

    CoalesceExpression :
        CoalesceExpressionHead ?? BitwiseORExpression

    CoalesceExpressionHead :
        CoalesceExpression
        BitwiseORExpression

    ConditionalExpression :
        ShortCircuitExpression
        ShortCircuitExpression ? AssignmentExpression : AssignmentExpression
features: [coalesce-expression]
---*/

var x;

x = undefined ?? true ? 0 : 42;
console.log(Object.is(x, 0));

x = undefined;
x = null ?? true ? 0 : 42;
console.log(Object.is(x, 0));

x = undefined;
x = undefined ?? false ? 0 : 42;
console.log(Object.is(x, 42));

x = undefined;
x = null ?? false ? 0 : 42;
console.log(Object.is(x, 42));

x = undefined;
x = false ?? true ? 0 : 42;
console.log(Object.is(x, 42));

x = undefined;
x = 0 ?? true ? 0 : 42;
console.log(Object.is(x, 42));

x = undefined;
x = 1 ?? false ? 0 : 42;
console.log(Object.is(x, 0));

x = undefined;
x = true ?? false ? 0 : 42;
console.log(Object.is(x, 0));

x = undefined;
x = true ?? true ? 0 : 42;
console.log(Object.is(x, 0));

x = undefined;
x = '' ?? true ? 0 : 42;
console.log(Object.is(x, 42));

x = undefined;
x = Symbol() ?? false ? 0 : 42;
console.log(Object.is(x, 0));

x = undefined;
x = {} ?? false ? 0 : 42;
console.log(Object.is(x, 0));
