"use strict";

const net = require("node:net");

let serverKeepAliveLine = null;
let clientKeepAliveLine = null;
let clientNoDelayLine = null;
let clientKeepAliveDelayLine = null;
let clientDataLine = null;

const server = net.createServer(function (socket) {
  serverKeepAliveLine = "server keepalive same:" + (socket.setKeepAlive(true) === socket);
  socket.end("ok");
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  const client = net.connect(info.port, info.address, function () {
    clientKeepAliveLine = "client keepalive same:" + (client.setKeepAlive(true) === client);
    clientNoDelayLine = "client nodelay same:" + (client.setNoDelay() === client);

    try {
      client.setKeepAlive(true, 25);
    } catch (err) {
      clientKeepAliveDelayLine = "client keepalive delay:" + err.message;
    }
  });

  client.setEncoding("utf8");
  client.on("data", function (chunk) {
    clientDataLine = "client data:" + chunk;
  });

  client.on("end", function () {
    console.log(clientKeepAliveLine);
    console.log(clientNoDelayLine);
    console.log(clientKeepAliveDelayLine);
    console.log(serverKeepAliveLine);
    console.log(clientDataLine);
    server.close(function () {
      console.log("server closed");
    });
  });
});
