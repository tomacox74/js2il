"use strict";

// Correctness-focused coverage for String.prototype.repeat(count).
// This primarily validates the runtime helper (JavaScriptRuntime.String.Repeat)
// and the string member-call dispatch path.

function logRepeat(s, n) {
    try {
        // Delimit output for stable snapshots (e.g., empty string prints as "[]").
        const r = s.repeat(n);
        console.log("[" + r + "]");
    }
    catch (e) {
        console.log(e && e.name ? e.name : String(e));
    }
}

logRepeat("ab", 3);        // "ababab"
logRepeat("x", 0);         // ""
logRepeat("x", 1);         // "x"
logRepeat("x", 2.9);       // "xx" (ToIntegerOrInfinity-style truncation)
logRepeat("x", -1);        // RangeError
logRepeat("x", Infinity);  // RangeError
