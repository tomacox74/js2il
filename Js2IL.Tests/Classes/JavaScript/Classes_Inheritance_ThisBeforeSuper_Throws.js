class B {
  constructor(x) {
    this.x = x;
  }
}

class D extends B {
  constructor() {
    try {
      this.y = 1;
      console.log("no-throw");
    } catch (e) {
      console.log(e.name);
    }

    super(5);
    console.log(this.x);
  }
}

new D();
