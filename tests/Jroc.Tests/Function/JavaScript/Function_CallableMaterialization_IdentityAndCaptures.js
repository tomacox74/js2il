let shared = 0;

const direct = value => {
    shared += value;
    return shared;
};

const escaping = value => {
    shared += value;
    return shared;
};
const alias = escaping;
const holder = { escaping };
const values = [escaping];

console.log("direct", direct(2), direct(3), shared);
console.log(
    "identity",
    alias === escaping,
    holder.escaping === escaping,
    values[0] === escaping);
console.log("shared", alias(4), escaping(5), shared);

function makeReturned() {
    const returned = () => ++shared;
    return returned;
}

const returned = makeReturned();
console.log("returned", returned(), returned(), shared);

function invokeUnknown(callback) {
    return callback();
}

const higherOrder = () => 17;
console.log("higher-order", invokeUnknown(higherOrder));

const methods = value => value;
const bound = methods.bind(null, 18);
console.log(
    "methods",
    methods.call(null, 19),
    methods.apply(null, [20]),
    bound(),
    methods.name);

const recursive = value => value === 0 ? 0 : value + recursive(value - 1);
console.log("recursive", recursive(4));

const even = value => value === 0 ? true : odd(value - 1);
const odd = value => value === 0 ? false : even(value - 1);
console.log("mutual", even(6), odd(6));

const exported = () => 21;
exports.exported = exported;
console.log("export", exported());
