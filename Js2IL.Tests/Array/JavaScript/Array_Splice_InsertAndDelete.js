"use strict";\r\n\r\nconst arr = [0,1,2,3];
const removed = arr.splice(1, 1, 'a', 'b');
console.log(removed.join(',')); // 1
console.log(arr.join(','));     // 0,a,b,2,3
