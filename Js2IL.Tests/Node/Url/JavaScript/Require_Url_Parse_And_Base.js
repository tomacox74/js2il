"use strict";

const url = require("url");

const parsed = new url.URL("https://user:pass@example.com:8080/docs/tutorial?topic=js2il#section");
console.log("protocol:", parsed.protocol);
console.log("username:", parsed.username);
console.log("password:", parsed.password);
console.log("host:", parsed.host);
console.log("hostname:", parsed.hostname);
console.log("port:", parsed.port);
console.log("pathname:", parsed.pathname);
console.log("search:", parsed.search);
console.log("hash:", parsed.hash);
console.log("origin:", parsed.origin);
console.log("topic:", parsed.searchParams.get("topic"));

const based = new url.URL("../guide?lang=en", "https://example.com/docs/start/");
console.log("based href:", based.href);
console.log("based path:", based.pathname);
console.log("based lang:", based.searchParams.get("lang"));
