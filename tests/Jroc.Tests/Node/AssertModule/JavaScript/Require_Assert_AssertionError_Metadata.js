"use strict";

const assert = require("node:assert");

try {
    assert.strictEqual(1, 2, "numbers differ");
} catch (error) {
    console.log("name:", error.name);
    console.log("code:", error.code);
    console.log("message:", error.message);
    console.log("actual:", error.actual);
    console.log("expected:", error.expected);
    console.log("operator:", error.operator);
    console.log("generated:", error.generatedMessage);
    console.log("instance:", error instanceof assert.AssertionError);
}

try {
    assert(false);
} catch (error) {
    console.log("default generated:", error.generatedMessage);
}
