"use strict";

var grouped = Object.groupBy([1, 2, 3, 4, 5], function (n) {
  return (n % 2 === 0) ? "even" : "odd";
});

console.log(grouped.even.length);
console.log(grouped.odd.length);
console.log(grouped.even[0] === 2 && grouped.even[1] === 4);
console.log(grouped.odd[0] === 1 && grouped.odd[1] === 3 && grouped.odd[2] === 5);
