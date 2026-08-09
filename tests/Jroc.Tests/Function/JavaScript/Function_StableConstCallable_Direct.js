const offset = 10;

const addOne = value => value + 1;
const addOffset = value => value + offset;
const withDefault = function (value = 4) {
    return value * 2;
};
const named = function namedAdd(value) {
    return value + 3;
};
const withRest = (...values) => values[0] + values[1] + values.length;
const expressionWithRest = function (first, ...remaining) {
    return first + remaining[0] + remaining.length;
};
const readsNewTarget = function () {
    return new.target === undefined;
};
const recurse = value => value === 0 ? 0 : value + recurse(value - 1);

const nestedOrdinaryFunction = () => {
    function readsOwnThis() {
        return this.value;
    }

    return typeof readsOwnThis;
};

const nestedOrdinaryClass = function () {
    class ReadsOwnThis {
        read() {
            return this.value;
        }
    }

    return typeof ReadsOwnThis;
};

const identity = addOne;
console.log(
    "direct",
    addOne(2),
    addOffset(5),
    withDefault(),
    named(6),
    withRest(1, 2),
    expressionWithRest(2, 3),
    readsNewTarget(),
    recurse(4));
console.log("nested", nestedOrdinaryFunction(), nestedOrdinaryClass());
console.log("identity", addOne === identity, addOne === addOne, addOne.name, named.name);
