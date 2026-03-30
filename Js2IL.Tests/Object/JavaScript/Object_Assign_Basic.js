"use strict";

// Object.assign ( target , ... sources )

var target = { a: 1 };
var source1 = { b: 2, c: 3 };
var source2 = { c: 4, d: 5 };

var result = Object.assign(target, source1, source2);

console.log('same_ref=' + (result === target));
console.log('a=' + target.a);
console.log('b=' + target.b);
console.log('c=' + target.c);
console.log('d=' + target.d);
