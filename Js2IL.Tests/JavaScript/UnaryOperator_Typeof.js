// typeof on various values
const x = 1;
const y = "s";
const z = true;
const n = null;
function f(){}
const o = { a: 1 };

console.log(
  typeof x,
  typeof y,
  typeof z,
  typeof n, // JS: 'object'
  typeof f,
  typeof o
);
