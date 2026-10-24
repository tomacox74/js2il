"""Rebuild only the owned operational panels and paused alert definitions."""
import json
from pathlib import Path
from .inventory import REPO

DS={'type':'grafana-postgresql-datasource','uid':'dfdvmewbhyccga'}
FILTER="repository_id IN (${repository:sqlstring})"
SIGNALS=['monitor-age','backup-health','restore-health','duplicate-execution-percent',
         'reservation-health','collection-lifecycle','publication-backlog',
         'native-target-freshness','ingestion-p95-seconds']


def panel(id,title,query,y,description,width=24,x=0,height=8):
    return {'id':id,'title':title,'type':'table','datasource':DS,
        'description':description,'gridPos':{'x':x,'y':y,'w':width,'h':height},
        'fieldConfig':{'defaults':{},'overrides':[]},
        'options':{'cellHeight':'sm','showHeader':True},
        'targets':[{'refId':'A','datasource':DS,'editorMode':'code','format':'table',
                    'rawQuery':True,'rawSql':query}]}


def extend(dashboard,conformance=False):
    panels=[p for p in dashboard['panels'] if p['id']<100]
    for item in panels:
        content=item.get('options',{}).get('content')
        if isinstance(content,str):
            item['options']['content']=content.replace('refresh every minute','refresh every five minutes')
    dashboard['description']=dashboard.get('description','').replace('refresh every minute','refresh every five minutes')
    y=max(p['gridPos']['y']+p['gridPos']['h'] for p in panels)
    target=panel(101 if not conformance else 100,'Master, build, pin and registration freshness',
        "SELECT evidence_kind,registration_revision,observed_master_revision,matches_observed_master,"
        "registration_validation_coherent,validation_revision,upstream_pin,execution_identity_sha256,"
        "last_trusted_evidence_at,master_observed_at,as_of FROM test262_reporting.v_target_health WHERE "+FILTER,
        y if conformance else y+10,
        'Master is an hourly GitHub observation. NULL means unknown/stale monitoring. Exact execution fingerprint binds binaries, harness and environment; empty evidence is not conformance.')
    if conformance:
        panels.append(target)
    else:
        panels += [panel(100,'Operational warnings — unknown is not healthy',
            'SELECT signal,health_state,actual,threshold,detail,as_of FROM test262_reporting.v_operational_signals WHERE '+FILTER+' ORDER BY signal',
            y,'Dashboard warnings enabled; notification rules remain paused by operator choice. Targets: duplicate executions <5% / 7d, backup age <=36h, interactive queries <=2s.',height=10),target]
        y+=18
        panels += [panel(102,'Collection lifecycle',
            'SELECT run_kind,state,source_revision,started_at,ended_at,duration_seconds,collection_reason,overdue_collection,run_id FROM test262_reporting.v_operational_runs WHERE '+FILTER+' ORDER BY started_at DESC LIMIT 30',
            y,'Collection completion is separate from suite completion and budget exhaustion.',12),
            panel(103,'Budgets and reservation accounting',
            'SELECT external_key,state,charged_attempts,attempt_limit,charged_ms,time_limit_ms,reserved_attempts,reserved_ms,reservation_mismatch,overdue_reservations,ended_at FROM test262_reporting.v_budget_health WHERE '+FILTER+' ORDER BY ended_at DESC NULLS FIRST LIMIT 100',
            y,'Ledger mismatch or a reserved lease overdue by 15 minutes needs attention. Interrupted historical allocations are not live leaks.',12,12)]
        y+=8
        panels += [panel(104,'Queue and lease health by provenance',
            'SELECT q.provenance_id,EXISTS(SELECT 1 FROM test262_reporting.v_target_health t WHERE t.provenance_id=q.provenance_id AND t.repository_id=q.repository_id) AS current_target,q.coherent_area,q.state,q.work_count,q.expired_lease_count,q.oldest_eligible_at,q.as_of FROM test262_reporting.v_queue_health q WHERE '+FILTER+' ORDER BY current_target DESC,expired_lease_count DESC,work_count DESC LIMIT 100',
            y,'Older provenance queues remain historical work; they are not automatically current backlog.',12),
            panel(105,'Delivery latency and evidence freshness — seven days',
            'SELECT producer_id,observations,p95_delivery_seconds,clock_skew_observations,last_received_at,window_start,as_of FROM test262_reporting.v_ingestion_latency_7d WHERE '+FILTER,
            y,'Server receive minus worker finish time; negative clock-skew samples are shown separately. Local unuploaded outboxes are not visible to the server.',12,12)]
        y+=8
        panels += [panel(106,'Publication backlog and ownership',
            'SELECT pr_number,state,closure_reason,batch_age_seconds,needs_attention,expected_head,publication_id FROM test262_reporting.v_publication_health WHERE '+FILTER+' ORDER BY needs_attention DESC,batch_created_at DESC LIMIT 50',
            y,'Age is since batch creation, not an invented publication timestamp. Ownership-lost/blocked records require review; ordinary open PRs await human review.',12),
            panel(107,'Duplicate execution measurement — seven days',
            'SELECT execution_observations,baseline_executions,avoidable_repeats,intentional_retries,lease_recoveries,unclassified_repeats,uninstrumented_observations,classified_duplicate_percent,classification_complete,window_start,as_of FROM test262_reporting.v_duplicate_executions_7d WHERE '+FILTER,
            y,'Only immutable live execution observations count. Idempotent upload replay adds no observations. Explicit retries/lease recovery are excluded; unclassified repeats prevent a passing SLO claim.',12,12)]
        y+=8
        panels.append(panel(108,'Backup, retention, restore and workflow receipts',
            'SELECT workflow,expected_cadence,latest_run_id,latest_conclusion,success_run_id,success_finished_at,receipt_verified,retention_verified,retention_until,sample_age_seconds,artifact_id,as_of FROM test262_reporting.v_workflow_health WHERE '+FILTER+' ORDER BY workflow',
            y,'Digest-verified retained workflow receipts are historical checks, not fresh S3 verification. No receipt/stale collector means unknown. Performance jobs are release/manual, not daily.'))
        y+=8
        panels.append(panel(109,'Performance ingestion dates and cadence',
            "SELECT source,max(run_at) AS last_measurement_at,count(*) FILTER(WHERE run_at>=now()-interval '7 days') AS rows_in_seven_days,'release-or-manual' AS expected_cadence,statement_timestamp() AS as_of FROM public.perf_results WHERE source IN ('prime-script','benchmarkdotnet') GROUP BY source ORDER BY source",
            y,'Existing performance datasource SELECT only. Quiet time is not evidence of an ingestion failure; compare an expected release/manual run with its exact ingestion receipt.'))
    dashboard['panels']=panels
    dashboard['refresh']='5m'
    dashboard['description']+=' Operational signals use explicit unknown states; notifications are paused.' if 'Operational signals' not in dashboard.get('description','') else ''
    return dashboard


