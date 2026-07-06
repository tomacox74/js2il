"use strict";

const err = new TypeError("boom");

assert.sameValue(globalThis.TypeError, TypeError);
assert(err instanceof TypeError);
assert(err instanceof Error);
