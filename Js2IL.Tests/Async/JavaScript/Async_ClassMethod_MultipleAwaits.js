"use strict";

class Processor {
    async process(items) {
        const first = await Promise.resolve(items[0]);
        const second = await Promise.resolve(items[1]);
        return first + second;
    }
}

const processor = new Processor();
processor.process([10, 32]).then((result) => {
    console.log("Sum:", result);
});
