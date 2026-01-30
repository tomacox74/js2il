"use strict";

const arr = [0,1,2,3,4,5];
let removed;

// delete from middle
removed = arr.splice(2, 2); // [2,3], arr -> [0,1,4,5]
console.log(removed.join(','));
console.log(arr.join(','));

// insert without delete
removed = arr.splice(2, 0, 2, 3); // [], arr -> [0,1,2,3,4,5]
console.log(removed.join(','));
console.log(arr.join(','));

// delete till end by omitting count
removed = arr.splice(4); // [4,5], arr -> [0,1,2,3]
console.log(removed.join(','));
console.log(arr.join(','));

// negative start and over-large deleteCount
removed = arr.splice(-3, 10); // from index 1, delete to end => [1,2,3], arr -> [0]
console.log(removed.join(','));
console.log(arr.join(','));
