"use strict";

// A numeric coercion of a variable (e.g. `b - a`) must not change what the variable
// holds for later reads in non-numeric contexts within the same basic block.
function subtractThenAdd(a, b) {
    var d = b - a;
    return [d, a + b, typeof a];
}

function localsFromObject(o) {
    var a = o.a, b = o.b;
    var d = b - a;
    return [d, a + b, a < b, typeof a, typeof b];
}

var valueOfCalls = 0;
function coerceTwice(o) {
    var v = o.v;
    var d = v - 1;
    var e = v * 2;
    return [d, e, valueOfCalls];
}

console.log(JSON.stringify(subtractThenAdd("1", "2")));
console.log(JSON.stringify(subtractThenAdd(1, 2)));
console.log(JSON.stringify(localsFromObject({ a: "10", b: "9" })));
console.log(JSON.stringify(localsFromObject({ a: 10, b: 9 })));
console.log(JSON.stringify(coerceTwice({ v: { valueOf() { valueOfCalls++; return 5; } } })));
