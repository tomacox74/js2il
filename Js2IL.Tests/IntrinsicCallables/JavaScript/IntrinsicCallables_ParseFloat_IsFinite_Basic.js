"use strict";

console.log(parseFloat("1.25abc"));
console.log(parseFloat("   -3.5e2zzz"));
console.log(parseFloat("Infinity"));
console.log(parseFloat("-Infinity"));
console.log(parseFloat("not-a-number"));

console.log(isFinite(123));
console.log(isFinite(Infinity));
console.log(isFinite(-Infinity));
console.log(isFinite(NaN));
console.log(isFinite("  4.5 "));
