const callbacks = [];

for (const value of [1, 2, 3]) {
    callbacks.push(() => value);
}

await Promise.resolve();

for (const callback of callbacks) {
    console.log(callback());
}
