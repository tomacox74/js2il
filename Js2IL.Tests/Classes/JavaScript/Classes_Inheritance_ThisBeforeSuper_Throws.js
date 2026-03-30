"use strict";

class B {
  constructor(x) {
    this.x = x;
  }
}

class D extends B {
  constructor() {
    try {
      this.y = 1; // In derived constructors, `this` is not usable before calling super().
      console.log("no-throw");
    } catch (e) {
      console.log(e.name);
    }

    super(5);
    console.log(this.x);
  }
}

new D();
