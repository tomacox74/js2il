"use strict";

var array = [0, "a", true, false, null, , undefined, NaN];
var i = 0;

for (var value of array[Symbol.iterator]()) {
  console.log(value);
  i++;
}

console.log(i);
