"use strict";

var set = new Set([1, 2, 3]);
var other = new Set([3, 4]);

console.log(Array.from(set.difference(other)).join(","));
console.log(Array.from(set.intersection(other)).join(","));
console.log(Array.from(set.union(other)).join(","));
console.log(Array.from(set.symmetricDifference(other)).join(","));
console.log(set.isDisjointFrom(new Set([4, 5])));
console.log(set.isDisjointFrom(other));
console.log(new Set([1, 2]).isSubsetOf(new Set([1, 2, 3])));
console.log(set.isSupersetOf(new Set([1, 3])));

// Set-like objects only need size/has/keys, so arrays are rejected.
var setLike = {
    size: 2,
    has: function (value) { return value === 3 || value === 4; },
    keys: function* () { yield 3; yield 4; }
};
console.log(Array.from(set.union(setLike)).join(","));
try {
    set.union([3, 4]);
    console.log("array did not throw");
} catch (e) {
    console.log(e instanceof TypeError);
}
