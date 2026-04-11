"use strict";

// Test module.exports basic functionality
// This tests that module.exports is the authoritative export value

exports.foo = "exported via exports.foo";
module.exports.bar = "exported via module.exports.bar";

// Verify exports has the property set via module.exports.bar
console.log("foo:", exports.foo);
console.log("bar:", module.exports.bar);

// Both foo and bar should be accessible on both objects when they are the same reference
console.log("exports.bar:", exports.bar);
console.log("module.exports.foo:", module.exports.foo);
