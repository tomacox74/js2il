// Mirrors Variable_NumericVarLocalSpecialization for `let` declarations.
// Uninitialized `let` bindings that are provably numeric before every read
// must specialize to unboxed numeric locals without changing observable
// semantics (temporal dead zone, undefined reads, type transitions).

function calculate(limit) {
    let total = 0;
    for (let i = 0; i < limit; i++) {
        total += i;
    }

    let direction;
    if (total >= 0) {
        direction = 1;
    } else {
        direction = -1;
    }

    return total + direction;
}

function drawLineShape(count) {
    // Shape of the dromaeo 3d-cube inner loop: several uninitialized `let`
    // numerics assigned on every path before the hot loop reads them.
    let IncX1, IncY1;
    let Den;
    let Num;
    let NumAdd;
    let NumPix;

    if (count >= 0) { IncX1 = 1; IncY1 = 1; }
    else { IncX1 = -1; IncY1 = -1; }

    Den = 7;
    Num = 0;
    NumAdd = 3;
    NumPix = count;

    let acc = 0;
    for (let i = 0; i < NumPix; i++) {
        Num += NumAdd;
        if (Num >= Den) {
            Num -= Den;
            acc += IncX1;
        }
        acc += IncY1;
    }
    return acc;
}

function temporalDeadZoneRead() {
    try {
        return value;
    } catch (error) {
        return error.constructor.name;
    }
    // eslint-disable-next-line no-unreachable
    let value = 1;
}

function conditionallyInitialized(flag) {
    let value;
    if (flag) {
        value = 3;
    }
    return typeof value;
}

function mixedType(flag) {
    let value;
    value = 1;
    if (flag) {
        value = "text";
    }
    return value;
}

function captured() {
    let value;
    value = 1;
    return function () {
        return ++value;
    };
}

function shadowedSource() {
    let source = 1;
    let target;
    {
        let inner = "text";
        target = inner;
    }
    return target + source;
}

function loopCarried(limit) {
    let previous;
    let current;
    previous = 0;
    current = 1;
    for (let i = 0; i < limit; i++) {
        const next = previous + current;
        previous = current;
        current = next;
    }
    return current;
}

console.log(calculate(5));
console.log(drawLineShape(10));
console.log(temporalDeadZoneRead());
console.log(conditionallyInitialized(false));
console.log(conditionallyInitialized(true));
console.log(mixedType(false));
console.log(mixedType(true));
const increment = captured();
console.log(increment());
console.log(increment());
console.log(shadowedSource());
console.log(loopCarried(10));
