class B {
  constructor(x) {
    this.x = x;
  }
}

class D extends B {
  constructor() {
    super(5);
  }
}

console.log(new D().x);
