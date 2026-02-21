"use strict";

const a = BigInt(10);
const b = BigInt(3);

console.log(typeof (a + b));
console.log(a + b);
console.log(a.toString());
console.log(a.toString(16));
console.log(a - b);
console.log(a * b);
console.log(a / b);
console.log(a % b);
console.log(a ** BigInt(3));
console.log(a & b);
console.log(a | b);
console.log(a ^ b);
console.log(a << BigInt(2));
console.log(a >> BigInt(1));
console.log(a < b);
console.log(a > b);
console.log(a <= b);
console.log(a >= b);
console.log(a == BigInt(10));
console.log(a === BigInt(10));
console.log(-a);
console.log(~a);

try {
    console.log(a / BigInt(0));
} catch (e) {
    console.log(e.name);
}

try {
    console.log(a % BigInt(0));
} catch (e) {
    console.log(e.name);
}

try {
    console.log(a ** BigInt(-1));
} catch (e) {
    console.log(e.name);
}

try {
    console.log(a >>> BigInt(1));
} catch (e) {
    console.log(e.name);
}
