const console = {
    log(value) {
        globalThis.console.log("shadowed", value);
    }
};

console.log("local");
