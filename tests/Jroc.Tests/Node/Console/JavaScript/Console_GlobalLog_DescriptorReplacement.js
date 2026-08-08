const originalConsole = console;
Object.defineProperty(globalThis, "console", {
    value: {
        log(value) {
            originalConsole.log("descriptor", value);
        }
    },
    configurable: true,
    writable: true
});

console.log("global");
