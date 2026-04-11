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

var closedOnNormalCompletion = false;
var closableIterable = {
    [Symbol.iterator]() {
        var index = 0;
        var values = [4, 5];

        return {
            next() {
                if (index < values.length) {
                    return { value: values[index++], done: false };
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

var noCloseSet = new Set(closableIterable);
console.log(closedOnNormalCompletion);
console.log(noCloseSet.size);

var closedDuringNormalization = false;
var unionIterable = {
    [Symbol.iterator]() {
        var index = 0;
        var values = [5, 6];

        return {
            next() {
                if (index < values.length) {
                    return { value: values[index++], done: false };
                }

                return { value: undefined, done: true };
            },
            return() {
                closedDuringNormalization = true;
                return { done: true };
            }
        };
    }
};

var unionResult = noCloseSet.union(unionIterable);
console.log(closedDuringNormalization);
console.log(unionResult.has(4));
console.log(unionResult.has(6));
