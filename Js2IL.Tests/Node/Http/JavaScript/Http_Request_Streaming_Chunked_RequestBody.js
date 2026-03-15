"use strict";

const http = require("node:http");

const server = http.createServer(function (req, res) {
  req.setEncoding("utf8");

  let chunkCount = 0;
  let body = "";

  console.log("transfer:" + req.headers["transfer-encoding"]);

  req.on("data", function (chunk) {
    chunkCount += 1;
    console.log("chunk" + chunkCount + ":" + chunk);
    body += chunk;
  });

  req.on("end", function () {
    console.log("chunkCount:" + chunkCount);
    console.log("body:" + body);
    res.end("ok");
  });
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  const req = http.request(
    {
      host: info.address,
      port: info.port,
      path: "/stream",
      method: "POST",
      headers: {
        "Content-Type": "text/plain"
      }
    },
    function (res) {
      res.setEncoding("utf8");

      let responseBody = "";
      res.on("data", function (chunk) {
        responseBody += chunk;
      });

      res.on("end", function () {
        console.log("response:" + responseBody);
        server.close(function () {
          console.log("done");
        });
      });
    });

  req.write("alpha");
  req.end("beta");
});
