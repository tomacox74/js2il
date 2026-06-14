"use strict";

const https = require('node:https');
const { certPem, keyPem } = require('./Tls_TestCertificates');

const server = https.createServer({ key: keyPem, cert: certPem }, (req, res) => {
  req.setEncoding('utf8');
  let requestBody = '';

  req.on('data', (chunk) => {
    requestBody += chunk;
  });

  req.on('end', () => {
    console.log('req:' + req.method + ':' + req.url + ':' + requestBody);
    res.statusCode = 200;
    res.end('echo:' + requestBody);
  });
});

server.listen(0, '127.0.0.1', () => {
  const address = server.address();
  const req = https.request(
    {
      host: '127.0.0.1',
      port: address.port,
      method: 'POST',
      path: '/submit',
      rejectUnauthorized: false,
    },
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

  req.end('hello tls');
});
