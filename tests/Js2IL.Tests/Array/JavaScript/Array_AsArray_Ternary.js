"use strict";

// Confirms ternary using Array.isArray result
function asArray(v) { 
    return Array.isArray(v) ? v : (v == null ? [] : [v]); 
}

const a = asArray([1]);
const b = asArray(0);
console.log(a.length);
console.log(b.length);
