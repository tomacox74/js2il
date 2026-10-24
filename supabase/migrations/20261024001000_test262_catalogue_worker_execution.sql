-- Approved repository-bound execution clients, without scheduling/publication rights.
-- The v1 subject permission remains 'coordinator' for claim/complete compatibility;
-- SQL EXECUTE grants, not that label, define this role's limited capabilities.
SET LOCAL lock_timeout='5s';
CREATE ROLE test262_worker NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;
GRANT USAGE ON SCHEMA test262 TO test262_worker;
GRANT EXECUTE ON FUNCTION test262.api_contract(),
 test262.api_read(bigint,text,jsonb,jsonb,int),
 test262.api_start_run(bigint,jsonb),test262.api_ingest(bigint,uuid,jsonb),
 test262.api_claim(bigint,uuid,uuid,bigint,int),test262.api_renew(bigint,uuid,bigint,int),
 test262.api_complete(bigint,uuid,bigint,uuid),
 test262.api_metric(bigint,uuid,text,int,bigint,int,int),
 test262.api_finish_run(bigint,uuid,text,timestamptz,jsonb) TO test262_worker;
-- No raw tables, api_put/transition/reconcile/export_open/finish_budget, reporter
-- or coordinator membership. Workers cannot create allocations/targets/batches.
