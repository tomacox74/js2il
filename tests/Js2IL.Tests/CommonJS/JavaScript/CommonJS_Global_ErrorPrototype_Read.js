"use strict";

// Issue #550 repro: CommonJS module that touches `Error.prototype`.
// This exercises the IR pipeline (module main method compilation) and previously failed
// during HIR->LIR lowering because `Error` was not available as an intrinsic global.

const hasErrorPrototype = require('./CommonJS_Global_ErrorPrototype_Read_Lib');
console.log("hasErrorPrototype:", hasErrorPrototype);
