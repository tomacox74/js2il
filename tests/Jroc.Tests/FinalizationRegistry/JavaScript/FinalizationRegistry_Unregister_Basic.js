"use strict";

const hits = [];
const registry = new FinalizationRegistry((heldValue) => {
    hits.push(heldValue);
});
const token = {};

(function () {
    const target = { name: "alpha" };
    registry.register(target, "held", token);
})();

console.log(registry.unregister(token));

try {
    registry.unregister(123);
    console.log("bad token threw:", false);
} catch (error) {
    console.log("bad token threw:", true);
    console.log("bad token name:", error.name);
}

gc();

setImmediate(() => {
    console.log(hits.length);
});
