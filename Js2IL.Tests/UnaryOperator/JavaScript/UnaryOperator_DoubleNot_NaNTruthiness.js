class C {
    returnNaN() {
        return 0.0 / 0.0;
    }

    returnNumber() {
        return 1.0;
    }

    constructor() {
        console.log(!!this.returnNaN());
        console.log(!!this.returnNumber());
    }
}

new C();
