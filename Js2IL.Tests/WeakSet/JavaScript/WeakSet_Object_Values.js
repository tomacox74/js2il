"use strict";

var ws = new WeakSet();
var obj1 = { name: "obj1" };
var obj2 = { name: "obj2" };
ws.add(obj1);
ws.add(obj2);
console.log(ws.has(obj1));
console.log(ws.has(obj2));
