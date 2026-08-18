"use strict";

async function* g() {
    try {
        const x = yield 1;
        yield x + 1;
    } catch (error) {
        yield `caught:${error}`;
    }
    return "done-value";
}

(async () => {
    // %AsyncGeneratorPrototype% is not directly exposed as a global; obtain it
    // via an instance's prototype chain, as ECMA-262 intends.
    const AsyncGeneratorPrototype = Object.getPrototypeOf(g());

    const it1 = g();
    const r1 = await AsyncGeneratorPrototype.next.call(it1);
    console.log(`r1: ${r1.value} done: ${r1.done}`);
    const r2 = await AsyncGeneratorPrototype.next.call(it1, 10);
    console.log(`r2: ${r2.value} done: ${r2.done}`);

    const it2 = g();
    await AsyncGeneratorPrototype.next.call(it2);
    const r3 = await AsyncGeneratorPrototype.throw.call(it2, "boom");
    console.log(`r3: ${r3.value} done: ${r3.done}`);

    const it3 = g();
    await AsyncGeneratorPrototype.next.call(it3);
    const r4 = await AsyncGeneratorPrototype.return.call(it3, 99);
    console.log(`r4: ${r4.value} done: ${r4.done}`);

    console.log(AsyncGeneratorPrototype.next.length);
    console.log(AsyncGeneratorPrototype.return.length);
    console.log(AsyncGeneratorPrototype.throw.length);
})();
