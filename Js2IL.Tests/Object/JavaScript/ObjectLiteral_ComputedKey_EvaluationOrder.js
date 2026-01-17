// Verify evaluation order for computed keys and spread members.
// Spec: members evaluate left-to-right; computed key evaluates key then value.

let log = '';
function f() { log += 'f'; return 'k'; }
function g() { log += 'g'; return 1; }
function h() { log += 'h'; return { a: 2 }; }

const o = { [f()]: g(), ...h() };

console.log(log);
console.log(o.k);
console.log(o.a);
