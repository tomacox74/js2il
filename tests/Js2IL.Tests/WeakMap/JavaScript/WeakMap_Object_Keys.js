"use strict";

var wm = new WeakMap();
var obj1 = { name: "obj1" };
var obj2 = { name: "obj2" };
wm.set(obj1, "value1");
wm.set(obj2, "value2");
console.log(wm.get(obj1));
console.log(wm.get(obj2));
