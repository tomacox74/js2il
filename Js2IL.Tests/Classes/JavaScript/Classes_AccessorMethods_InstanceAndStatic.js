"use strict";

class Counter {
  static _total = 3;

  constructor(initial) {
    this._count = initial;
  }

  get count() {
    return this._count;
  }

  set count(value) {
    this._count = value + 1;
  }

  get label() {
    return "count=" + this.count;
  }

  static get total() {
    return Counter._total;
  }

  static set total(value) {
    Counter._total = value * 2;
  }
}

const counter = new Counter(4);
console.log(counter.count);
counter.count = 10;
console.log(counter.count);
console.log(counter.label);
console.log(Counter.total);
Counter.total = 5;
console.log(Counter.total);
