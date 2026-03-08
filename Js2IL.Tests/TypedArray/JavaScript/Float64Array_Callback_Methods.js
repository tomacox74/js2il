"use strict";

const values = new Float64Array([1.5, 2.5, 3.5]);

console.log(values.map(function (value, index) {
  return value + index;
}).join("|"));

console.log(values.filter(function (value) {
  return value > 2;
}).join("|"));

console.log(values.every(function (value) {
  return value > 1;
}));

console.log(values.some(function (value) {
  return value > 3;
}));

console.log(values.find(function (value) {
  return value > 2;
}));

console.log(values.findIndex(function (value) {
  return value > 2;
}));

let total = 0;
values.forEach(function (value) {
  total += value;
});
console.log(total);

console.log(values.reduce(function (accumulator, value) {
  return accumulator + value;
}, 1));
