"use strict";

const tls = require('node:tls');
const { certPem, keyPem } = require('./Tls_TestCertificates');

const secureContext = tls.createSecureContext({ key: keyPem, cert: certPem });
console.log('secureContext:' + !!secureContext);

const server = tls.createServer({ secureContext }, (socket) => {
  socket.end('ctx-ok');
});

server.listen(0, '127.0.0.1', () => {
  const address = server.address();
  const client = tls.connect(
    { port: address.port, host: '127.0.0.1', rejectUnauthorized: false },
    () => {
      client.setEncoding('utf8');
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
