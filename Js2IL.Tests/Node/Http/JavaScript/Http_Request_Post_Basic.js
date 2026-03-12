"use strict";

const http = require("node:http");

const server = http.createServer(function (req, res) {
  let body = "";

  req.on("data", function (chunk) {
    body += chunk;
  });

  req.on("end", function () {
    console.log("server method:" + req.method);
    console.log("server url:" + req.url);
    console.log("server type:" + req.headers["content-type"]);
    console.log("server body:" + body);

    res.writeHead(202, {
      "Content-Type": "text/plain",
      "X-Body-Length": String(body.length)
    });
    res.write("accepted:");
    res.end(body.toUpperCase());
  });
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  const req = http.request(
    {
      host: info.address,
      port: info.port,
      path: "/submit",
      method: "POST",
      headers: {
        "Content-Type": "text/plain"
      }
    },
    function (res) {
      console.log("client status:" + res.statusCode);
      console.log("client type:" + res.headers["content-type"]);
      console.log("client length:" + res.headers["x-body-length"]);

      let responseBody = "";
      res.on("data", function (chunk) {
        responseBody += chunk;
      });

      res.on("end", function () {
        console.log("client body:" + responseBody);
        server.close(function () {
          console.log("done");
        });
      });
    });

  req.write("node ");
  req.end("baseline");
});
