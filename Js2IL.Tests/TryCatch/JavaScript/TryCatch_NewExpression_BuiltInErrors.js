"use strict";\r\n\r\n// PL3.3a: NewExpression for built-in Error types
try {
    throw new TypeError(123);
} catch (e) {
    console.log(e);
}

console.log(new Error("boom"));
console.log(new EvalError("eval"));
console.log(new RangeError("range"));
console.log(new ReferenceError("ref"));
console.log(new SyntaxError("syntax"));
console.log(new URIError("uri"));
console.log(new AggregateError("agg"));
