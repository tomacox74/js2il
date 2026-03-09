"use strict";

const url = require("url");

const parsed = new url.URL("https://example.com/search?q=first&q=second");
console.log("initial q:", parsed.searchParams.get("q"));
console.log("all q:", parsed.searchParams.getAll("q").join("|"));
console.log("has mode:", parsed.searchParams.has("mode"));

parsed.searchParams.append("mode", "fast");
parsed.searchParams.set("q", "done now");
parsed.searchParams.append("empty", "");

parsed.searchParams.forEach((value, key) => {
    console.log("pair:", key + "=" + value);
});

const receiver = { tag: "ctx" };
parsed.searchParams.forEach(function (value, key) {
    if (key === "q") {
        console.log("this tag:", this.tag);
    }
}, receiver);

console.log("search:", parsed.search);
console.log("href:", parsed.href);

const copy = new url.URLSearchParams("alpha=1&beta=two+words");
console.log("copy:", copy.toString());
console.log("copy beta:", copy.get("beta"));
