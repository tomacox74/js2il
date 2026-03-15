"use strict";

const net = require("node:net");

const server = net.createServer(function (socket) {
  const received = [];
  let replied = false;

  socket.on("data", function (chunk) {
    received.push(chunk);
    const payload = Buffer.concat(received);
    if (payload.length < 4 || replied) {
      return;
    }

    replied = true;
    console.log("server buffer:" + Buffer.isBuffer(payload));
    console.log("server bytes:" + payload.length + ":" + payload[0] + ":" + payload[1] + ":" + payload[2] + ":" + payload[3]);
    socket.end(Buffer.from([payload[3], payload[2], payload[1], payload[0]]));
  });
});

server.listen(0, "127.0.0.1", function () {
  const info = server.address();
  const client = net.connect(info.port, info.address, function () {
    client.write(Buffer.from([1, 2, 65, 66]));
  });

  const reply = [];
  client.on("data", function (chunk) {
    console.log("client buffer:" + Buffer.isBuffer(chunk));
    reply.push(chunk);
    client.end();
  });

  client.on("end", function () {
    const payload = Buffer.concat(reply);
    console.log("client bytes:" + payload.length + ":" + payload[0] + ":" + payload[1] + ":" + payload[2] + ":" + payload[3]);
    server.close(function () {
      console.log("server closed");
    });
  });
});
