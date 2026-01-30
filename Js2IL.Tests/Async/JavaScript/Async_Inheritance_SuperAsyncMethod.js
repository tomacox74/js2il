"use strict";\r\n\r\nclass Base {
    async inc(x) {
        return await Promise.resolve(x + 1);
    }
}

class Derived extends Base {
    async run() {
        return await super.inc(4);
    }
}

new Derived().run().then((result) => {
    console.log("Result:", result);
});
