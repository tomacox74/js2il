class Derived extends Array {
    constructor(length) {
        if (length === undefined) {
            super();
            return;
        }

        super(length);
    }
}

console.log(new Derived().length);
console.log(new Derived(3).length);
