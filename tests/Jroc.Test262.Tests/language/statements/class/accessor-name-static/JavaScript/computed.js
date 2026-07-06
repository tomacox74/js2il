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
  static get [_ = 'str' + 'ing']() { return 'get string'; }
  static set [_ = 'str' + 'ing'](param) { stringSet = param; }
}

assert.sameValue(C['string'], 'get string');

C['string'] = 'set string';
assert.sameValue(stringSet, 'set string');
