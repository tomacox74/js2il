async function declared(value) {
    await Promise.resolve(null);
    return this.base + value;
}

const expression = async function named(value) {
    await Promise.resolve(null);
    return value * 2;
};

function makeArrow(base) {
    return async (value) => {
        await Promise.resolve(null);
        return base + value;
    };
}

const arrow = makeArrow(30);
const holder = {
    run: async function(value) {
        return value + 1;
    }
};
declared.custom = "custom";
console.log(
    "metadata",
    typeof declared,
    declared.name,
    declared.length,
    expression.name,
    expression.length,
    arrow.name,
    arrow.length);
console.log(
    "surface",
    Object.getPrototypeOf(declared) === Object.getPrototypeOf(expression),
    Object.hasOwn(declared, "prototype"),
    Object.hasOwn(expression, "prototype"),
    Object.hasOwn(arrow, "prototype"),
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
const dynamic = [expression][0](4);
const lexical = arrow.call({ ignored: true }, 5);
const objectMember = holder.run(9);

async function parameterReject(value = missingParameterBinding) {
    return value;
}
const arrowParameterReject = async (value = missingArrowBinding) => value;
const primitiveParameterReject = async function(
    value = (() => {
        throw 42;
    })()) {
    return value;
};
let parameterSynchronousThrow = false;
let parameterRejection;
let arrowParameterSynchronousThrow = false;
let arrowParameterRejection;
try {
    parameterRejection = parameterReject();
} catch (error) {
    parameterSynchronousThrow = true;
}
try {
    arrowParameterRejection = arrowParameterReject();
} catch (error) {
    arrowParameterSynchronousThrow = true;
}
console.log(
    "parameterSetup",
    parameterSynchronousThrow,
    parameterRejection instanceof Promise,
    arrowParameterSynchronousThrow,
    arrowParameterRejection instanceof Promise);
const parameterOutcome = parameterRejection.catch((error) => {
    return error instanceof ReferenceError;
});
const arrowParameterOutcome = arrowParameterRejection.catch((error) => {
    return error instanceof ReferenceError;
});
const primitiveParameterOutcome = primitiveParameterReject.call(null)
    .catch((reason) => reason === 42);

Promise.all([
    first,
    second,
    dynamic,
    lexical,
    objectMember,
    parameterOutcome,
    arrowParameterOutcome,
    primitiveParameterOutcome
]).then((values) => {
    console.log("results", values.join(","));
});

async function rejects() {
    throw new Error("async failure");
}

let synchronousThrow = false;
let rejection;
try {
    rejection = rejects();
} catch (error) {
    synchronousThrow = true;
}
console.log("syncThrow", synchronousThrow, rejection instanceof Promise);
rejection.catch((error) => {
    console.log("rejection", error.message);
});

function makeAsync() {
    return async function() {
        return 1;
    };
}
console.log("identity", makeAsync() !== makeAsync());
