async function main() {
    for (const cause of ["mapper", "borrowed mapper", "define"]) {
        for (const mode of ["done true", "done false", "getter throw", "throw", "primitive", "done throw", "value throw", "reject"]) {
            const events = [];
            const original = {};
            const closeError = {};
            let closes = 0;
            const iterator = {
                next() {
                    Object.defineProperty(iterator, "return", {
                        configurable: true,
                        get() {
                            events.push("get return");
                            if (mode === "getter throw") throw closeError;
                            return function () {
                                events.push("return");
                                closes++;
                                if (mode === "throw" && closes === 1) throw closeError;
                                if (mode === "primitive" && closes === 1) return 1;
                                return {
                                    get then() {
                                        events.push("unexpected then");
                                        throw closeError;
                                    },
                                    get done() {
                                        events.push("done");
                                        if (mode === "done throw" && closes === 1) throw closeError;
                                        return mode !== "done false";
                                    },
                                    get value() {
                                        events.push("value");
                                        if (mode === "value throw" && closes === 1) throw closeError;
                                        return {
                                            then(resolve, reject) {
                                                events.push("await value");
                                                if (mode === "reject" && closes === 1) reject(closeError);
                                                else resolve(8);
                                            }
                                        };
                                    }
                                };
                            };
                        }
                    });
                    return { done: false, value: 1 };
                },
                get return() {
                    events.push("early return");
                    throw closeError;
                },
                [Symbol.iterator]() { return this; }
            };
            function Result() {
                if (cause === "define") {
                    Object.defineProperty(this, "0", { value: 0, configurable: false });
                }
            }
            function mapper(value) {
                if (cause !== "define") throw original;
                return value;
            }
            try {
                if (cause === "mapper") await Array.fromAsync(iterator, mapper);
                else await Array.fromAsync.call(Result, iterator, mapper);
                console.log("unexpected fulfillment");
            } catch (error) {
                console.log(cause, mode,
                    cause === "define" ? error instanceof TypeError : error === original,
                    events.join(","));
            }
        }
    }
}
main().catch(function (error) { console.log("unexpected", error); });
