// test262: test/built-ins/BigInt/asUintN/not-a-constructor.js
function isConstructor(fn) {
    try {
        Reflect.construct(function () { }, [], fn);
    } catch (e) {
        return false;
    }
    return true;
}

var threw = false;
try {
    new BigInt.asUintN(2, 1n);
} catch (e) {
    threw = e && e.name === "TypeError";
}

console.log(isConstructor(BigInt.asUintN) === false);
console.log(threw);
