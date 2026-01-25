class B {
  m() {
    return 3;
  }
}

class D extends B {
  m() {
    return super.m() + 2;
  }
}

console.log(new D().m());
