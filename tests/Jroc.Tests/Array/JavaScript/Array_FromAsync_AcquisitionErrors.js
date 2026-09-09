async function main() {
    for (const kind of ["sync", "async"]) {
        for (const failure of ["method", "next getter", "uncallable next"]) {
            const events = [];
            const marker = {};
            const iterator = {
                get next() {
                    events.push("next getter");
                    if (failure === "next getter") throw marker;
                    return 1;
                }
            };
            const input = {};
            input[kind === "sync" ? Symbol.iterator : Symbol.asyncIterator] = function () {
                events.push("method");
                if (failure === "method") throw marker;
                return iterator;
            };
            function Result() { events.push("constructor"); }
            try {
                await Array.fromAsync.call(Result, input);
                console.log("unexpected fulfillment");
            } catch (error) {
                console.log(kind, failure,
                    failure === "uncallable next" ? error instanceof TypeError : error === marker,
                    events.join(","));
            }
        }
    }
}
main().catch(function (error) { console.log("unexpected", error); });
