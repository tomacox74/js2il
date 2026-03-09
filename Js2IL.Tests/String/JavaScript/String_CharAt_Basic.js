"use strict";

const text = "hello";
console.log(text.charAt());
console.log(text.charAt(1));
console.log(text.charAt(4));
console.log(text.charAt(5) === "");
console.log(text.charAt(-1) === "");
try {
    console.log(text.charAt(BigInt(1)));
} catch (error) {
    console.log("bigint threw:", true);
    console.log("bigint name:", error.name);
    console.log("bigint message:", error.message);
}

const wrapped = new String("world");
console.log(wrapped.charAt(0));
console.log(wrapped.charAt(2));
