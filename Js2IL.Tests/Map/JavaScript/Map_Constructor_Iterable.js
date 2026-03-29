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

