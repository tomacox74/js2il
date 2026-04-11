"use strict";

const http = require("node:http");

const server = http.createServer(function (req, res) {
  res.setHeader("Content-Type", "text/plain");
  res.write("alpha");
  res.end("beta");
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();

  http.get(
    {
      host: info.address,
      port: info.port,
      path: "/stream"
    },
    function (res) {
      res.setEncoding("utf8");
      console.log("transfer:" + res.headers["transfer-encoding"]);

      let chunkCount = 0;
      let body = "";
      res.on("data", function (chunk) {
        chunkCount += 1;
        console.log("chunk" + chunkCount + ":" + chunk);
        body += chunk;
      });

      res.on("end", function () {
        console.log("chunkCount:" + chunkCount);
        console.log("body:" + body);
        server.close(function () {
          console.log("done");
        });
      });
    });
});
