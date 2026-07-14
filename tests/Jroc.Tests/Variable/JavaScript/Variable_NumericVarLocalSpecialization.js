function calculate(limit) {
    var total = 0;
    for (var i = 0; i < limit; i++) {
        total += i;
    }

    var direction;
    if (total >= 0) {
        direction = 1;
    } else {
        direction = -1;
    }

    return total + direction;
}

function readBeforeInitialization() {
    console.log(typeof value);
    console.log(value === undefined);
    var value = 2;
    return value;
}

function conditionallyInitialized(flag) {
    var value;
    if (flag) {
        value = 3;
    }
    return typeof value;
}

function mixedType(flag) {
    var value = 1;
    if (flag) {
        value = "text";
    }
    return value;
}

function captured() {
    var value = 1;
    return function () {
        return ++value;
    };
}

function hoistedDependency() {
    var result = value;
    var value = 1;
    return result;
}

function objectLiteralWrite() {
    var value = 1;
    var object = { property: (value = "text") };
    return value + object.property;
}

function labeledBreak(flag) {
    label: {
        if (flag) {
            break label;
        }
        value = 1;
    }
    return value;
    var value;
}

function optionalWrite() {
    var callback = null;
    var value;
    callback?.(value = 1);
    return value;
}

function shadowedSource() {
    var source = 1;
    var target;
    {
        let source = "text";
        target = source;
    }
    return target;
}

console.log(calculate(5));
console.log(readBeforeInitialization());
console.log(conditionallyInitialized(false));
console.log(mixedType(false));
console.log(mixedType(true));
var increment = captured();
console.log(increment());
console.log(increment());
console.log(hoistedDependency());
console.log(objectLiteralWrite());
console.log(labeledBreak(true));
console.log(labeledBreak(false));
console.log(optionalWrite());
console.log(shadowedSource());
