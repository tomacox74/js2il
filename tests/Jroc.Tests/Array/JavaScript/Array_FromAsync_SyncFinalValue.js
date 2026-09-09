async function main() {
    for (const borrowed of [false, true]) {
        for (const mode of ["resolve", "throw", "reject", "generator"]) {
            const events = [];
            const marker = {};
            let closes = 0;
            let iterator;
            if (mode === "generator") {
                function* generate() { return Promise.reject(marker); }
                iterator = generate();
            } else {
                iterator = {
                    next() {
                        return {
                            done: true,
                            get value() {
                                events.push("value");
                                if (mode === "throw") throw marker;
                                return {
                                    then(resolve, reject) {
                                        events.push("await");
                                        if (mode === "reject") reject(marker);
                                        else resolve(9);
                                    }
                                };
                            }
                        };
                    },
                    [Symbol.iterator]() { return this; }
                };
            }
            iterator.return = function () {
                closes++;
                return { done: true };
            };
            try {
                const result = borrowed
                    ? await Array.fromAsync.call(Array, iterator)
                    : await Array.fromAsync(iterator);
                console.log(borrowed, mode, "fulfilled", result.length, closes, events.join(",") || "none");
            } catch (error) {
                console.log(borrowed, mode, "rejected", error === marker, closes, events.join(",") || "none");
            }
        }
    }
}
main().catch(function (error) { console.log("unexpected", error); });
