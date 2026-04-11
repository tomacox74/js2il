"use strict";

import { a } from "./Import_LiveBindings_Cycle_A.mjs";

export let b = 10;

export function incB() {
    b = b + 1;
}

export function getAFromB() {
    return a;
}
