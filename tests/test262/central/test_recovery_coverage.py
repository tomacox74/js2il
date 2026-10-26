from datetime import datetime, timezone
import json
import subprocess
import unittest

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

    def test_inventory_reads_only_bounded_metadata_and_never_claims_archive_preservation(self):
        _,pins,_,_=recovery_coverage.load_manifest(recovery_coverage.DEFAULT_MANIFEST)
        ids=list(pins)
        requests=[]
        def fetch(path,binary=False):
            requests.append(path)
            self.assertFalse(binary)
            self.assertNotIn('/zip',path)
            if 'workflows/' in path:
                return {'workflow_runs':[]}
            artifact_id=int(path.rsplit('/',1)[-1])
            if artifact_id==ids[0]:
                raise subprocess.CalledProcessError(1,['gh'],stderr='PRIVATE')
            return {'id':artifact_id,'digest':'sha256:'+pins[artifact_id],
                    'expired':artifact_id==ids[1],
                    'expires_at':'2027-01-23T00:00:00Z'}
        report=recovery_coverage.collect(fetch,NOW)
        self.assertEqual(len(requests),2+len(pins))
        self.assertEqual(report['archives'][0]['actions_state'],'unavailable')
        self.assertEqual(report['archives'][1]['actions_state'],'expired')
        self.assertEqual(report['archives'][2]['actions_state'],'metadata_available')
        self.assertFalse(report['archive_preservation_verified'])
        self.assertFalse(report['history_complete'])
        self.assertFalse(report['database_access'])
        self.assertNotIn('PRIVATE',json.dumps(report))
        self.assertTrue(all(r['independent_archive_state']=='not_assessed' for r in report['archives']))

    def test_naive_clock_is_rejected(self):
        with self.assertRaises(ValueError):
            recovery_coverage.collect(now=datetime(2026,10,25))


if __name__=='__main__':
    unittest.main()
