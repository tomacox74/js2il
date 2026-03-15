"use strict";

const tls = require('node:tls');
const { certPem, keyPem } = require('./Tls_TestCertificates');

const server = tls.createServer({ key: keyPem, cert: certPem }, (socket) => {
  socket.setEncoding('utf8');
  console.log('server secure:' + socket.encrypted + ':' + socket.authorized);
  socket.end('pong');
});

server.listen(0, '127.0.0.1', () => {
  const address = server.address();
  console.log('listening:' + address.address + ':ready');

  const client = tls.connect(
    { port: address.port, host: '127.0.0.1', rejectUnauthorized: false },
    () => {
      client.setEncoding('utf8');
      console.log('client secure:' + client.encrypted + ':' + client.authorized + ':' + !!client.authorizationError);
      client.write('ping');
    }
  );

  client.on('data', (chunk) => {
    console.log('client data:' + chunk);
  });

  client.on('close', () => {
    server.close(() => {
      console.log('closed');
    });
  });
});
