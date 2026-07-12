"use strict";

function checkResultShape(label, result) {
    console.log(label + ".proto===ObjectPrototype", Object.getPrototypeOf(result) === Object.prototype);
    console.log(label + ".keys", JSON.stringify(Object.keys(result)));
    const desc = Object.getOwnPropertyDescriptor(result, "done");
    console.log(label + ".doneDescriptor", JSON.stringify(desc));
    result.extra = 42;
    console.log(label + ".extra", result.extra);
}

// Array iterator
const arrayIterator = [1, 2].values();
checkResultShape("array", arrayIterator.next());

// Map iterator
const map = new Map([["a", 1]]);
checkResultShape("map", map.entries().next());

// Set iterator
const set = new Set([1]);
checkResultShape("set", set.values().next());

// String iterator
const strIterator = "hi"[Symbol.iterator]();
checkResultShape("string", strIterator.next());

// Generator
function* gen() { yield 1; yield 2; }
const g = gen();
checkResultShape("generator", g.next());
console.log("generator.return", JSON.stringify(g.return(99)));
console.log("generator.throwProto", Object.getPrototypeOf(
    (function () {
        function* thrower() { try { yield 1; } catch (e) { return "caught"; } }
        const t = thrower();
        t.next();
        return t.throw(new Error("x"));
    })()
) === Object.prototype);

// Iterator helper
const helper = Iterator.from([1, 2, 3]).map(v => v * 2);
checkResultShape("helper", helper.next());

// JSON.parse object shape
const parsed = JSON.parse('{"a":1,"b":[2,3]}');
console.log("jsonParse.proto===ObjectPrototype", Object.getPrototypeOf(parsed) === Object.prototype);
console.log("jsonParse.keys", JSON.stringify(Object.keys(parsed)));
console.log("jsonParse.arrayIsArray", Array.isArray(parsed.b));
parsed.c = "added";
console.log("jsonParse.stringify", JSON.stringify(parsed));
