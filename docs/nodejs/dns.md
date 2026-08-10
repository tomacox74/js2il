# Module: dns

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 24.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/dns.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Dns.cs`
- `src/JavaScriptRuntime/Node/Contracts/IDnsModule.Generated.cs`

## Notes

Provides the callback-based dns.lookup() behavior required by Undici. Lookup is asynchronous, preserves Node's error-first callback shape, supports hostname literals and operating-system name resolution, and supports the family, all, order, and verbatim options. Direct DNS record resolution, Resolver instances, custom DNS servers, DNS constants and error codes, and dns.promises are represented by the complete generated Node 24 contract but currently fail explicitly.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| lookup(hostname[, options], callback) | function | supported | [docs](https://nodejs.org/api/dns.html#dnslookuphostname-options-callback) |
| getDefaultResultOrder() / setDefaultResultOrder(order) | function | supported | [docs](https://nodejs.org/api/dns.html#dnsgetdefaultresultorder) |
| Resolver, resolve*, reverse, lookupService, getServers, setServers, constants, and promises | property | not-supported | [docs](https://nodejs.org/api/dns.html) |

## API Details

### lookup(hostname[, options], callback)

Supports asynchronous error-first callbacks, family 0/4/6 and IPv4/IPv6 selection, all result records, result ordering, and the deprecated verbatim option.

**Tests:**
- `Jroc.Tests.Node.Dns.ExecutionTests.Require_Dns_Lookup` (`tests/Jroc.Tests/Node/Dns/ExecutionTests.cs`)
- `Jroc.Tests.Node.Dns.GeneratorTests.Require_Dns_Lookup` (`tests/Jroc.Tests/Node/Dns/GeneratorTests.cs`)

### getDefaultResultOrder() / setDefaultResultOrder(order)

Supports verbatim, ipv4first, and ipv6first defaults used when lookup options do not specify an order.

### Resolver, resolve*, reverse, lookupService, getServers, setServers, constants, and promises

These Node 24 exports remain available in the generated contract and throw NotImplementedException explicitly when accessed through that contract.
