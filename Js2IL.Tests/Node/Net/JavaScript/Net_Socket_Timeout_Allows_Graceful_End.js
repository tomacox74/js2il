"use strict";

const net = require("node:net");

const server = net.createServer(function (socket) {
  socket.setTimeout(20, function () {
    console.log("server timeout");
    console.log("server writable:" + socket.writable);
    socket.end("timeout-response");
  });

  socket.on("close", function (hadError) {
    console.log("server socket close:" + hadError);
  });
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  const client = net.connect(info.port, info.address, function () {
    console.log("client connected");
  });

  client.setEncoding("utf8");
  client.on("data", function (chunk) {
    console.log("client data:" + chunk);
  });

  client.on("end", function () {
    console.log("client end");
    server.close();
  });
});
