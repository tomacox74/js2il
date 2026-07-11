// Phase 5 (#1433): descriptor-observing APIs and mutation fallbacks
// (delete / defineProperty / freeze / seal) for object-literal inference.
// All of these disqualify the shape, so the literals here run on the generic
// JsObject path; behavior must match Node exactly.

// getOwnPropertyDescriptor on a literal.
const descObj = { n: 42, s: "str" };
const d1 = Object.getOwnPropertyDescriptor(descObj, "n");
console.log(d1.value, d1.writable, d1.enumerable, d1.configurable);
console.log(Object.getOwnPropertyDescriptor(descObj, "missing") === undefined);

// defineProperty: non-enumerable member is hidden from keys/JSON but readable.
const defObj = { visible: 1 };
Object.defineProperty(defObj, "hidden", { value: 99, enumerable: false, writable: false, configurable: false });
console.log(defObj.hidden, Object.keys(defObj).join(","), JSON.stringify(defObj));

// delete removes the property and later reads see undefined.
const delObj = { keep: "k", drop: "d" };
console.log(delete delObj.drop, delObj.drop === undefined, Object.keys(delObj).join(","));
console.log(JSON.stringify(delObj));

// freeze: writes are ignored (non-strict), delete fails, isFrozen reports true.
const frozen = { a: 1, b: "two" };
Object.freeze(frozen);
frozen.a = 100;
frozen.c = 3;
console.log(delete frozen.a, frozen.a, frozen.c === undefined, Object.isFrozen(frozen));
console.log(JSON.stringify(frozen));

// seal: existing props writable, no adds/deletes, isSealed reports true.
const sealed = { x: 1 };
Object.seal(sealed);
sealed.x = 2;
sealed.y = 3;
console.log(delete sealed.x, sealed.x, sealed.y === undefined, Object.isSealed(sealed), Object.isFrozen(sealed));

// 'in' operator observes presence without reading values.
const inObj = { present: undefined };
console.log("present" in inObj, "absent" in inObj);
