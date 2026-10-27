"""Private, read-only Grafana acceptance measurements; never deploy or record telemetry."""
import argparse
from datetime import datetime, timedelta, timezone
import hashlib
import json
import os
from pathlib import Path
import statistics
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request
import uuid

from .diagnostics import report_failure

REPO = Path(__file__).resolve().parents[3]
DATASOURCE = {'type': 'grafana-postgresql-datasource', 'uid': 'dfdvmewbhyccga'}
DASHBOARDS = ('test262-summary', 'test262-conformance')
REPRESENTATIVE = {'test262-summary': {100, 107}, 'test262-conformance': {6}}
PERFORMANCE = {
    3: """SELECT run_at AS "Last run (UTC)"
FROM public.perf_results
WHERE $__timeFilter(run_at)
  AND source = 'prime-script'
  AND benchmark_profile = 'default'
    AND COALESCE(dotnet_runtime, '') IN ('', 'net10.0')
    AND os_platform = 'linux'
  AND scenario = 'prime-javascript' AND metric = 'passes_per_second'
ORDER BY run_at DESC, run_id DESC, run_attempt DESC
LIMIT 1;""",
    9: """SELECT run_at AS "Last run (UTC)"
FROM public.perf_results
WHERE $__timeFilter(run_at)
  AND source = 'benchmarkdotnet'
  AND benchmark_profile = 'default'
    AND COALESCE(dotnet_runtime, '') IN ('', 'net10.0')
    AND os_platform = 'linux'
  AND metric = 'mean_ns'
ORDER BY run_at DESC, run_id DESC, run_attempt DESC
LIMIT 1;""",
}
ACCESS = {
    'role': """SELECT current_user AS login,rolsuper,rolbypassrls,rolcreaterole,rolcreatedb,
current_setting('transaction_read_only') AS transaction_read_only,statement_timestamp() AS as_of
FROM pg_catalog.pg_roles WHERE rolname=current_user""",
    'relations': """SELECT n.nspname,c.relname,c.relkind,
has_schema_privilege(n.oid,'USAGE') AS schema_usage,
has_table_privilege(c.oid,'SELECT') AS can_select,
has_table_privilege(c.oid,'INSERT,UPDATE,DELETE,TRUNCATE,REFERENCES,TRIGGER') AS has_other_privilege
FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace
WHERE c.relkind IN ('r','p','v','m','f') AND
(n.nspname IN ('test262','test262_reporting') OR (n.nspname='public' AND c.relname='perf_results'))
ORDER BY n.nspname,c.relname LIMIT 500""",
    'routines': """SELECT n.nspname,p.proname,pg_get_function_identity_arguments(p.oid) AS arguments,
has_schema_privilege(n.oid,'USAGE') AND has_function_privilege(p.oid,'EXECUTE') AS effective_execute
FROM pg_catalog.pg_proc p JOIN pg_catalog.pg_namespace n ON n.oid=p.pronamespace
WHERE n.nspname IN ('test262','test262_api') ORDER BY n.nspname,p.proname,p.oid LIMIT 500""",
    'backup-isolation': """SELECT CASE WHEN EXISTS
(SELECT 1 FROM pg_catalog.pg_roles WHERE rolname='test262_backup')
THEN has_table_privilege('test262_backup','public.perf_results','SELECT') END AS backup_can_select_performance""",
}


class AuditError(ValueError):
    def __init__(self, kind, status=None):
        super().__init__(kind)
        self.kind = kind
        self.status = status

    def record(self):
        return {'state': 'error', 'kind': self.kind, 'http_status': self.status}


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


class Grafana:
    def __init__(self, url, token):
        parsed = urllib.parse.urlsplit(url)
        if (parsed.scheme != 'https' or not parsed.hostname or parsed.username or
                parsed.password or parsed.query or parsed.fragment or not token):
            raise AuditError('invalid-grafana-configuration')
        self.url = url.rstrip('/')
        self.token = token
        self.opener = urllib.request.build_opener(NoRedirect())

    def request(self, path, body=None):
        request = urllib.request.Request(self.url + path,
            headers={'Authorization': 'Bearer ' + self.token, 'Content-Type': 'application/json'},
            data=None if body is None else json.dumps(body).encode())
        try:
            with self.opener.open(request, timeout=45) as response:
                raw = response.read(8_000_001)
        except urllib.error.HTTPError as error:
            raise AuditError('http-error', error.code) from None
        except (urllib.error.URLError, TimeoutError):
            raise AuditError('transport-error') from None
        if len(raw) > 8_000_000:
            raise AuditError('response-too-large')
        try:
            return json.loads(raw)
        except (ValueError, UnicodeError):
            raise AuditError('invalid-json-response') from None

    def dashboard(self, uid):
        # All callers use fixed, reviewed UIDs, not response-supplied links.
        document = self.request('/api/dashboards/uid/' + uid)
        dashboard = document.get('dashboard') if isinstance(document, dict) else None
        if not isinstance(dashboard, dict) or dashboard.get('uid') != uid:
            raise AuditError('invalid-dashboard-response')
        return dashboard

    def query(self, sql, window):
        started = time.monotonic()
        document = self.request('/api/ds/query', {
            'from': window['from_ms'], 'to': window['to_ms'],
            'queries': [{'refId': 'A', 'datasource': DATASOURCE, 'rawSql': sql,
                         'format': 'table', 'editorMode': 'code', 'rawQuery': True,
                         'intervalMs': 1000, 'maxDataPoints': 1000}]})
        result = decode_query(document)
        result['end_to_end_ms'] = round((time.monotonic() - started) * 1000, 3)
        return result


