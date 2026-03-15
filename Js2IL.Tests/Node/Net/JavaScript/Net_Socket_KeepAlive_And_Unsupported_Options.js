"use strict";

const net = require("node:net");

const server = net.createServer(function (socket) {
  console.log("server keepalive same:" + (socket.setKeepAlive(true) === socket));
  socket.end("ok");
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  const client = net.connect(info.port, info.address, function () {
    console.log("client keepalive same:" + (client.setKeepAlive(true) === client));
    console.log("client nodelay same:" + (client.setNoDelay() === client));

    try {
      client.setKeepAlive(true, 25);
    } catch (err) {
      console.log("client keepalive delay:" + err.message);
    }
  });

  client.setEncoding("utf8");
  client.on("data", function (chunk) {
    console.log("client data:" + chunk);
  });

  client.on("end", function () {
    server.close(function () {
      console.log("server closed");
    });
  });
});
