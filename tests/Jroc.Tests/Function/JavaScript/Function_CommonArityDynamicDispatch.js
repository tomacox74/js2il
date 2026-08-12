"use strict";

function collect() {
  const last = arguments.length === 0
    ? ""
    : arguments[arguments.length - 1];
  return arguments.length + ":" + last;
}

let fn = collect;
console.log(fn());
console.log(fn(1, 2, 3, 4));
console.log(fn(1, 2, 3, 4, 5));
console.log(fn(1, 2, 3, 4, 5, 6));

console.log((true ? fn : null)(7, 8, 9, 10, 11));

let holder = {};
holder.invoke = fn;
console.log(holder.invoke(12, 13, 14, 15));
console.log(holder.invoke(16, 17, 18, 19, 20));
console.log(holder["invoke"](21, 22, 23, 24));
console.log(holder["invoke"](25, 26, 27, 28, 29));

const inheritedHolder = Object.create(holder);
console.log(inheritedHolder.invoke(30, 31, 32, 33, 34));

Object.defineProperty(holder, "accessor", {
  get: function () {
    return fn;
  },
});
console.log(holder.accessor(35, 36, 37, 38, 39));

function Box() {
  this.count = arguments.length;
  this.last = arguments[arguments.length - 1];
}

let DynamicBox = Box;
const box0 = new DynamicBox();
const box4 = new DynamicBox(1, 2, 3, 4);
const box5 = new DynamicBox(1, 2, 3, 4, 5);
const box6 = new DynamicBox(1, 2, 3, 4, 5, 6);
console.log(box0.count);
console.log(box4.last);
console.log(box5.last);
console.log(box6.count);

const spreadValues = [31, 32, 33, 34, 35, 36];
console.log(fn(...spreadValues));
