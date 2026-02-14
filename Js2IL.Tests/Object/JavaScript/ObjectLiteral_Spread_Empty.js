"use strict";

// Spreading an empty object should produce an empty object.

const empty = {};
const spread = { ...empty };

console.log(Object.getOwnPropertyNames(spread).length);
