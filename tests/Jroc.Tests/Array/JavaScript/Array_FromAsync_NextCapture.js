async function main() {
    for (const kind of ["sync", "async"]) {
        for (const borrowed of [false, true]) {
            const events = [];
            let step = 0;
            const iterator = {};
            Object.defineProperty(iterator, "next", {
                configurable: true,
                get() {
                    events.push("get next");
                    return function () {
                        events.push("next");
                        Object.defineProperty(iterator, "next", {
                            configurable: true,
                            value() {
                                events.push("replacement");
                                return { done: true };
                            }
                        });
                        return { done: step++ > 0, value: 7 };
                    };
                }
            });
            const input = {};
            input[kind === "sync" ? Symbol.iterator : Symbol.asyncIterator] = function () {
                events.push("iterator");
                return iterator;
            };
            function Result() {
                events.push("constructor");
                Object.defineProperty(iterator, "next", {
                    configurable: true,
                    value() {
                        events.push("constructor replacement");
                        return { done: true };
                    }
                });
            }
            const result = borrowed
                ? await Array.fromAsync.call(Result, input)
                : await Array.fromAsync(input);
            console.log(kind, borrowed, result.length, result[0], events.join(","));
        }
    }
}
main().catch(function (error) { console.log("unexpected", error); });
