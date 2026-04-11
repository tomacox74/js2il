"use strict";

// Minimal repro: `Error.prototype` access at module init time.
// This specifically covers the IR lowering path that previously failed with:
//   HIR->LIR: failed lowering PropertyAccessExpression (property='prototype')
var p = Error.prototype;
module.exports = p ? 1 : 0;
