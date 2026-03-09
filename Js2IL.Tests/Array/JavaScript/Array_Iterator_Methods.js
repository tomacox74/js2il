"use strict";

const arr = [1, 2];

const keys = Array.from(arr.keys());
console.log(keys.length);
console.log(keys[0]);
console.log(keys[1]);

const values = Array.from(arr.values());
console.log(values.length);
console.log(values[0]);
console.log(values[1]);

const entries = Array.from(arr.entries());
console.log(entries.length);
console.log(entries[0][0]);
console.log(entries[0][1]);
console.log(entries[1][0]);
console.log(entries[1][1]);

const iter = Array.prototype[Symbol.iterator].call(arr);
console.log(iter.next().value);
console.log(iter.next().value);
console.log(iter.next().done);

const sparse = new Array(2);
sparse[1] = "x";
const sparseValues = Array.from(sparse.values());
console.log(sparseValues.length);
console.log(sparseValues[0] === undefined);
console.log(sparseValues[1]);

const arrayLike = { 0: "a", 1: "b", length: 2 };
const borrowed = Array.from(Array.prototype.values.call(arrayLike));
console.log(borrowed.length);
console.log(borrowed[0]);
console.log(borrowed[1]);

const overridden = [1, 2];
overridden[Symbol.iterator] = function () {
  let used = false;
  return {
    next: function () {
      if (used) {
        return { value: undefined, done: true };
      }

      used = true;
      return { value: "override", done: false };
    },
  };
};

console.log([...overridden].join(","));

const collected = [];
for (const value of overridden) {
  collected.push(value);
}
console.log(collected.join(","));
