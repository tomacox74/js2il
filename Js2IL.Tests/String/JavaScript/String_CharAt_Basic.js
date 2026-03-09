"use strict";

const text = "hello";
console.log(text.charAt());
console.log(text.charAt(1));
console.log(text.charAt(4));
console.log(text.charAt(5) === "");
console.log(text.charAt(-1) === "");

const wrapped = new String("world");
console.log(wrapped.charAt(0));
console.log(wrapped.charAt(2));
