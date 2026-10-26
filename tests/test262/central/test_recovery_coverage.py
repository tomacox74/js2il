from datetime import datetime, timedelta, timezone
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

from scripts.test262.central import recovery_coverage


NOW = datetime(2026, 10, 25, tzinfo=timezone.utc)


class RecoveryCoverageTests(unittest.TestCase):
    def healthy(self):
        return {'fetch_state':'observed','latest_run_id':1,'latest_state':'completed',
                'latest_conclusion':'success','receipt_verified':True,'retention_verified':True,
                'receipt_checked_at':'2026-10-24T12:00:00+00:00',
                'retention_until':'2026-11-27T00:00:00+00:00'}

    def codes(self, check):
        return {r['code'] for r in recovery_coverage.assess({'backup':check}, NOW)}

    def test_latest_failure_is_visible_despite_older_valid_receipt(self):
        check=self.healthy()
        self.assertEqual(self.codes(check),set())
        check['latest_conclusion']='failure'
        self.assertIn('latest_attempt_unsuccessful',self.codes(check))
        check['latest_state']='in_progress'
        self.assertIn('latest_attempt_incomplete',self.codes(check))

    def test_stale_receipt_and_expired_lock_are_not_healthy(self):
        check=self.healthy()
        check.update(receipt_checked_at='2026-10-20T00:00:00+00:00',
                     retention_until='2026-10-24T00:00:00+00:00')
        self.assertEqual(self.codes(check),{'backup_older_than_monitor_threshold','recorded_object_lock_expired'})
        check['receipt_checked_at']='2026-10-26T00:00:00+00:00'
        self.assertIn('backup_older_than_monitor_threshold',self.codes(check))

    def test_missing_or_unverifiable_evidence_stays_unknown(self):
        findings=recovery_coverage.assess({'restore':{'fetch_state':'unavailable'}},NOW)
        self.assertTrue(findings)
        self.assertTrue(all(r['state']=='unknown' for r in findings))
        check=self.healthy();check['receipt_verified']=None
        self.assertIn('verified_receipt_missing_expired_or_invalid',self.codes(check))

    def test_monitor_and_drill_review_boundaries_are_exact_not_rpo_guarantees(self):
        check=self.healthy()
        check['receipt_checked_at']='2026-10-23T12:00:00+00:00'
        self.assertNotIn('backup_older_than_monitor_threshold',self.codes(check))
        check['receipt_checked_at']='2026-10-23T11:59:59+00:00'
        self.assertIn('backup_older_than_monitor_threshold',self.codes(check))
        check['receipt_checked_at']='2026-10-17T00:00:00+00:00'
        findings=recovery_coverage.assess({'restore':check},NOW)
        self.assertNotIn('weekly_drill_overdue',{r['code'] for r in findings})
        check['receipt_checked_at']='2026-10-16T23:59:59+00:00'
        findings=recovery_coverage.assess({'restore':check},NOW)
        self.assertIn('weekly_drill_overdue',{r['code'] for r in findings})

    def test_ledger_digest_authenticates_exact_bytes(self):
        self.assertTrue(recovery_coverage.LEDGER.is_absolute())
        self.assertEqual(recovery_coverage.LEDGER_SHA256,
                         '5b1f6081dc7b5e61229a1be44bca4afc43889b735d6f5201e32c3022c8132be4')
        ledger = recovery_coverage.load_ledger()
        self.assertIsInstance(ledger, dict)
        self.assertEqual(len(ledger['archives']), 41)
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory)/'ledger.json'
            raw = recovery_coverage.LEDGER.read_bytes()
            path.write_bytes(raw)
            self.assertEqual(recovery_coverage.load_ledger(path), ledger)
            path.write_bytes(raw+b'\n')
            with self.assertRaises(ValueError):
                recovery_coverage.load_ledger(path)

    def test_inventory_reads_only_bounded_metadata_and_never_claims_archive_preservation(self):
        ledger = recovery_coverage.load_ledger()
        calls = []
        def check(repository, kind, fetch):
            calls.append((repository, kind, fetch))
            return self.healthy()
        def fetch(*args, **kwargs):
            self.fail('Original Actions archive metadata must not be re-inventoried')
        with patch.object(recovery_coverage, 'collect_check', side_effect=check):
            report = recovery_coverage.collect(fetch, NOW)
        self.assertEqual([row[1] for row in calls], ['backup', 'restore'])
        self.assertTrue(all(row[2] is fetch for row in calls))
        self.assertEqual(report['schema_version'], 2)
        self.assertIs(report['accepted_historical_container_coverage'], True)
        self.assertEqual(report['accepted_historical_gap_count'], 11)
        self.assertEqual(report['summary']['required_container_count'], 41)
        self.assertEqual(report['summary']['earliest_recorded_lock_deadline'],
                         ledger['summary']['earliest_compliance_deadline'])
        self.assertEqual(len(report['archives']), 41)
        by_id = {row['artifact_id']: row for row in report['archives']}
        self.assertEqual(set(by_id), {row['artifact_id'] for row in ledger['archives']})
        for source in ledger['archives']:
            row = by_id[source['artifact_id']]
            self.assertEqual(row['sha256'], source['object']['sha256'])
            self.assertEqual(row['independent_archive_state'], 'historical_exact_version_proof')
            self.assertEqual(row['actions_state'], 'not_assessed')
            self.assertEqual(row['recorded_verified_at'], source['verification']['checked_at'])
            self.assertEqual(row['recorded_retain_until'], source['object']['retain_until'])
            self.assertEqual(row['recorded_lock_state'], 'deadline_future')
        for flag in ('archive_preservation_verified', 'object_store_access',
                     'history_complete', 'notifications_enabled', 'database_access'):
            self.assertIs(report[flag], False, flag)
        self.assertTrue(all(row['owner_issue'] == 2343 for row in report['findings']))
        self.assertNotIn('original_actions_archive_', json.dumps(report['findings']))

    def test_archive_findings_pin_lock_expiry_and_protection_boundaries(self):
        ledger = recovery_coverage.load_ledger()
        source = ledger['archives'][0]
        deadline = datetime.fromisoformat(source['object']['retain_until'])
        for partition, expiry, unproven in (
                ('unknown', None, True),
                ('before', (deadline-timedelta(seconds=1)).isoformat(), False),
                ('equal', deadline.isoformat(), True),
                ('after', (deadline+timedelta(seconds=1)).isoformat(), True)):
            for offset, expired in ((-1, False), (0, True), (1, True)):
                with self.subTest(partition=partition, offset=offset):
                    source['actions_expires_at_from_frozen_inventory'] = expiry
                    with patch.object(recovery_coverage, 'load_ledger', return_value=ledger), \
                         patch.object(recovery_coverage, 'collect_check', return_value=self.healthy()):
                        report = recovery_coverage.collect(now=deadline+timedelta(seconds=offset))
                    row = next(row for row in report['archives']
                               if row['artifact_id'] == source['artifact_id'])
                    self.assertEqual(row['recorded_lock_state'],
                                     'deadline_elapsed' if expired else 'deadline_future')
                    findings = [row for row in report['findings']
                                if row.get('artifact_id') == source['artifact_id']]
                    codes = {row['code'] for row in findings}
                    self.assertEqual(
                        'historical_archive_protection_beyond_actions_expiry_unproven' in codes,
                        unproven)
                    self.assertEqual('recorded_archive_lock_expired' in codes, expired)
                    self.assertTrue(all(row['owner_issue'] == 2343 and
                                        row['state'] == 'attention' for row in findings))

    def test_naive_clock_is_rejected(self):
        with self.assertRaises(ValueError):
            recovery_coverage.collect(now=datetime(2026,10,25))


if __name__=='__main__':
    unittest.main()
