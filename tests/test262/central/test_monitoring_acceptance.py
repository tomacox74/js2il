from datetime import datetime, timezone
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import Mock, patch
import urllib.error

from scripts.test262.central import monitoring_acceptance as audit


def frame(rows=None):
    return {'results': {'A': {'status': 200, 'frames': [
        {'schema': {'fields': [{'name': 'value'}]}, 'data': {'values': [rows or []]}}]}}}


class MonitoringAcceptanceTests(unittest.TestCase):
    def test_datasource_errors_and_malformed_frames_cannot_become_healthy(self):
        for name, document in (
                ('http-ok-query-error', {'results': {'A': {'status': 200, 'error': 'private SQL error'}}}),
                ('failed-query-status', {'results': {'A': {'status': 500}}}),
                ('missing-frames', {'results': {'A': {'status': 200}}}),
                ('missing-result', {}),
                ('uneven-columns', {'results': {'A': {'frames': [
                    {'schema': {'fields': [{'name': 'a'}, {'name': 'b'}]},
                     'data': {'values': [[1], []]}}]}}})):
            with self.subTest(case=name), self.assertRaises(audit.AuditError) as caught:
                audit.decode_query(document)
            self.assertNotIn('private SQL error', str(caught.exception))
        self.assertEqual(audit.decode_query(frame())['state'], 'no-data')
        self.assertEqual(audit.decode_query(frame([7]))['frames'][0]['rows'], [{'value': 7}])

    def test_drift_blocks_unreviewed_live_queries_but_ignores_visual_defaults(self):
        expected = {'panels': [{'id': 100, 'datasource': audit.DATASOURCE,
            'targets': [{'refId': 'A', 'rawSql': 'SELECT 1', 'format': 'table'}]}]}
        live = json.loads(json.dumps(expected))
        live['panels'][0]['fieldConfig'] = {'defaults': {}}
        self.assertEqual(audit.compare_dashboard(expected, live)['state'], 'matched')
        live['panels'][0]['targets'][0]['rawSql'] = 'SELECT unreviewed_function()'
        comparison = audit.compare_dashboard(expected, live)
        self.assertEqual(comparison['state'], 'query-drift')
        self.assertEqual(comparison['changed_queries'], ['100:A'])
        missing_intro = json.loads(json.dumps(expected))
        missing_intro['panels'].append({'id': 1, 'type': 'text'})
        self.assertEqual(audit.compare_dashboard(missing_intro, expected)['missing_panels'], [1])
        changed_variable = dict(expected, templating={'list': [{'name': 'repository', 'query': 'SELECT changed'}]})
        self.assertFalse(audit.compare_dashboard(expected, changed_variable)['navigation_matches'])
        grafana = Mock()
        grafana.dashboard.return_value = live
        grafana.query.return_value = {'state': 'ok', 'frames': [], 'end_to_end_ms': 10}
        with patch.object(audit, 'DASHBOARDS', ('test262-summary',)), \
                patch.object(Path, 'read_text', return_value=json.dumps(expected | {'uid': 'expected'})):
            result = audit.collect(grafana, '00000000-0000-0000-0000-000000000001', samples=1)
        self.assertFalse(result['collection_complete'])
        self.assertNotIn('test262-summary/100:A', result['queries'])
        self.assertFalse(any('unreviewed_function' in call.args[0] for call in grafana.query.call_args_list))

    def test_samples_keep_transport_and_instrumented_database_timing_separate(self):
        grafana = Mock()
        grafana.query.side_effect = [
            {'state': 'ok', 'frames': [], 'end_to_end_ms': milliseconds} for milliseconds in (15, 2000, 2001)
        ] + [{'state': 'ok', 'frames': [{'fields': ['QUERY PLAN'], 'rows': [
            {'QUERY PLAN': [{'Planning Time': 1.5, 'Execution Time': 8.5}]}]}], 'end_to_end_ms': 20}]
        result = audit.measure(grafana, 'SELECT 1;', {}, 3, explain=True)
        self.assertEqual(result['end_to_end_ms'], {'min': 15, 'median': 2000, 'max': 2001})
        self.assertFalse(result['diagnostic_samples_within_2000ms'])
        self.assertFalse(result['sustained_slo_verified'])
        self.assertEqual(result['separate_explain_analyze'],
                         {'planning_ms': 1.5, 'execution_ms': 8.5, 'end_to_end_ms': 20,
                          'plan': {'Planning Time': 1.5, 'Execution Time': 8.5}})
        self.assertEqual(grafana.query.call_args.args[0],
                         'EXPLAIN (ANALYZE, FORMAT JSON, TIMING OFF) SELECT 1')
        grafana.query.side_effect = [{'state': 'no-data', 'frames': [], 'end_to_end_ms': 2000}]
        self.assertTrue(audit.measure(grafana, 'SELECT 1', {}, 1)['diagnostic_samples_within_2000ms'])

    def test_query_failure_is_reported_without_retry_or_raw_response(self):
        grafana = Mock()
        grafana.query.side_effect = audit.AuditError('http-error', 503)
        result = audit.measure(grafana, 'SELECT 1', {}, 3)
        self.assertEqual(result, {'sql_sha256': result['sql_sha256'], 'samples': [],
                                 'state': 'error', 'kind': 'http-error', 'http_status': 503})
        grafana.query.assert_called_once_with('SELECT 1', {})

    def test_slow_query_automatically_retains_a_separate_plan_not_a_passing_slo(self):
        grafana = Mock()
        grafana.query.side_effect = [
            {'state': 'ok', 'frames': [], 'end_to_end_ms': 2100},
            {'state': 'ok', 'end_to_end_ms': 2200, 'frames': [{'rows': [
                {'QUERY PLAN': '[{"Planning Time": 3, "Execution Time": 2050, "Plan": {"Node Type": "Aggregate"}}]'}]}]}]
        result = audit.measure(grafana, 'SELECT 1', {}, 1)
        self.assertEqual(result['separate_explain_analyze']['execution_ms'], 2050)
        self.assertEqual(result['separate_explain_analyze']['plan']['Plan'], {'Node Type': 'Aggregate'})
        self.assertFalse(result['diagnostic_samples_within_2000ms'])
        self.assertFalse(result['sustained_slo_verified'])

    def test_http_transport_never_follows_redirects_or_retains_error_bodies(self):
        grafana = audit.Grafana('https://grafana.example', 'private-token')
        grafana.opener = Mock()
        grafana.opener.open.side_effect = urllib.error.HTTPError(
            'https://grafana.example', 302, 'private-token', {}, None)
        with self.assertRaises(audit.AuditError) as caught:
            grafana.request('/api/dashboards/uid/reviewed')
        self.assertEqual(caught.exception.record(),
                         {'state': 'error', 'kind': 'http-error', 'http_status': 302})
        self.assertNotIn('private-token', str(caught.exception))
        self.assertIsNone(audit.NoRedirect().redirect_request(None, None, 302, None, None, 'https://other.example'))
        for name, url in (('plaintext', 'http://grafana.example'),
                          ('userinfo', 'https://secret@grafana.example'),
                          ('query', 'https://grafana.example?token=secret')):
            with self.subTest(case=name), self.assertRaises(audit.AuditError):
                audit.Grafana(url, 'private-token')

    def test_collection_renders_only_validated_repository_scope_and_records_no_data(self):
        expected = {'uid': 'reviewed', 'panels': [{'id': 2, 'datasource': audit.DATASOURCE,
            'targets': [{'refId': 'A', 'rawSql': 'SELECT ${repository:sqlstring}', 'format': 'table'}]}]}
        grafana = Mock()
        grafana.dashboard.side_effect = [expected, audit.AuditError('http-error', 403)]
        grafana.query.return_value = {'state': 'no-data', 'frames': [], 'end_to_end_ms': 10}
        with patch.object(audit, 'DASHBOARDS', ('test262-summary',)), \
                patch.object(Path, 'read_text', return_value=json.dumps(expected)):
            result = audit.collect(grafana, '00000000-0000-0000-0000-000000000001', samples=1,
                                   now=datetime(2026, 10, 27, tzinfo=timezone.utc))
        self.assertFalse(result['collection_complete'])
        self.assertEqual(result['queries']['test262-summary/2:A']['samples'][0]['state'], 'no-data')
        self.assertEqual(grafana.query.call_args_list[0].args[0],
                         "SELECT '00000000-0000-0000-0000-000000000001'")
        self.assertFalse(result['mutations_performed'])
        self.assertEqual(result['acceptance'], 'operator-review-required')
        grafana.reset_mock()
        with self.assertRaises(ValueError):
            audit.collect(grafana, "'; SELECT unreviewed_function(); --")
        grafana.dashboard.assert_not_called()
        for count in (False, 0, 4):
            with self.subTest(case='invalid-count-' + str(count)), self.assertRaises(audit.AuditError):
                audit.collect(grafana, '00000000-0000-0000-0000-000000000001', samples=count)

    def test_private_report_is_atomic_restricted_and_cannot_enter_checkout(self):
        with tempfile.TemporaryDirectory() as temporary:
            path = Path(temporary) / 'receipt.json'
            audit.write_private(path, {'collection_complete': False})
            self.assertEqual(json.loads(path.read_text()), {'collection_complete': False})
            self.assertEqual(path.stat().st_mode & 0o777, 0o600)
            with patch.object(audit.json, 'dump', side_effect=ValueError('bad document')), \
                    self.assertRaises(ValueError):
                audit.write_private(path, {})
            self.assertEqual(json.loads(path.read_text()), {'collection_complete': False})
            self.assertEqual([p.name for p in Path(temporary).iterdir()], ['receipt.json'])
        with self.assertRaises(audit.AuditError):
            audit.write_private(audit.REPO / 'private-audit.json', {})

    def test_complete_audit_uses_only_read_queries_and_preserves_pending_acceptance(self):
        grafana = Mock()
        def dashboard(uid):
            if uid == 'tonh8st':
                return {'uid': uid, 'links': [{'url': '/unrelated'}],
                        'templating': {'list': [{'name': 'unrelated'}]},
                        'panels': [{'id': key, 'targets': [
                            {'refId': 'A', 'rawSql': sql, 'datasource': audit.DATASOURCE, 'format': 'table'}]}
                            for key, sql in audit.PERFORMANCE.items()]}
            return json.loads((audit.REPO / 'grafana/dashboards' /
                              (uid.removeprefix('jroc-') + '.json')).read_text())
        def query(sql, window):
            if sql.startswith('EXPLAIN'):
                return {'state': 'ok', 'end_to_end_ms': 10, 'frames': [{'rows': [
                    {'QUERY PLAN': [{'Planning Time': 1, 'Execution Time': 2}]}]}]}
            return dict(audit.decode_query(frame([1])), end_to_end_ms=10)
        grafana.dashboard.side_effect = dashboard
        grafana.query.side_effect = query
        result = audit.collect(grafana, '00000000-0000-0000-0000-000000000001', samples=1)
        self.assertTrue(result['collection_complete'])
        self.assertEqual(result['acceptance'], 'operator-review-required')
        self.assertFalse(result['coherent_database_snapshot'])
        self.assertFalse(result['server_read_only_transaction_enforced'])
        for call in grafana.query.call_args_list:
            sql = call.args[0]
            self.assertTrue(sql.startswith(('SELECT ', 'EXPLAIN (ANALYZE, FORMAT JSON, TIMING OFF) SELECT ')))
            self.assertNotIn('${repository:', sql)
            self.assertNotIn('$__timeFilter', sql)
