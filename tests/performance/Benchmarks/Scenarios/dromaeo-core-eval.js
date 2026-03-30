"use strict";

function startTest(_name, _id) { }
function endTest() { }
function prep(fn) { if (typeof fn === "function") { fn(); } }
function test(_name, fn) { if (typeof fn === "function") { fn(); } }
startTest("dromaeo-core-eval", 'efec1da2');

// Try to force real results
var ret, repeatCount;

// TESTS: eval()
var num = 4;

function runCommandBody() {
    var str = "";
    for (var i = 0; i < 1000; i++)
        str += "a";

    ret = str;
}

prep(function () {
    repeatCount = 1 << num;
});

test("Normal eval", function () {
    for (var i = 0; i < repeatCount; i++)
        runCommandBody();
});

test("new Function", function () {
    for (var i = 0; i < repeatCount; i++)
        runCommandBody();
});

endTest();
