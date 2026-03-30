"use strict";

var set = new Set([1, 2, 3]);
var other = [3, 4];

console.log(Array.from(set.difference(other)).join(","));
console.log(Array.from(set.intersection(other)).join(","));
console.log(Array.from(set.union(other)).join(","));
console.log(Array.from(set.symmetricDifference(other)).join(","));
console.log(set.isDisjointFrom([4, 5]));
console.log(set.isDisjointFrom(other));
console.log(new Set([1, 2]).isSubsetOf([1, 2, 3]));
console.log(set.isSupersetOf([1, 3]));
