"use strict";

import { a, incA, getBFromA } from "./Import_LiveBindings_Cycle_A.mjs";
import { b, incB, getAFromB } from "./Import_LiveBindings_Cycle_B.mjs";

console.log("a0:", a);
console.log("b0:", b);
console.log("a->b:", getBFromA());
console.log("b->a:", getAFromB());

incA();
incB();

console.log("a1:", a);
console.log("b1:", b);
console.log("a->b1:", getBFromA());
console.log("b->a1:", getAFromB());
