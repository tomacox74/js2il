const makeCallbacks = () => {
    const callbacks = [];

    for (let i = 0, length = 2; i < length; i++) {
        const argPosition = i + 1;
        callbacks.push(() => argPosition);
    }

    return callbacks;
};

const callbacks = makeCallbacks();
const first = callbacks[0];
const second = callbacks[1];

console.log(first());
console.log(second());
