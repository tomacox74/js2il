"use strict";

// `var s = new String(); s += "a"` in a loop: the local starts as a String wrapper object
// (not statically a string), so the compiler uses the dynamic concat accumulator.
// Every JS-visible read must still observe a plain string with correct + semantics.
function concatFromWrapper() {
    var str = new String();
    var before = typeof str;
    for (var i = 0; i < 5; i++) {
        str += "a";
    }
    return before + " " + typeof str + " " + str + " " + str.length;
}
console.log(concatFromWrapper());

function readsBetweenAppends() {
    var str = new String("a");
    str += "b";
    var snapshot = str;
    str += "c";
    if (str.length === 3) {
        str += "-";
    }
    return snapshot + " " + str + " " + (snapshot === "ab");
}
console.log(readsBetweenAppends());

function assignmentResultIsString() {
    var str = new String("q");
    var result = (str += "w");
    return result + "|" + typeof result + "|" + str;
}
console.log(assignmentResultIsString());

function customToPrimitive() {
    var obj = { valueOf() { return 42; }, toString() { return "str"; } };
    var str = obj;
    str += "x";
    str += "y";
    return str;
}
console.log(customToPrimitive());

function mixedAppends() {
    var str = new String("m");
    str += "n";
    str = 5;
    str += "o";
    str += str;
    str += `t${1 + 1}`;
    return str;
}
console.log(mixedAppends());

function symbolThrows() {
    var str = new String("z");
    try {
        str += Symbol("s");
    } catch (err) {
        return err instanceof TypeError;
    }
    return "no throw";
}
console.log(symbolThrows());

function nullishStarts() {
    var a = undefined;
    a += "u";
    a += "v";
    var b = null;
    b += "w";
    return a + " " + b;
}
console.log(nullishStarts());
