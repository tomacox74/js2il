async function getValue() {
    return 42;
}

getValue().then((value) => {
    console.log("Got value:", value);
});
