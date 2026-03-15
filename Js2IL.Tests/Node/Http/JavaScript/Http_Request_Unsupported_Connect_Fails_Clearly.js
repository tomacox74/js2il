"use strict";

const http = require("node:http");

try {
  const req = http.request({
    host: "127.0.0.1",
    port: 80,
    path: "/tunnel",
    method: "CONNECT"
  });

  req.end();
  console.log("unexpected");
} catch (err) {
  console.log("connect unsupported:" + /CONNECT requests are not supported/.test(err.message));
}
