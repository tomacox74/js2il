"use strict";

// .call()/.apply() with generic array-like receivers must keep working through
// the explicit-receiver adapter ABI (issue #1895).
var arrayLike = { 0: "a", 1: "b", 2: "c", length: 3 };
console.log(Array.prototype.join.call([1, 2, 3], "-"));
console.log(Array.prototype.map.call(arrayLike, function (x) { return x.toUpperCase(); }));
console.log(Array.prototype.filter.apply(arrayLike, [function (x) { return x !== "b"; }]));
console.log(Array.prototype.every.call([1, 2, 3], function (x) { return x > 0; }));
console.log(Array.prototype.some.call([1, 2, 3], function (x) { return x > 2; }));
console.log(Array.prototype.at.call(arrayLike, -1));
console.log(Array.prototype.toString.call([1, 2, 3]));

// Fixed-arity zero-argument calls (via .call, so dispatch goes through the
// Array.prototype adapter rather than the compiler's direct instance-method
// dispatch): a missing argument and an explicit `undefined` argument must be
// treated identically.
console.log(Array.prototype.join.call([1, 2]));
console.log(Array.prototype.join.call([1, 2], undefined));
console.log(Array.prototype.at.call([1, 2, 3]));
console.log(Array.prototype.at.call([1, 2, 3], undefined));
console.log(Array.prototype.indexOf.call([undefined, 2]));
console.log(Array.prototype.indexOf.call([undefined, 2], undefined));

// Symbol.iterator must remain the exact same function object as "values".
console.log(Array.prototype.values === Array.prototype[Symbol.iterator]);

// Variadic methods must still support their full item/argument lists.
var pushed = [1, 2];
console.log(pushed.push(3, 4, 5));
console.log(pushed.join(","));
console.log(Array.prototype.reduce.call([1, 2, 3], function (acc, x) { return acc + x; }));
console.log(Array.prototype.reduce.call([1, 2, 3], function (acc, x) { return acc + x; }, 10));

// Missing receiver still throws the expected errors.
try {
    Array.prototype.reduce.call(null, function () {});
} catch (error) {
    console.log(error instanceof TypeError);
}

try {
    Array.prototype.every.call(undefined, function () {});
} catch (error) {
    console.log(error instanceof TypeError);
}

// Overriding Array.prototype methods must still be observable through the
// property descriptor (note: calling `arr.join()` directly on a
// statically-typed Array receiver uses the compiler's separate direct
// instance-method dispatch fast path and does not consult this property;
// that optimization is a pre-existing, out-of-scope behavior).
var originalJoin = Array.prototype.join;
Array.prototype.join = function () {
    return "overridden";
};
console.log(Array.prototype.join.call([1, 2, 3]));
Array.prototype.join = originalJoin;
