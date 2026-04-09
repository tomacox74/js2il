"use strict";

const https = require('node:https');
const { URL: NodeUrl } = require('node:url');
const { certPem, keyPem } = require('./Tls_TestCertificates');

const server = https.createServer({ key: keyPem, cert: certPem }, (req, res) => {
  console.log('req:' + req.method + ':' + req.url + ':' + req.headers['accept-encoding']);
  res.statusCode = 200;
  res.end('url-object-ok');
});

server.listen(0, '127.0.0.1', () => {
  const address = server.address();
  const target = new NodeUrl('https://127.0.0.1:' + address.port + '/spec?section=27.3');
  const req = https.request(
    target,
    {
      method: 'GET',
      headers: {
        'Accept-Encoding': 'identity',
      },
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

  req.end();
});
