const originalConsole = console;
globalThis.console = {
    log(value) {
        originalConsole.log("replaced", value);
    }
};

console.log("global");
