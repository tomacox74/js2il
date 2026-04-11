"use strict";

const net = require("node:net");

const server = net.createServer({ allowHalfOpen: true }, function (socket) {
  console.log("server allowHalfOpen:" + socket.allowHalfOpen);

  let received = "";
  socket.on("data", function (chunk) {
    received += chunk.toString("utf8");
  });

  socket.on("end", function () {
    console.log("server end:" + received);
    setTimeout(function () {
      console.log("server writable:" + socket.writable);
      socket.end("delayed:" + received.toUpperCase());
    }, 0);
  });
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  const client = net.connect(info.port, info.address, function () {
    client.end("ping");
  });

  client.setEncoding("utf8");
  let reply = "";
  client.on("data", function (chunk) {
    reply += chunk;
  });

  client.on("end", function () {
    console.log("client recv:" + reply);
    server.close(function () {
      console.log("server closed");
    });
  });
});
