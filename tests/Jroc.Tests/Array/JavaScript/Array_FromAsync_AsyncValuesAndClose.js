async function main() {
    for (const borrowed of [false, true]) {
        let valueAwaits = 0;
        let finalReads = 0;
        const value = { then(resolve) { valueAwaits++; resolve(42); } };
        let step = 0;
        const iterator = {
            next() {
                if (step++ === 0) return Promise.resolve({ done: false, value: value });
                return Promise.resolve({
                    done: true,
                    get value() { finalReads++; throw "final value must not be read"; }
                });
            },
            [Symbol.asyncIterator]() { return this; }
        };
        const result = borrowed
            ? await Array.fromAsync.call(Array, iterator)
            : await Array.fromAsync(iterator);
        console.log("values", borrowed, result[0] === value, valueAwaits, finalReads);

        const events = [];
        const original = {};
        const closing = {
            next() { return Promise.resolve({ done: false, value: value }); },
            return() {
                events.push("return");
                return {
                    then(resolve) {
                        events.push("await result");
                        resolve({
                            get done() { events.push("done"); return true; },
                            get value() { events.push("value"); return Promise.reject("wrong"); }
                        });
                    }
                };
            },
            [Symbol.asyncIterator]() { return this; }
        };
        function mapper(item) {
            events.push(item === value ? "raw value" : "wrong value");
            throw original;
        }
        try {
            if (borrowed) await Array.fromAsync.call(Array, closing, mapper);
            else await Array.fromAsync(closing, mapper);
        } catch (error) {
            console.log("close", borrowed, error === original, valueAwaits, events.join(","));
        }
    }
}
main().catch(function (error) { console.log("unexpected", error); });
