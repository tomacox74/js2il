"use strict";

export let x = 1;
export function inc() {
    x = x + 1;
}

export default function getX() {
    return x;
}