def decode_query(document):
    try:
        result = document['results']['A']
        if result.get('error') or result.get('status', 200) != 200:
            raise AuditError('datasource-query-error')
        if not isinstance(result.get('frames'), list):
            raise AuditError('invalid-query-response')
        frames = []
        for frame in result['frames']:
            names = [field['name'] for field in frame['schema']['fields']]
            values = frame['data']['values']
            if (not names or any(not isinstance(name, str) or not name for name in names) or
                    len(set(names)) != len(names) or len(names) != len(values) or
                    any(not isinstance(column, list) for column in values) or
                    len({len(column) for column in values}) != 1):
                raise AuditError('invalid-query-frame')
            frames.append({'fields': names, 'rows': [dict(zip(names, row)) for row in zip(*values)]})
        return {'state': 'ok' if frames and any(f['rows'] for f in frames) else 'no-data',
                'frames': frames}
    except (KeyError, TypeError, AttributeError):
        raise AuditError('invalid-query-response') from None


def digest(value):
    return hashlib.sha256(json.dumps(value, sort_keys=True, separators=(',', ':')).encode()).hexdigest()


def panel_queries(dashboard):
    result = {}
    try:
        for panel in dashboard['panels']:
            for target in panel.get('targets', []):
                if target.get('rawSql'):
                    key = f"{panel['id']}:{target['refId']}"
                    if key in result:
                        raise AuditError('ambiguous-panel-query')
                    result[key] = {'sql': target['rawSql'],
                        'datasource': target.get('datasource', panel.get('datasource')),
                        'format': target.get('format', 'table')}
    except (KeyError, TypeError, AttributeError):
        raise AuditError('invalid-dashboard-panels') from None
    return result


def compare_dashboard(expected, deployed):
    left, right = panel_queries(expected), panel_queries(deployed)
    expected_ids = {p['id'] for p in expected['panels']}
    deployed_ids = {p['id'] for p in deployed['panels']}
    def variables(dashboard):
        keys = ('name', 'type', 'datasource', 'definition', 'query', 'multi', 'includeAll')
        return [{key: variable.get(key) for key in keys}
                for variable in dashboard.get('templating', {}).get('list', [])]
    navigation_matches = (variables(expected) == variables(deployed) and
                          expected.get('links', []) == deployed.get('links', []))
    return {'state': 'matched' if left == right and expected_ids == deployed_ids and navigation_matches else 'query-drift',
            'missing_queries': sorted(left.keys() - right.keys()),
            'extra_queries': sorted(right.keys() - left.keys()),
            'changed_queries': sorted(key for key in left.keys() & right.keys() if left[key] != right[key]),
            'missing_panels': sorted(expected_ids - deployed_ids),
            'extra_panels': sorted(deployed_ids - expected_ids),
            'navigation_matches': navigation_matches,
            'expected_sha256': digest(expected), 'deployed_sha256': digest(deployed),
            'query_contract_sha256': digest(right)}


def measure(grafana, sql, window, samples, explain=False):
    result = {'sql_sha256': hashlib.sha256(sql.encode()).hexdigest(), 'samples': []}
    for _ in range(samples):
        try:
            sample = grafana.query(sql, window)
        except AuditError as error:
            result.update(error.record())
            return result
        result['samples'].append(sample)
    timings = [sample['end_to_end_ms'] for sample in result['samples']]
    result.update(state='measured', end_to_end_ms={
        'min': min(timings), 'median': statistics.median(timings), 'max': max(timings)},
        diagnostic_samples_within_2000ms=all(t <= 2000 for t in timings),
        sustained_slo_verified=False)
    if explain or max(timings) > 2000:
        try:
            profile = grafana.query('EXPLAIN (ANALYZE, FORMAT JSON, TIMING OFF) ' + sql.rstrip(';'), window)
            rows = profile['frames'][0]['rows']
            plan = rows[0]['QUERY PLAN']
            if isinstance(plan, str):
                plan = json.loads(plan)
            result['separate_explain_analyze'] = {
                'planning_ms': plan[0]['Planning Time'], 'execution_ms': plan[0]['Execution Time'],
                'end_to_end_ms': profile['end_to_end_ms'], 'plan': plan[0]}
        except AuditError as error:
            result['separate_explain_analyze'] = error.record()
            result['state'] = 'error'
        except (KeyError, IndexError, TypeError, ValueError):
            result['separate_explain_analyze'] = AuditError('invalid-explain-response').record()
            result['state'] = 'error'
    return result


