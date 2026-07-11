// Phase 5 (#1433): enumeration order and JSON parity for object-literal inference.
// Enumeration and JSON.stringify disqualify a shape, so these literals exercise
// the fallback path; output must match Node exactly.

// for-in enumeration order (insertion order for string keys).
const forInObj = { b: 1, a: 2, c: 3, z: 0, m: 5 };
const forInKeys = [];
for (const k in forInObj) {
  forInKeys.push(k);
}
console.log(forInKeys.join(","));

// Object.keys / values / entries order.
const keysObj = { second: 2, first: 1, third: 3 };
console.log(Object.keys(keysObj).join(","));
console.log(Object.values(keysObj).join(","));
console.log(Object.entries(keysObj).map(function (e) { return e[0] + "=" + e[1]; }).join(","));

// Integer-like keys enumerate in ascending numeric order before string keys.
const mixedKeys = { b: "s1", 2: "i2", a: "s2", 0: "i0", 1: "i1" };
console.log(Object.keys(mixedKeys).join(","));
console.log(JSON.stringify(mixedKeys));

// JSON.stringify basics: insertion order, null, nested object, function elided.
const jsonObj = {
  text: "hello",
  n: 42,
  flag: true,
  boxed: null,
  nested: { x: 1, y: "two" },
  fn: function () { return 1; }
};
console.log(JSON.stringify(jsonObj));
console.log(JSON.stringify(jsonObj, null, 1));

// Writes before stringification are observed.
const mutated = { a: 1, b: 2 };
mutated.a = 10;
mutated.b += 5;
console.log(JSON.stringify(mutated));

// A literal that is only read stays eligible; verify its values agree with an
// enumerated clone of the same shape (parity between the two paths).
const eligibleTwin = { p: 1, q: "two", r: true };
const enumeratedTwin = { p: 1, q: "two", r: true };
console.log(eligibleTwin.p, eligibleTwin.q, eligibleTwin.r);
console.log(Object.keys(enumeratedTwin).join(","), enumeratedTwin.p, enumeratedTwin.q, enumeratedTwin.r);
