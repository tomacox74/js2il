function* declared(value) {
    yield this.base + value;
    yield this.base + value + 1;
}

const expression = function* named(value) {
    yield value * 2;
};

function makeGenerator() {
    return function*() {
        yield 1;
    };
}

declared.custom = "custom";
console.log(
    "metadata",
    typeof declared,
    declared.name,
    declared.length,
    expression.name,
    expression.length);
console.log(
    "surface",
    Object.getPrototypeOf(declared) === Object.getPrototypeOf(expression),
    Object.hasOwn(declared, "prototype"),
    Object.getPrototypeOf(declared.call({ base: 0 }, 0)) === declared.prototype,
    declared.custom);

let constructThrows = false;
try {
    Reflect.construct(declared, []);
} catch (error) {
    constructThrows = error instanceof TypeError;
}
console.log("nonconstructor", constructThrows);

const first = declared.call({ base: 10 }, 1);
const second = declared.call({ base: 20 }, 2);
console.log(
    "interleaved",
    first.next().value,
    second.next().value,
    first.next().value,
    second.next().value);

const dynamic = [expression][0](4);
console.log("dynamic", dynamic.next().value);

function makeSharedGenerator() {
    let shared = 0;
    return function*() {
        let local = 0;
        shared++;
        local++;
        yield shared + ":" + local;
        shared++;
        local++;
        yield shared + ":" + local;
    };
}

const sharedGenerator = makeSharedGenerator();
const sharedFirst = sharedGenerator();
const sharedSecond = sharedGenerator();
console.log(
    "state",
    sharedFirst.next().value,
    sharedSecond.next().value,
    sharedFirst.next().value,
    sharedSecond.next().value);

const object = {
    base: 30,
    *run(value) {
        yield this.base + value;
    }
};

class GeneratorClass {
    constructor(base) {
        this.base = base;
    }

    *run(value) {
        yield this.base + value;
    }

    callRun(value) {
        return this.run(value);
    }

    static *twice(value) {
        yield value * 2;
    }
}

class DerivedGeneratorClass extends GeneratorClass {
    callSuper(value) {
        return super.run(value);
    }
}

console.log(
    "methods",
    object.run(3).next().value,
    new GeneratorClass(40).run(4).next().value,
    GeneratorClass.twice(5).next().value,
    Object.hasOwn(object.run, "prototype"),
    Object.hasOwn(GeneratorClass.prototype.run, "prototype"),
    Object.hasOwn(GeneratorClass.twice, "prototype"));

const customMethodPrototype = {};
GeneratorClass.prototype.run.prototype = customMethodPrototype;
const generatorInstance = new GeneratorClass(50);
const derivedGeneratorInstance = new DerivedGeneratorClass(60);
console.log(
    "internalMethods",
    Object.getPrototypeOf(generatorInstance.callRun(6))
        === customMethodPrototype,
    Object.getPrototypeOf(derivedGeneratorInstance.callSuper(7))
        === customMethodPrototype);

const customExpressionPrototype = {};
expression.prototype = customExpressionPrototype;
function tailCallGenerator() {
    return expression(6);
}
console.log(
    "tail",
    Object.getPrototypeOf(tailCallGenerator())
        === customExpressionPrototype);

function* parameterThrows(value = missingGeneratorParameter) {
    yield value;
}

let parameterSynchronousThrow = false;
let parameterIterationThrow = false;
try {
    parameterThrows().next();
} catch (error) {
    parameterIterationThrow = error instanceof ReferenceError;
}
console.log(
    "parameterSetup",
    parameterSynchronousThrow,
    parameterIterationThrow);
console.log("identity", makeGenerator() !== makeGenerator());
