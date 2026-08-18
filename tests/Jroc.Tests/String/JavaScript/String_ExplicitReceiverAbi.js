"use strict";

console.log(String.prototype.slice.call("abcdef", 1, 4));
console.log(String.prototype.toUpperCase.call(new String("mixed")));
console.log(String.prototype.startsWith.call(12345, "23"));
console.log(String.prototype.endsWith.apply("abcdef", ["de", 5]));
console.log(String.prototype.concat.call("a", "b", "c", "d", "e", "f", "g"));
console.log("abcdef".substr(-3, 2));
console.log(String.prototype.trimLeft === String.prototype.trimStart);
console.log(String.prototype.trimRight === String.prototype.trimEnd);

try {
    String.prototype.trim.call(null);
} catch (error) {
    console.log(error instanceof TypeError);
}

var originalSlice = String.prototype.slice;
String.prototype.slice = function () {
    return "overridden";
};
console.log("abcdef".slice(1, 4));
String.prototype.slice = originalSlice;
