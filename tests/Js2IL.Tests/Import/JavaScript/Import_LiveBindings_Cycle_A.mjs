"use strict";

import { b } from "./Import_LiveBindings_Cycle_B.mjs";

export let a = 1;

export function incA() {
    a = a + 1;
}

export function getBFromA() {
    return b;
}
