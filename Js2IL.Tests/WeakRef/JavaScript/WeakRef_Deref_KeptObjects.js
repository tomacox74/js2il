"use strict";

function createWeakRef() {
    const target = { name: "alpha" };
    const ref = new WeakRef(target);
    console.log(ref.deref().name);
    gc();
    console.log(ref.deref().name);
    return ref;
}

const ref = createWeakRef();
console.log(Object.prototype.toString.call(ref));

try {
    new WeakRef(123);
    console.log("primitive target threw:", false);
} catch (error) {
    console.log("primitive target threw:", true);
    console.log("primitive target name:", error.name);
}

setImmediate(() => {
    gc();
    console.log(ref.deref() == null);
});
