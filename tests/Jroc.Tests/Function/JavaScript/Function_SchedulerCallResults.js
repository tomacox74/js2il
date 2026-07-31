"use strict";

function math(value) {
    return Math.floor(value) + Math.sqrt(value);
}

function nestedCall(value) {
    return Math.abs(Math.floor(value));
}

function compareCall(left, right) {
    return Math.floor(left) < Math.ceil(right);
}

function argumentCall(value) {
    return Math.max(Math.floor(value), Math.sqrt(value));
}

function argumentDependency(left, right, exponent) {
    return Math.max(Math.pow(left + right, exponent), 0);
}

let order = "";

function g() {
    order += "g";
    return 1;
}

function h() {
    order += "h";
    return 2;
}

function ordered() {
    order = "";
    const result = g() + h();
    return result + "," + order;
}

function throwing() {
    order = "";
    try {
        return g() + (() => {
            order += "t";
            throw new Error("x");
        })();
    } catch (error) {
        return order;
    }
}

function multiUse() {
    order = "";
    const value = g();
    return (value + value) + "," + order;
}

class Counter {
    constructor(value) {
        this.value = value;
    }

    add(value) {
        this.value += value;
        return this;
    }

    getValue() {
        return this.value;
    }

    chain() {
        return this.add(2).add(3).getValue();
    }

    addOnce() {
        return this.add(2);
    }

    next() {
        this.value += 1;
        return this.value;
    }

    maxNext() {
        return Math.max(this.next(), 0);
    }

    sum(a, b, c, d, e, f, g, h, i) {
        return a + b + c + d + e + f + g + h + i;
    }

    maxSum() {
        return Math.max(this.sum(1, 2, 3, 4, 5, 6, 7, 8, 9), 0);
    }

    maxSumWithFloor() {
        return Math.floor(1.5) + Math.max(this.sum(1, 2, 3, 4, 5, 6, 7, 8, 9), 0);
    }
}

function fluentTypedCalls() {
    return new Counter(1).chain();
}

function singleTypedCall() {
    return new Counter(1).addOnce().getValue();
}

function scheduledInlineCall() {
    const counter = new Counter(1);
    return counter.maxNext() + "," + counter.value;
}

function deepScheduledInlineCall() {
    return new Counter(0).maxSum();
}

function deepScheduledInlineCallWithCarriedResult() {
    return new Counter(0).maxSumWithFloor();
}

console.log(math(9));
console.log(nestedCall(-2.7));
console.log(compareCall(1.2, 1.1));
console.log(argumentCall(9));
console.log(argumentDependency(2, 3, 2));
console.log(ordered());
console.log(throwing());
console.log(multiUse());
console.log(fluentTypedCalls());
console.log(singleTypedCall());
console.log(scheduledInlineCall());
console.log(deepScheduledInlineCall());
console.log(deepScheduledInlineCallWithCarriedResult());
