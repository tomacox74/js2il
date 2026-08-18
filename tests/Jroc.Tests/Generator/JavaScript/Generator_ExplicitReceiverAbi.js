"use strict";

function* g() {
    try {
        const x = yield 1;
        yield x + 1;
    } catch (error) {
        yield `caught:${error}`;
    }
    return "done-value";
}

// %GeneratorPrototype% is not directly exposed as a global; obtain it via an
// instance's prototype chain, as ECMA-262 intends.
const GeneratorPrototype = Object.getPrototypeOf(g());

// next / return / throw invoked via .call() on the prototype methods.
const it1 = g();
const r1 = GeneratorPrototype.next.call(it1);
console.log(`r1: ${r1.value} done: ${r1.done}`);
const r2 = GeneratorPrototype.next.call(it1, 10);
console.log(`r2: ${r2.value} done: ${r2.done}`);

const it2 = g();
GeneratorPrototype.next.call(it2);
const r3 = GeneratorPrototype.throw.call(it2, "boom");
console.log(`r3: ${r3.value} done: ${r3.done}`);

const it3 = g();
GeneratorPrototype.next.call(it3);
const r4 = GeneratorPrototype.return.call(it3, 99);
console.log(`r4: ${r4.value} done: ${r4.done}`);

// Receiver errors.
try {
    GeneratorPrototype.next.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    GeneratorPrototype.return.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    GeneratorPrototype.throw.call({}, "x");
} catch (error) {
    console.log(error instanceof TypeError);
}

console.log(GeneratorPrototype.next.length);
console.log(GeneratorPrototype.return.length);
console.log(GeneratorPrototype.throw.length);
