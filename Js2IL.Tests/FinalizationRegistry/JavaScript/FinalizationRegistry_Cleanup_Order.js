"use strict";

const order = [];
const registry = new FinalizationRegistry((heldValue) => {
    order.push("cleanup:" + heldValue);
});

console.log(Object.prototype.toString.call(registry));

try {
    const same = {};
    registry.register(same, same);
    console.log("same target threw:", false);
} catch (error) {
    console.log("same target threw:", true);
    console.log("same target name:", error.name);
}

(function () {
    const target = { name: "alpha" };
    registry.register(target, "held");
})();

new Promise((resolve) => resolve()).then(() => order.push("promise"));
gc();

setImmediate(() => {
    order.push("immediate");
    console.log(order.join(","));
});
