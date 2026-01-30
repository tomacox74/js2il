"use strict";\r\n\r\nconst obj = {
    prefix: "Result: ",
    asyncFormat: async function (valuePromise) {
        const value = await valuePromise;
        return this.prefix + value;
    }
};

obj.asyncFormat(Promise.resolve(42)).then(function (value) {
    console.log(value);
});
