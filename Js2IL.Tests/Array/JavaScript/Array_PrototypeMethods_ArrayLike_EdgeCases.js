"use strict";

// Missing callback must throw TypeError (regardless of length/initial value)
var arrayLikeEmpty = { length: 0 };
try {
  Array.prototype.reduce.call(arrayLikeEmpty);
  console.log("no-throw");
} catch (e) {
  console.log(e.name);
}

try {
  Array.prototype.reduceRight.call(arrayLikeEmpty);
  console.log("no-throw");
} catch (e) {
  console.log(e.name);
}

// Sparse/"hole" semantics for array-like objects (skip missing indexes)
var sparse = { length: 3, 1: "b" };

var calls = 0;
var r1 = Array.prototype.reduce.call(sparse, function (acc, x) {
  calls++;
  return acc + x;
});
console.log(r1);
console.log(calls);

calls = 0;
var r2 = Array.prototype.reduce.call(sparse, function (acc, x) {
  calls++;
  return acc + x;
}, "");
console.log(r2);
console.log(calls);

calls = 0;
var r3 = Array.prototype.reduceRight.call(sparse, function (acc, x) {
  calls++;
  return acc + x;
});
console.log(r3);
console.log(calls);

calls = 0;
var r4 = Array.prototype.reduceRight.call(sparse, function (acc, x) {
  calls++;
  return acc + x;
}, "");
console.log(r4);
console.log(calls);

// All holes with no initial value must throw
var holes = { length: 2 };
try {
  Array.prototype.reduce.call(holes, function (acc, x) {
    return acc + x;
  });
  console.log("no-throw");
} catch (e) {
  console.log(e.name);
}

try {
  Array.prototype.reduceRight.call(holes, function (acc, x) {
    return acc + x;
  });
  console.log("no-throw");
} catch (e) {
  console.log(e.name);
}

// indexOf fromIndex clamping (±Infinity / huge magnitudes)
var arrayLike = { 0: "a", 1: "b", length: 2 };
console.log(Array.prototype.indexOf.call(arrayLike, "b", Infinity));
console.log(Array.prototype.indexOf.call(arrayLike, "b", -Infinity));
console.log(Array.prototype.indexOf.call(arrayLike, "b", 9999999999));
console.log(Array.prototype.indexOf.call(arrayLike, "b", -9999999999));
