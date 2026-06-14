"use strict";

const net = require("node:net");

const server = net.createServer(function (socket) {
  socket.setEncoding("utf8");

  let received = "";
  socket.on("data", function (chunk) {
    console.log("server chunk:" + typeof chunk + ":" + chunk);
    received += chunk;
  });

  socket.on("end", function () {
    console.log("server recv:" + received);
    socket.end("done:" + received);
  });
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  const client = net.connect(info.port, info.address, function () {
    client.write(Buffer.from([0xE2, 0x82]));
    setTimeout(function () {
      client.end(Buffer.from([0xAC, 0x21]));
    }, 0);
  });

  client.setEncoding("utf8");
  let reply = "";
  client.on("data", function (chunk) {
    console.log("client chunk:" + typeof chunk + ":" + chunk);
    reply += chunk;
  });

  client.on("end", function () {
    console.log("client recv:" + reply);
    server.close(function () {
      console.log("server closed");
    });
  });
});
