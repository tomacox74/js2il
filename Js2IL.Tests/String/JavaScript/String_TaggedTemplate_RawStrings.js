"use strict";

function tag(strings, ...values) {
    console.log("cooked[0]:", strings[0]);
    console.log("raw[0]:", strings.raw[0]);
    console.log("cooked[1]:", strings[1]);
    console.log("raw[1]:", strings.raw[1]);
}

tag`line1\nline2${42}test\ttab`;
