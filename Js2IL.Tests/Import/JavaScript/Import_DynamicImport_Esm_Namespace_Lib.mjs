"use strict";

export let value = 1;

export function inc() {
    value = value + 1;
}

export default function readValue() {
    return value;
}
