"use strict";\r\n\r\nasync function getValue() {
    return 42;
}

getValue().then((value) => {
    console.log("Got value:", value);
});
