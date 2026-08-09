globalThis.stableConstMarker = "global";

const ordinaryThis = function () {
    return this.stableConstMarker;
};

const strictThis = function () {
    "use strict";
    return this === undefined;
};

const withArguments = function (first) {
    return arguments.length + ":" + first + ":" + arguments[1];
};

const spreadTarget = (left, right) => left + right;

const asyncArrow = async value => value + 1;
const asyncExpression = async function (value) {
    return value + 2;
};
const generatorExpression = function* () {
    yield 7;
};

const receiver = {
    value: 9,
    read: function () {
        const lexicalThis = () => this.value;
        return lexicalThis();
    }
};

function readLexicalArguments(value) {
    const lexicalArguments = () => arguments[0];
    return lexicalArguments();
}

function ReadNewTarget() {
    const lexicalNewTarget = () => new.target === ReadNewTarget;
    return lexicalNewTarget();
}

const nestedArrowThis = function () {
    const read = () => this.stableConstMarker;
    return read();
};

const namedRecursion = function recurse(value) {
    return value === 0 ? 0 : value + recurse(value - 1);
};

const namedIdentity = function ownIdentity() {
    return ownIdentity === namedIdentity;
};

class StableConstBase {
    read() {
        return 11;
    }
}

class StableConstDerived extends StableConstBase {
    read() {
        const lexicalSuper = () => super.read();
        return lexicalSuper();
    }
}

console.log("receiver", ordinaryThis(), strictThis(), receiver.read());
console.log("context", withArguments("a", "b"), readLexicalArguments("arg"), ReadNewTarget(), new ReadNewTarget());
console.log("spread", spreadTarget(...[3, 4]), nestedArrowThis(), new StableConstDerived().read());
console.log("named", namedRecursion(4), namedIdentity());
console.log("generator", generatorExpression().next().value);

Promise.all([asyncArrow(5), asyncExpression(5)]).then(values => {
    console.log("async", values.join(","));
});
