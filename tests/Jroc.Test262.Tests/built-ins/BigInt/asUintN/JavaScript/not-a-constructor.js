// test262: test/built-ins/BigInt/asUintN/not-a-constructor.js
var threw = false;
try {
    new BigInt.asUintN(2, 1n);
} catch (e) {
    threw = e && e.name === "TypeError";
}

console.log(isConstructor(BigInt.asUintN) === false);
console.log(threw);