def alerts():
    rules=[]
    for signal in SIGNALS:
        rules.append({'uid':'t262-'+signal,'title':'Test262 '+signal,'condition':'C','for':'15m',
            'noDataState':'NoData','execErrState':'Error','isPaused':True,
            'labels':{'service':'test262-catalogue','severity':'warning'},
            'annotations':{'summary':'Review operational signal '+signal,
                'runbook_url':'https://github.com/tomacox74/js2il/blob/master/docs/ECMA262/Test262SupabaseCatalog.md#operational-monitoring-2341'},
            'data':[{'refId':'A','relativeTimeRange':{'from':600,'to':0},'datasourceUid':DS['uid'],
                'model':{'refId':'A','datasource':DS,'editorMode':'code','format':'table','rawQuery':True,
                    'rawSql':"SELECT repository_id::text AS repository,breach::double precision AS value FROM test262_reporting.v_operational_signals WHERE signal='"+signal+"' AND breach IS NOT NULL",'intervalMs':1000,'maxDataPoints':1000}},
                {'refId':'B','datasourceUid':'__expr__','model':{'refId':'B','type':'reduce','expression':'A','reducer':'last',
                    'conditions':[],'settings':{'mode':'strict'}}},
                {'refId':'C','datasourceUid':'__expr__','model':{'refId':'C','type':'threshold','expression':'B',
                    'conditions':[{'evaluator':{'type':'gt','params':[0]},'operator':{'type':'and'},'query':{'params':['C']},
                        'reducer':{'type':'last','params':[]},'type':'query'}]}}]})
    return {'apiVersion':1,'groups':[{'orgId':1,'name':'test262-operational-health',
        'folder':'Test262','interval':'5m','rules':rules}]}


def main():
    for filename,conformance in [('test262-summary.json',False),('test262-conformance.json',True)]:
        path=REPO/'grafana/dashboards'/filename
        dashboard=extend(json.loads(path.read_text()),conformance)
        path.write_text(json.dumps(dashboard,indent=2)+'\n')
    (REPO/'grafana/alerting/test262-operational-paused.json').write_text(json.dumps(alerts(),indent=2)+'\n')


if __name__=='__main__':
    main()
