"use strict";

console.log(String.fromCodePoint(0x41, 0x1F600));
console.log(String.raw({ raw: ["a", "b", "c"] }, 1, 2));
console.log(String.raw({ raw: ["a", "b"] }, 1, 2));
console.log("abc".at(0));
console.log("abc".at(-1));
console.log(String("abc".at(99)));
console.log("A\uD83D\uDE00B".codePointAt(1));
console.log(String("A\uD83D\uDE00B".codePointAt(20)));
console.log("5".padStart(3, "0"));
console.log("5".padEnd(4, "xy"));
console.log("foofoo".replaceAll("oo", "ar"));
console.log("aba".replaceAll("", "-"));
console.log("\uD83D\uDE00".isWellFormed());
console.log("\uD83D".isWellFormed());
console.log("A\uD83D".toWellFormed().charCodeAt(1));
