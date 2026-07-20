class Bar {
    constructor(n) {
        this.n = n;
    }
}

class Foo {
    constructor() {
        this.bar = new Bar(5);
    }
}

new Bar(1);
new Bar(2);
const foo = new Foo();
console.log(foo.bar.n);
