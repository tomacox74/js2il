// Phase 5 (#1433): closure capture and aliasing behavior for object-literal inference.

// Aliasing to another binding disqualifies the shape; both the original and the
// alias must observe the same object identity and values.
const aliased = { count: 1, label: "orig" };
const alias = aliased;
alias.count = 2;
aliased.label = "updated";
console.log(aliased.count, alias.label, alias === aliased);

// Passing the binding to a function (escape) disqualifies; the callee's writes
// must be visible to the caller.
const escaped = { total: 10 };
addFive(escaped);
console.log(escaped.total);

function addFive(o) {
  o.total += 5;
}

// Closure capture with reads and writes: a nested function reading and writing
// members of a captured literal binding.
const captured = { hits: 0, name: "counter" };
function bump() {
  captured.hits += 1;
  return captured.hits;
}
console.log(bump(), bump(), captured.hits, captured.name);

// Closure capture through an arrow returned from a function.
const box = { value: 5 };
const getValue = function () { return box.value; };
const setValue = function (v) { box.value = v; };
setValue(box.value * 2);
console.log(getValue());

// Conditional assignment through a closure interleaved with direct access.
let toggleState = { on: false, flips: 0 };
function flip() {
  toggleState.on = !toggleState.on;
  toggleState.flips += 1;
}
flip();
console.log(toggleState.on, toggleState.flips);
flip();
console.log(toggleState.on, toggleState.flips);
