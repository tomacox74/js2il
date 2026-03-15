"use strict";

const http = require("node:http");

const agent = new http.Agent({ keepAlive: true });
let connections = 0;
let responses = 0;

const server = http.createServer(function (req, res) {
  responses += 1;
  res.setHeader("Content-Type", "text/plain");
  res.end("resp" + responses);
});

server.on("connection", function () {
  connections += 1;
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();

  http.get(
    {
      host: info.address,
      port: info.port,
      path: "/one",
      agent: agent
    },
    function (res) {
      res.setEncoding("utf8");
      console.log("first connection:" + res.headers["connection"]);

      let body = "";
      res.on("data", function (chunk) {
        body += chunk;
      });

      res.on("end", function () {
        console.log("body1:" + body);

        http.get(
          {
            host: info.address,
            port: info.port,
            path: "/two",
            agent: agent
          },
          function (nextRes) {
            nextRes.setEncoding("utf8");
            console.log("second connection:" + nextRes.headers["connection"]);

            let nextBody = "";
            nextRes.on("data", function (chunk) {
              nextBody += chunk;
            });

            nextRes.on("end", function () {
              console.log("body2:" + nextBody);
              console.log("connections:" + connections);
              agent.destroy();
              server.close(function () {
                console.log("done");
              });
            });
          });
      });
    });
});
