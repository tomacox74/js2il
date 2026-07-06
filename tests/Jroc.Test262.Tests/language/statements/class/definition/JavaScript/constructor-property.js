var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {}
assert.sameValue(
    C,
    C.prototype.constructor,
    "The value of `C` is `C.prototype.constructor`"
);
var desc = Object.getOwnPropertyDescriptor(C.prototype, 'constructor');
assert.sameValue(desc.configurable, true, "The value of `desc.configurable` is `true`, after executing `var desc = Object.getOwnPropertyDescriptor(C.prototype, 'constructor');`");
assert.sameValue(desc.enumerable, false, "The value of `desc.enumerable` is `false`, after executing `var desc = Object.getOwnPropertyDescriptor(C.prototype, 'constructor');`");
assert.sameValue(desc.writable, true, "The value of `desc.writable` is `true`, after executing `var desc = Object.getOwnPropertyDescriptor(C.prototype, 'constructor');`");
