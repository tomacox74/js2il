"use strict";

const values = Iterator.from([1, 2, 3, 4])
    .map(value => value * 2)
    .filter(value => value > 2)
    .drop(1)
    .take(2)
    .toArray();

console.log(values.length);
console.log(values[0]);
console.log(values[1]);

console.log(Iterator.from([1, 2, 3]).reduce((sum, value) => sum + value, 0));
console.log(Iterator.from([1, 2, 3]).every(value => value > 0));
console.log(Iterator.from([1, 2, 3]).some(value => value === 2));
console.log(Iterator.from([1, 2, 3]).find(value => value > 1));

let total = 0;
Iterator.from([1, 2, 3]).forEach(value => { total += value; });
console.log(total);

const flatMapped = Iterator.from([1, 2])
    .flatMap(value => [value, value + 10])
    .toArray();

console.log(flatMapped.length);
console.log(flatMapped[0]);
console.log(flatMapped[1]);
console.log(flatMapped[2]);
console.log(flatMapped[3]);
