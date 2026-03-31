"use strict";

function makeIterable(limit) {
    let closed = 0;
    return {
        getClosed() {
            return closed;
        },
        iterable: {
            [Symbol.iterator]() {
                let value = 0;
                return {
                    next() {
                        value++;
                        return { value, done: value > limit };
                    },
                    return() {
                        closed++;
                        return { done: true };
                    }
                };
            }
        }
    };
}

const mapState = makeIterable(2);
const mapHelper = Iterator.from(mapState.iterable).map(value => value);
mapHelper.next();
mapHelper.next();
console.log(mapHelper.next().done);
console.log(mapState.getClosed());

const abruptMapState = makeIterable(2);
const abruptMapHelper = Iterator.from(abruptMapState.iterable).map(value => {
    throw new Error("boom");
});
try {
    abruptMapHelper.next();
} catch (error) {
}
console.log(abruptMapState.getClosed());

const abruptFilterState = makeIterable(2);
const abruptFilterHelper = Iterator.from(abruptFilterState.iterable).filter(value => {
    throw new Error("boom");
});
try {
    abruptFilterHelper.next();
} catch (error) {
}
console.log(abruptFilterState.getClosed());

const takeState = makeIterable(3);
const takeHelper = Iterator.from(takeState.iterable).take(0);
console.log(takeHelper.next().done);
console.log(takeState.getClosed());

let flatInnerClosed = 0;
const flatHelper = Iterator.from([1]).flatMap(value => ({
    [Symbol.iterator]() {
        let done = false;
        return {
            next() {
                if (done) {
                    return { done: true };
                }

                done = true;
                return { value, done: false };
            },
            return() {
                flatInnerClosed++;
                return { done: true };
            }
        };
    }
}));
flatHelper.next();
console.log(flatHelper.next().done);
console.log(flatInnerClosed);

const flatSourceState = makeIterable(2);
const flatSourceHelper = Iterator.from(flatSourceState.iterable).flatMap(value => {
    throw new Error("boom");
});
try {
    flatSourceHelper.next();
} catch (error) {
}
console.log(flatSourceState.getClosed());

let flatThrowOuterClosed = 0;
let flatThrowInnerClosed = 0;
const flatThrowHelper = Iterator.from({
    [Symbol.iterator]() {
        let emitted = false;
        return {
            next() {
                if (emitted) {
                    return { done: true };
                }

                emitted = true;
                return { value: 1, done: false };
            },
            return() {
                flatThrowOuterClosed++;
                return { done: true };
            }
        };
    }
}).flatMap(value => ({
    [Symbol.iterator]() {
        return {
            next() {
                throw new Error("innerBoom");
            },
            return() {
                flatThrowInnerClosed++;
                return { done: true };
            }
        };
    }
}));
try {
    flatThrowHelper.next();
    flatThrowHelper.next();
} catch (error) {
}
console.log(flatThrowOuterClosed);
console.log(flatThrowInnerClosed);
