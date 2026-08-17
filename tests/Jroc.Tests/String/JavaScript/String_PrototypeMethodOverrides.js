"use strict";

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

var originalStartsWith = String.prototype.startsWith;
String.prototype.startsWith = function (prefix) {
    return "alias-" + prefix;
};
var startsWith = "abc".startsWith;
console.log(startsWith.call("abc", "a"));
console.log(startsWith.apply("abc", ["b"]));
String.prototype.startsWith = originalStartsWith;
