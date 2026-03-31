"use strict";

var iterable = {
    [Symbol.iterator]() {
        var index = 0;
        var entries = [
            ["alpha", 1],
            ["beta", 2],
            ["alpha", 3]
        ];

        return {
            next() {
                if (index < entries.length) {
                    return { value: entries[index++], done: false };
                }

                return { value: undefined, done: true };
            }
        };
    }
};

var map = new Map(iterable);
console.log(map.size);
console.log(map.get("alpha"));
console.log(map.get("beta"));

var closedOnNormalCompletion = false;
var closableIterable = {
    [Symbol.iterator]() {
        var index = 0;
        var entries = [
            ["gamma", 4],
            ["delta", 5]
        ];

        return {
            next() {
                if (index < entries.length) {
                    return { value: entries[index++], done: false };
                }

                return { value: undefined, done: true };
            },
            return() {
                closedOnNormalCompletion = true;
                return { done: true };
            }
        };
    }
};

var normalCompletionMap = new Map(closableIterable);
console.log(closedOnNormalCompletion);
console.log(normalCompletionMap.size);

var shortEntryMap = new Map([
    ["short"]
]);
console.log(shortEntryMap.has("short"));
console.log(shortEntryMap.get("short") === undefined);

var closedOnAbruptCompletion = false;
var invalidIterable = {
    [Symbol.iterator]() {
        var done = false;

        return {
            next() {
                if (!done) {
                    done = true;
                    return { value: "ab", done: false };
                }

                return { value: undefined, done: true };
            },
            return() {
                closedOnAbruptCompletion = true;
                return { done: true };
            }
        };
    }
};

try {
    new Map(invalidIterable);
    console.log("no error");
} catch (error) {
    console.log(error.name);
}

console.log(closedOnAbruptCompletion);
