"use strict";

var modern = new RegExp("\\u{1F600}.", "dus");
console.log(modern.dotAll);
console.log(modern.unicode);
console.log(modern.hasIndices);
console.log(modern.sticky);
console.log(modern.flags);

console.log(/^.$/u.test("😀"));
console.log(/\u{1F600}/u.test("😀"));
console.log(/^.$/s.test("\n"));
console.log(/^.$/.test("\r"));
console.log(/^.$/.test("\u2028"));
console.log(/^.$/.test("\u2029"));

var sets = new RegExp("a", "v");
console.log(sets.unicodeSets);
console.log(sets.unicode);
console.log(sets.flags);

try {
    new RegExp("a", "gg");
    console.log("no-error");
} catch (e) {
    console.log(e.name);
}
