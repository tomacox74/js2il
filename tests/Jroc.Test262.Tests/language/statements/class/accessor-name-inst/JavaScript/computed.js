var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

var _;


var stringSet;

class C {
  get [_ = 'str' + 'ing']() { return 'get string'; }
  set [_ = 'str' + 'ing'](param) { stringSet = param; }
}

assert.sameValue(C.prototype['string'], 'get string');

C.prototype['string'] = 'set string';
assert.sameValue(stringSet, 'set string');
