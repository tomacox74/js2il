"use strict";

const url = require("url");

try {
    new url.URL("https://[invalid]", "https://example.com/");
    console.log("threw:", false);
} catch (error) {
    console.log("threw:", true);
    console.log("name:", error.name);
    console.log("message:", error.message);
}
