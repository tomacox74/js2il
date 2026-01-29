class C {
    constructor() {
        this.x = 48;
    }

    make() {
        const someObject = {
            x: 999,
            getValue: () => this.x,
        };

        return someObject;
    }
}

const c = new C();
const someObject = c.make();

console.log(someObject.getValue());
