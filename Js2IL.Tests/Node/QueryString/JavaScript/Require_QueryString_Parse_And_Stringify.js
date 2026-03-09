"use strict";

const querystring = require("querystring");

const parsed = querystring.parse("name=Jane+Doe&lang=en&lang=fr&empty=&encoded=a%2Bb");
console.log("name:", parsed.name);
console.log("lang count:", parsed.lang.length);
console.log("langs:", parsed.lang[0] + "|" + parsed.lang[1]);
console.log("empty:", parsed.empty === "");
console.log("encoded:", parsed.encoded);

const serialized = querystring.stringify({
    name: "Jane Doe",
    lang: ["en", "fr"],
    empty: "",
    encoded: "a+b"
});
console.log("stringify:", serialized);

const custom = querystring.parse("left:1|right:2", "|", ":");
console.log("custom:", custom.left + "," + custom.right);
console.log("custom stringify:", querystring.stringify({ left: 1, right: 2 }, "|", ":"));

const malformed = querystring.parse("bad=%&plus=a+b%");
console.log("malformed bad:", malformed.bad);
console.log("malformed plus:", malformed.plus);
