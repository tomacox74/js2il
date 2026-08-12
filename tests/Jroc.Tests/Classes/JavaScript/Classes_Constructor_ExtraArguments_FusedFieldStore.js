class Child {
    constructor(value) {
        this.value = value;
    }
}

class Parent {
    constructor() {
        this.child = new Child(1, 2);
    }
}

console.log(new Parent().child.value);
