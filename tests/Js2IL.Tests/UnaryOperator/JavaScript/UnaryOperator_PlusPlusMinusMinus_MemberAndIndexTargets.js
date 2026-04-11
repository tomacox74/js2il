"use strict";

let doc = { _nextnid: 7 };
let node = { ownerDocument: doc, _nid: 0 };

node._nid = node.ownerDocument._nextnid++;
console.log(node._nid);
console.log(node.ownerDocument._nextnid);

let o = { a: 1 };
console.log(o.a++);
console.log(o.a);
console.log(++o.a);
console.log(o.a);

let arr = [10];
console.log(arr[0]++);
console.log(arr[0]);
console.log(++arr[0]);
console.log(arr[0]);

console.log(--arr[0]);
console.log(arr[0]--);
console.log(arr[0]);
