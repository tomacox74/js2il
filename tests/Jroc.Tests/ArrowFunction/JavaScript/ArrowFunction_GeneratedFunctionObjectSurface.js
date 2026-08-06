"use strict";

function makeArrow() {
    return (left, right) => this.base + left + right;
}

const arrow = makeArrow.call({ base: 10 });
const sameArrow = arrow;

console.log(typeof arrow);
console.log(arrow === sameArrow);
console.log(arrow.length);
console.log(arrow.name === "");
console.log(Object.hasOwn(arrow, "prototype"));
console.log(arrow.call({ base: 100 }, 1, 2));
console.log(arrow.apply({ base: 100 }, [3, 4]));

const bound = arrow.bind({ base: 100 }, 5);
console.log(bound(6));
console.log(bound.length);
console.log(bound.name);

try {
    new arrow();
} catch (error) {
    console.log(error instanceof TypeError);
}