def collect(grafana, repository_id, samples=3, now=None):
    repository_id = str(uuid.UUID(repository_id))
    if type(samples) is not int or not 1 <= samples <= 3:
        raise AuditError('invalid-sample-count')
    now = now or datetime.now(timezone.utc)
    if now.utcoffset() is None:
        raise AuditError('timezone-required')
    window = {'from_ms': str(int((now - timedelta(days=30)).timestamp() * 1000)),
              'to_ms': str(int(now.timestamp() * 1000))}
    report = {'schema_version': 1, 'observed_at': now.isoformat(), 'repository_id': repository_id,
              'collector_sha256': hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
              'read_only_operations': True, 'mutations_performed': False, 'window': window,
              'acceptance': 'operator-review-required', 'browser_rendering_measured': False,
              'coherent_database_snapshot': False,
              'server_read_only_transaction_enforced': False,
              'dashboards': {}, 'queries': {}, 'access': {}}
    for filename in DASHBOARDS:
        expected = json.loads((REPO / 'grafana/dashboards' / (filename + '.json')).read_text())
        uid = expected['uid']
        try:
            deployed = grafana.dashboard(uid)
            comparison = compare_dashboard(expected, deployed)
            report['dashboards'][filename] = comparison
            if comparison['state'] != 'matched':
                continue
        except AuditError as error:
            report['dashboards'][filename] = error.record()
            continue
        # Execute reviewed repository SQL only after matching the deployed contract.
        for key, target in panel_queries(expected).items():
            sql = target['sql'].replace('${repository:sqlstring}', "'" + repository_id + "'")
            representative = int(key.split(':')[0]) in REPRESENTATIVE[filename]
            report['queries'][filename + '/' + key] = measure(
                grafana, sql, window, samples if representative else 1, representative)
    expected_performance = {'panels': [
        {'id': panel_id, 'targets': [{'refId': 'A', 'rawSql': sql, 'datasource': DATASOURCE, 'format': 'table'}]}
        for panel_id, sql in PERFORMANCE.items()]}
    try:
        deployed = grafana.dashboard('tonh8st')
        deployed = {'panels': [p for p in deployed['panels'] if p['id'] in PERFORMANCE]}
        comparison = compare_dashboard(expected_performance, deployed)
        report['dashboards']['performance-sample'] = comparison
        if comparison['state'] == 'matched':
            for panel_id, sql in PERFORMANCE.items():
                # Expand the single reviewed macro ourselves for the separate EXPLAIN.
                time_filter = (f"run_at >= to_timestamp({int(window['from_ms'])}/1000.0) "
                               f"AND run_at <= to_timestamp({int(window['to_ms'])}/1000.0)")
                sql = sql.replace('$__timeFilter(run_at)', time_filter)
                report['queries'][f'performance/{panel_id}'] = measure(grafana, sql, window, samples, True)
    except AuditError as error:
        report['dashboards']['performance-sample'] = error.record()
    for name, sql in ACCESS.items():
        report['access'][name] = measure(grafana, sql, window, 1)
    span = ("SELECT min(started_at) AS first_execution_at,max(started_at) AS last_execution_at,"
            "count(DISTINCT date_trunc('day',started_at AT TIME ZONE 'UTC')) AS active_utc_days,"
            "count(DISTINCT provenance_id) AS provenances,count(DISTINCT run_id) AS runs "
            "FROM test262_reporting.v_execution_classification WHERE repository_id='" + repository_id +
            "' AND started_at>=statement_timestamp()-interval '7 days'")
    report['queries']['duplicate-window-coverage'] = measure(grafana, span, window, 1)
    report['collection_complete'] = (
        all(d['state'] == 'matched' for d in report['dashboards'].values()) and
        all(q['state'] == 'measured' for q in list(report['queries'].values()) + list(report['access'].values())))
    report['finished_at'] = datetime.now(timezone.utc).isoformat()
    return report


def private_path(path):
    path = Path(path).resolve()
    if path == REPO or REPO in path.parents:
        raise AuditError('output-must-be-outside-checkout')
    return path


def write_private(path, document):
    path = private_path(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    temporary = None
    try:
        with tempfile.NamedTemporaryFile(mode='w', encoding='utf-8', dir=path.parent, delete=False) as handle:
            temporary = Path(handle.name)
            json.dump(document, handle, sort_keys=True, indent=2)
            handle.write('\n')
        os.replace(temporary, path)
    finally:
        if temporary is not None:
            temporary.unlink(missing_ok=True)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--repository-id', required=True)
    parser.add_argument('--samples', type=int, choices=(1, 2, 3), default=3)
    parser.add_argument('--output', required=True, help='Private path outside the checkout')
    args = parser.parse_args(argv)
    output = private_path(args.output)
    grafana = Grafana(os.environ['GRAFANA_URL'], os.environ['GRAFANA_TOKEN'])
    document = collect(grafana, args.repository_id, args.samples)
    write_private(output, document)
    print(json.dumps({'collection_complete': document['collection_complete'],
                      'acceptance': document['acceptance'], 'mutations_performed': False}))
    return 0 if document['collection_complete'] else 2


if __name__ == '__main__':
    try:
        raise SystemExit(main())
    except Exception as error:
        report_failure(error)
        raise SystemExit(1)
