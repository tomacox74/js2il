"use strict";

const net = require("node:net");

const server = net.createServer(function (socket) {
  let received = "";

  socket.on("data", function (chunk) {
    received += chunk.toString("utf8");
  });

  socket.on("end", function () {
    console.log("server recv:" + received);
    socket.write("pong:" + received.toUpperCase());
    socket.end();
  });
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  console.log("listening:" + info.family + ":" + info.address + ":" + (info.port > 0));

  const client = net.connect(info.port, info.address, function () {
    client.end("ping");
  });

  let reply = "";
  client.on("data", function (chunk) {
    reply += chunk.toString("utf8");
  });

  client.on("end", function () {
    console.log("client recv:" + reply);
    server.close(function () {
      console.log("server closed");
    });
  });
});
