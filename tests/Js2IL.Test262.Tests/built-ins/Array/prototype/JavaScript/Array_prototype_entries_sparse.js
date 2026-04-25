"use strict";

var array = [0, "a", true, false, null, , undefined, NaN];
var i = 0;

for (var value of array.entries()) {
  console.log(value[0]);
  console.log(value[1]);
  console.log(value.length);
  i++;
}

console.log("count", i);
