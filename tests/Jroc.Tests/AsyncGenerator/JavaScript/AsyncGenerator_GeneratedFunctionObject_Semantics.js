async function* declared(value) {
    yield this.base + value;
    await Promise.resolve(null);
    yield this.base + value + 1;
}

const expression = async function* named(value) {
    yield value * 2;
};

function makeGenerator() {
    return async function*() {
        yield 1;
    };
}

const object = {
    base: 30,
    async *run(value) {
        yield this.base + value;
    }
};

class AsyncGeneratorClass {
    constructor(base) {
        this.base = base;
    }

    async *run(value) {
        yield this.base + value;
    }

    callRun(value) {
        return this.run(value);
    }

    static async *twice(value) {
        yield value * 2;
    }
}

class DerivedAsyncGeneratorClass extends AsyncGeneratorClass {
    callSuper(value) {
        return super.run(value);
    }
}

async function run() {
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
        (await first.next()).value,
        (await second.next()).value,
        (await first.next()).value,
        (await second.next()).value);

    const dynamic = [expression][0](4);
    console.log("dynamic", (await dynamic.next()).value);
    console.log(
        "methods",
        (await object.run(3).next()).value,
        (await new AsyncGeneratorClass(40).run(4).next()).value,
        (await AsyncGeneratorClass.twice(5).next()).value,
        Object.hasOwn(object.run, "prototype"),
        Object.hasOwn(AsyncGeneratorClass.prototype.run, "prototype"),
        Object.hasOwn(AsyncGeneratorClass.twice, "prototype"));
    console.log(
        "asyncPrototype",
        typeof declared.prototype.next,
        typeof declared.prototype.return,
        typeof declared.prototype.throw,
        Object.prototype.toString.call(declared()));

    const customMethodPrototype = {};
    AsyncGeneratorClass.prototype.run.prototype =
        customMethodPrototype;
    const generatorInstance = new AsyncGeneratorClass(50);
    const derivedGeneratorInstance =
        new DerivedAsyncGeneratorClass(60);
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

    async function* parameterThrows(
        value = missingAsyncGeneratorParameter) {
        yield value;
    }

    let parameterSynchronousThrow = false;
    try {
        parameterThrows();
    } catch (error) {
        parameterSynchronousThrow = error instanceof ReferenceError;
    }
    console.log("parameterSetup", parameterSynchronousThrow);
    console.log("identity", makeGenerator() !== makeGenerator());
}

run();
