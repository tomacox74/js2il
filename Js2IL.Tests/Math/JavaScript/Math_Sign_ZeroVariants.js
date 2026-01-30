"use strict";

// Validate Math.sign for +0, -0, positive/negative, NaN, and infinities
const vals = [0, (0 * (0 - 1)), 1, (0 - 1), NaN, Infinity, (Infinity * (0 - 1))];
const out = vals.map(v => Math.sign(v));
// Print with handling for -0 to show as "-0"; avoid Object.is and unary -Infinity literal
function toStr(n){
  return (n === 0 && (1/n) < 0) ? "-0" : ("" + n);
}
console.log(out.map(toStr).join(" "));
