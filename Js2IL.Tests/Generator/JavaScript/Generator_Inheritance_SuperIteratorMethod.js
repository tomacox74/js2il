"use strict";\r\n\r\nclass Base {
    *values() {
        yield 1;
        yield 2;
    }
}

class Derived extends Base {
    run() {
        const it = super.values();
        console.log(it.next().value);
        console.log(it.next().value);
    }
}

new Derived().run();
