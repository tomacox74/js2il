class Base {
    constructor(value) {
        this.value = value;
    }
}

class Derived extends Base {
    constructor(value) {
        try {
            super(value);
        } catch (error) {
            throw error;
        }
    }
}

console.log(new Derived(7).value);
