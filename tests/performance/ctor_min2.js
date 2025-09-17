"use strict";
class A {
  constructor(n) {
    let words = n >>> 1;
    let cap = 1 + words;
    this.a = words;
    this.b = cap;
    this.arr = new Int32Array(cap);
  }
}
new A(1000000);
console.log('ok');
