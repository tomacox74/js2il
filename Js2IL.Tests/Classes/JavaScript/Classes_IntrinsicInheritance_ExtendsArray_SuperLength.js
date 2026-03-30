"use strict";

class NodeList extends Array {
  constructor(a) {
    super((a && a.length) || 0);
    this[0] = "x";
  }
}

let a = [1, 2, 3];
let nl = new NodeList(a);
console.log(nl.length);
console.log(nl[0]);
console.log(nl[2]);

class Y extends Array {
  constructor() {
    super(1, 2, 3);
  }
}

let y = new Y();
console.log(y.length);
console.log(y[1]);
console.log(y[2]);
