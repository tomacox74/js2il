var literal = {
    id: (function() {}),
    seq: (0, function() {}),
    arrow: () => {},
    klass: class {
        static initialName = this.name;
    }
};
console.log(JSON.stringify([
    literal.id.name,
    literal.seq.name,
    literal.arrow.name,
    literal.klass.name,
    literal.klass.initialName
]));

var anonymous = (0, function() {});
var reused = { value: anonymous };
var spread = { ...reused };
console.log(JSON.stringify([anonymous.name, spread.value.name]));

var prototypeLiteral = { __proto__: class {} };
console.log(Object.getPrototypeOf(prototypeLiteral).name === "");
