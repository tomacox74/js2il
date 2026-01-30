"use strict";\r\n\r\nclass BeanCounter {
    constructor() {
        this.beanCounts = new Int32Array(3);
    }

    setBeanCount(box, count) {
        this.beanCounts[box] = count;
    }
}

const bc = new BeanCounter();
bc.setBeanCount(1, 42);
console.log(bc.beanCounts[1]);
