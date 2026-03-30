"use strict";

const http = require("node:http");

const server = http.createServer(function (req, res) {
  console.log("req method:" + req.method);
  console.log("req url:" + req.url);
  console.log("req host:" + (req.headers["host"].indexOf("127.0.0.1:") === 0));

  res.statusCode = 201;
  res.setHeader("X-Answer", "42");
  res.end("hello:" + req.url);
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  console.log("server ready:" + info.address + ":" + (info.port > 0));

  http.get("http://" + info.address + ":" + info.port + "/hello?name=js2il", function (res) {
    res.setEncoding("utf8");
    console.log("status:" + res.statusCode);
    console.log("answer:" + res.headers["x-answer"]);

    let body = "";
    res.on("data", function (chunk) {
      body += chunk;
    });

    res.on("end", function () {
      console.log("body:" + body);
      server.close(function () {
        console.log("closed");
      });
    });
  });
});
