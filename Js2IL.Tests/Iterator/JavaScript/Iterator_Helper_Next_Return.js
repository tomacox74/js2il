"use strict";

let closed = 0;
const iterable = {
    [Symbol.iterator]() {
        let value = 0;
        return {
            next() {
                value++;
                return { value, done: value > 3 };
            },
            return() {
                closed++;
                return { value: "closed", done: true };
            }
        };
    }
};

const helper = Iterator.from(iterable).map(value => value * 10);
console.log(helper[Symbol.iterator]() === helper);

let step = helper.next();
console.log(step.value);
console.log(step.done);

const returned = helper.return();
console.log(returned.done);
console.log(closed);

step = helper.next();
console.log(step.done);

const arrayIterator = [7, 8].values();
console.log(typeof Iterator);
console.log(typeof Iterator.from);
console.log(typeof Iterator.prototype.map);
console.log(arrayIterator[Symbol.iterator]() === arrayIterator);
console.log(arrayIterator.next().value);
console.log(arrayIterator.next().value);
console.log(arrayIterator.next().done);

const asyncSentinel = { kind: "async" };
console.log(typeof AsyncIterator);
console.log(AsyncIterator.prototype[Symbol.asyncIterator].call(asyncSentinel) === asyncSentinel);
