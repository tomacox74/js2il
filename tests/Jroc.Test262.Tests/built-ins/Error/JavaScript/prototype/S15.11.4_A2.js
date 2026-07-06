Error.prototype.toString = Object.prototype.toString;
var __tostr = Error.prototype.toString();

assert.sameValue(__tostr, "[object Object]", 'The value of __tostr is expected to be "[object Object]"');

