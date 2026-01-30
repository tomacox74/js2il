"use strict";\r\n\r\nclass PrimeSieve {
  constructor(n) { this.n = n; }
}

// The bug reproduces when a class is instantiated inside an arrow function,
// then the arrow is invoked. This mirrors the perf script pattern.
const make = () => new PrimeSieve(10);
const s = make();
console.log(s.n);
