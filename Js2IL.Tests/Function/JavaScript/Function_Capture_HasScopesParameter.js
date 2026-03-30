"use strict";

// Capturing function - MUST have scopes parameter
let outerValue = 42;

function captureOuter() {
    return outerValue;
}

console.log(captureOuter());
