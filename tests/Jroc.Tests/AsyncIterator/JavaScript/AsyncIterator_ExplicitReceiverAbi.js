"use strict";

async function* g() {
    yield 1;
    yield 2;
    return 3;
}

(async () => {
    const it1 = g();
    const r1 = await AsyncIterator.prototype.next.call(it1);
    console.log(`v1: ${r1.value} done: ${r1.done}`);

    const it2 = g();
    await it2.next();
    const r2 = await AsyncIterator.prototype.return.call(it2, 42);
    console.log(`v2: ${r2.value} done: ${r2.done}`);

    const it3 = g();
    console.log(AsyncIterator.prototype[Symbol.asyncIterator].call(it3) === it3);

    // Receiver error (thrown synchronously by the receiver check, not via rejection).
    try {
        AsyncIterator.prototype.next.call({});
    } catch (error) {
        console.log(error instanceof TypeError);
    }

    console.log(AsyncIterator.prototype.next.length);
    console.log(AsyncIterator.prototype.return.length);
})();
