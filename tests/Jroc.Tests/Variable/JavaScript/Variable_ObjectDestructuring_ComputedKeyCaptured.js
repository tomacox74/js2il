function createReader(key, source) {
    return () => {
        const { [key]: value } = source;
        return value;
    };
}

console.log(createReader("answer", { answer: 42 })());
