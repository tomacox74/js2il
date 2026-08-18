"use strict";

// Fixed-arity prototype methods invoked via .call() on a plain iterator receiver.
const it1 = [1, 2, 3][Symbol.iterator]();
const mapped = Iterator.prototype.map.call(it1, value => value * 2);
console.log(mapped.toArray().join(","));

const it2 = [1, 2, 3, 4][Symbol.iterator]();
console.log(Iterator.prototype.filter.call(it2, value => value % 2 === 0).toArray().join(","));

const it3 = [1, 2, 3][Symbol.iterator]();
console.log(Iterator.prototype.find.call(it3, value => value > 1));

const it4 = [1, 2, 3][Symbol.iterator]();
console.log(Iterator.prototype.some.call(it4, value => value === 2));

const it5 = [1, 2, 3][Symbol.iterator]();
console.log(Iterator.prototype.every.call(it5, value => value > 0));

const it6 = [1, 2][Symbol.iterator]();
console.log(Iterator.prototype.flatMap.call(it6, value => [value, value + 10]).toArray().join(","));

let total = 0;
const it7 = [1, 2, 3][Symbol.iterator]();
Iterator.prototype.forEach.call(it7, value => { total += value; });
console.log(total);

console.log(Iterator.prototype[Symbol.iterator].call(Iterator.from([1, 2])) === undefined);

// Variadic (count-sensitive) prototype methods.
console.log(Iterator.prototype.take.apply(Iterator.from([1, 2, 3]), [2]).toArray().join(","));

console.log(Iterator.prototype.reduce.call(Iterator.from([1, 2, 3]), (a, b) => a + b));
console.log(Iterator.prototype.reduce.call(Iterator.from([1, 2, 3]), (a, b) => a + b, 10));
console.log(Number.isNaN(Iterator.prototype.reduce.call(Iterator.from([1, 2, 3]), (a, b) => a + b, undefined)));

// toArray and receiver errors.
console.log(Iterator.prototype.toArray.call(Iterator.from([9, 8])).join(","));
try {
    Iterator.prototype.next.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    Iterator.prototype.map.call({}, value => value);
} catch (error) {
    console.log(error instanceof TypeError);
}

// Metadata.
console.log(Iterator.prototype.map.length);
console.log(Iterator.prototype.next.length);
console.log(Iterator.from.length);
