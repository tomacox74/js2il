"use strict";

function append(useArray, mode) {
    const receiver = useArray ? [] : {
        push(value) {
            return "object:" + value;
        }
    };

    if (mode === "own") {
        Object.defineProperty(receiver, "push", {
            configurable: true,
            get() {
                return function (value) {
                    return "own:" + value;
                };
            }
        });
    } else if (mode === "custom") {
        Object.setPrototypeOf(receiver, {
            push(value) {
                return "custom:" + value;
            }
        });
    }

    let last;
    for (let index = 0; index < 3; index++) {
        last = receiver.push(index);
    }
    return last;
}

console.log(append(true, "default"));
console.log(append(true, "own"));
console.log(append(true, "custom"));
console.log(append(false, "default"));

const originalPush = Array.prototype.push;

function mutateArrayPrototype() {
    Array.prototype.push = function (value) {
        return "prototype:" + value;
    };
}

function appendWithUnknownMutation(useArray, shouldMutate) {
    const receiver = useArray ? [] : {
        push(value) {
            return "control:" + value;
        }
    };
    let last;
    for (let index = 0; index < 3; index++) {
        if (shouldMutate && index === 1) {
            mutateArrayPrototype();
        }
        last = receiver.push(index);
    }
    return last;
}

appendWithUnknownMutation(false, false);
console.log(appendWithUnknownMutation(true, true));
console.log(append(true, "default"));

Object.defineProperty(Array.prototype, "push", {
    configurable: true,
    writable: true,
    value: originalPush
});
