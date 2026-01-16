const obj = {
    prefix: "Result: ",
    asyncFormat: async function (valuePromise) {
        const value = await valuePromise;
        return this.prefix + value;
    }
};

(async function () {
    console.log(await obj.asyncFormat(Promise.resolve(42)));
})();
