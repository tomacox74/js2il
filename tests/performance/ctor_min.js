"use strict";
class A {
  constructor(n) {
    this.n = n;
    this.a = n >>> 1;
    this.b = 1 + this.a;
  }
}
new A(1000000);
console.log('ok');
