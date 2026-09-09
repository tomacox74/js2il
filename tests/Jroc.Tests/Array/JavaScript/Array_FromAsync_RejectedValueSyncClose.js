async function main() {
    for (const borrowed of [false, true]) {
        for (const mode of ["nonsettling thenable", "throw", "primitive"]) {
            const marker = {};
            let closes = 0;
            let thenReads = 0;
            let valueReads = 0;
            let doneReads = 0;
            let outcome = "pending";
            const iterator = {
                next() {
                    return { done: false, value: Promise.reject(marker) };
                },
                return() {
                    closes++;
                    if (mode === "throw" && closes === 1) throw "close error";
                    if (mode === "primitive" && closes === 1) return 1;
                    return {
                        get then() {
                            thenReads++;
                            return function () {};
                        },
                        get done() { doneReads++; return true; },
                        get value() { valueReads++; return Promise.resolve(1); }
                    };
                },
                [Symbol.iterator]() { return this; }
            };
            const pending = borrowed
                ? Array.fromAsync.call(Array, iterator)
                : Array.fromAsync(iterator);
            pending.then(
                function () { outcome = "fulfilled"; },
                function (error) { outcome = error === marker ? "original rejection" : "wrong rejection"; }
            );
            // A broken close must not hang the test by awaiting the nonsettling thenable.
            for (let tick = 0; tick < 20; tick++) await Promise.resolve();
            console.log(borrowed, mode, outcome, closes, thenReads, doneReads, valueReads);
        }
    }
}
main().catch(function (error) { console.log("unexpected", error); });
