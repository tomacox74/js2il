"use strict";

var iterable = {
    [Symbol.iterator]() {
        var index = 0;
        var values = [1, 2, 2, 3];

        return {
            next() {
                if (index < values.length) {
                    return { value: values[index++], done: false };
                }

                return { value: undefined, done: true };
            }
        };
    }
};

var set = new Set(iterable);
console.log(set.size);
console.log(set.has(1));
console.log(set.has(3));
console.log(set.has(4));

