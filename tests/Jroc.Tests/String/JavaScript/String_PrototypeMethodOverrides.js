"use strict";

function callTrim(value) {
    return value.trim();
}

var boxed = new String("  boxed  ");
console.log(boxed.trim());
console.log(typeof boxed);

var own = new String("own-source");
own.trim = function () {
    return "own";
};
console.log(own.trim());

var defined = new String("defined-source");
Object.defineProperty(defined, "trim", {
    value: function () {
        return "defined";
    }
});
console.log(defined.trim());

var customPrototype = {
    trim: function () {
        return "custom-prototype";
    }
};
var custom = new String("custom-source");
Object.setPrototypeOf(custom, customPrototype);
console.log(custom.trim());

console.log(callTrim("  uncertain  "));
console.log(callTrim({
    trim: function () {
        return "object-fallback";
    }
}));

var originalCharAt = String.prototype.charAt;
String.prototype.charAt = function (index) {
    return "replaced-" + index;
};
console.log("abc".charAt(1));
String.prototype.charAt = originalCharAt;

var originalTrim = Object.getOwnPropertyDescriptor(String.prototype, "trim");
Object.defineProperty(String.prototype, "trim", {
    configurable: true,
    get: function () {
        console.log("trim-getter");
        return function () {
            return "accessor-result";
        };
    }
});
console.log(" abc ".trim());
Object.defineProperty(String.prototype, "trim", originalTrim);

var originalToUpperCase = String.prototype.toUpperCase;
var originalObjectToUpperCase = Object.prototype.toUpperCase;
delete String.prototype.toUpperCase;
Object.prototype.toUpperCase = function () {
    return "inherited";
};
console.log("abc".toUpperCase());
String.prototype.toUpperCase = originalToUpperCase;
if (originalObjectToUpperCase === undefined) {
    delete Object.prototype.toUpperCase;
} else {
    Object.prototype.toUpperCase = originalObjectToUpperCase;
}

var originalIncludes = String.prototype.includes;
delete String.prototype.includes;
try {
    "abc".includes("a");
} catch (error) {
    console.log(error.name);
}
String.prototype.includes = originalIncludes;

var originalStartsWith = String.prototype.startsWith;
String.prototype.startsWith = function (prefix) {
    return "alias-" + prefix;
};
var startsWith = "abc".startsWith;
console.log(startsWith.call("abc", "a"));
console.log(startsWith.apply("abc", ["b"]));
String.prototype.startsWith = originalStartsWith;

var originalCharCodeAt = String.prototype.charCodeAt;
String.prototype.charCodeAt = function () {
    return "41.9";
};
console.log(Math.floor("A".charCodeAt(0)));
console.log(+"A".charCodeAt(0));
String.prototype.charCodeAt = originalCharCodeAt;
