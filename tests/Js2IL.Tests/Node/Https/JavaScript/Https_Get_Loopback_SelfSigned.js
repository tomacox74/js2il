"use strict";

const https = require('node:https');
const { certPem, keyPem } = require('./Tls_TestCertificates');

const server = https.createServer({ key: keyPem, cert: certPem }, (req, res) => {
  console.log('req:' + req.method + ':' + req.url);
  res.statusCode = 201;
  res.setHeader('content-type', 'text/plain');
  res.end('secure hello');
});

server.listen(0, '127.0.0.1', () => {
  const address = server.address();
  console.log('listening:' + address.address + ':ready');

  https.get(
    'https://127.0.0.1:' + address.port + '/hello?x=1',
    { rejectUnauthorized: false },
    (res) => {
      res.setEncoding('utf8');
      console.log('status:' + res.statusCode);

      let body = '';
      res.on('data', (chunk) => {
        body += chunk;
      });
      res.on('end', () => {
        console.log('body:' + body);
        server.close(() => {
          console.log('closed');
        });
      });
    }
  );
});
