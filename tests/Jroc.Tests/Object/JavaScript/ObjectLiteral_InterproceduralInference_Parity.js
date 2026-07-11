// Direct propagation: the literal's specialized shape flows into the parameter `o`.
const a = { b: "hello" };
function c(o) {
  console.log(o.b);
}
c(a);

// Multi-hop propagation across a chain longer than the old 4-iteration cap.
const p = { v: 7 };
function h6(o) { console.log(o.v); }
function h5(o) { h6(o); }
function h4(o) { h5(o); }
function h3(o) { h4(o); }
function h2(o) { h3(o); }
function h1(o) { h2(o); }
h1(p);

// Two structurally identical literals share one generated type and join at `readX`.
const s1 = { x: 1 };
const s2 = { x: 2 };
function readX(o) {
  console.log(o.x);
}
readX(s1);
readX(s2);

// Incompatible call sites keep the parameter generic but still produce correct output.
const d1 = { m: "A" };
const d2 = { z: "B" };
function readM(o) {
  console.log(o.m);
}
readM(d1);
readM(d2);

// Unsafe callee use (unknown call) keeps the literal generic but still runs correctly.
const e = { q: 9 };
function keep(o) {
  console.log(Object.keys(o).join(","));
}
keep(e);

// Reordered same-shape literals canonicalize to one generated type and join at `readXY`.
const r1 = { x: 1, y: "left" };
const r2 = { y: "right", x: 2 };
function readXY(o) {
  console.log(o.x + ":" + o.y);
}
readXY(r1);
readXY(r2);

// Safe parameter member write/read: same-type write and update lower through the generated setter.
const w = { n: 1 };
function bump(o) {
  o.n = 5;
  o.n++;
  console.log(o.n);
}
bump(w);

// A spread before a positional literal shifts its parameter slot: the analysis falls back to the
// generic path, but the program still produces the correct result.
const sh = { b: "shifted" };
const restArgs = [10, 20];
function shift(x, y, o) {
  console.log(x + "," + y + "," + o.b);
}
shift(...restArgs, sh);
