class Fetcher {
    static async getData() {
        return await Promise.resolve({ value: 42 });
    }
}

Fetcher.getData().then((data) => {
    console.log("Value:", data.value);
});
