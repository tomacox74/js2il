// test262: test/built-ins/BigInt/asIntN/not-a-constructor.js
var threw = false;
try {
    new BigInt.asIntN(2, 1n);
} catch (e) {
    threw = e && e.name === "TypeError";
}

console.log(isConstructor(BigInt.asIntN) === false);
console.log(threw);
