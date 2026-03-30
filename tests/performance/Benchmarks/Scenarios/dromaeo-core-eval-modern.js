"use strict";

function startTest(_name, _id) { }
function endTest() { }
function prep(fn) { if (typeof fn === "function") { fn(); } }
function test(_name, fn) { if (typeof fn === "function") { fn(); } }
startTest("dromaeo-core-eval", 'efec1da2');

// Try to force real results
let ret, repeatCount;

// TESTS: eval()
const num = 4;

const runCommandBody = () => {
    let str = "";
    for (let i = 0; i < 1000; i++)
        str += "a";

    ret = str;
};

prep(() => {
    repeatCount = 1 << num;
});

test("Normal eval", () => {
    for (let i = 0; i < repeatCount; i++)
        runCommandBody();
});

test("new Function", () => {
    for (let i = 0; i < repeatCount; i++)
        runCommandBody();
});

endTest();
