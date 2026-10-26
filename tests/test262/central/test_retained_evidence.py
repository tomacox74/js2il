import copy
from datetime import datetime, timedelta, timezone
import hashlib
import io
import json
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch
import zipfile

from scripts.test262.central import retained_evidence as evidence
from scripts.test262.central.cutover_evidence import DEFAULT_MANIFEST, load_manifest


class RetainedEvidenceTests(unittest.TestCase):
    def setUp(self):
        self.pins=json.loads(evidence.PINS.read_text())
        _,artifacts,expected,_=load_manifest(DEFAULT_MANIFEST)
        self.receipt={
            'complete':True,'stage':'complete','history_complete':False,'production_changed':False,
            'checked_at':self.pins['source_checked_at'],'missing_artifact_gaps':11,
            'final_legacy_checkpoint_disposition':'retained-only, not imported',
            'preserved_objects':[{'artifact_id':i,'sha256':sha,'version_id':'version-'+str(i),
                'key':f"private-base/cutover-evidence/{self.pins['run_id']}-1/{i}.zip",
                'retain_until':'2026-11-12T02:13:52+00:00'} for i,sha in artifacts.items()],
            'verified_snapshots':[{'artifact_id':i,'observations':count,
                'outbox':{'messages_observations':count,'pending':0,'integrity':'ok'}}
                for i,count in expected.items()],
            'private_project':'PRIVATE','permission_audit':'PRIVATE'}

    def zip(self,receipt=None):
        body=json.dumps(receipt or self.receipt).encode()
        stream=io.BytesIO()
        with zipfile.ZipFile(stream,'w',zipfile.ZIP_DEFLATED) as archive:
            archive.writestr('evidence-report.json',body)
        raw=stream.getvalue()
        pins=dict(self.pins,zip_sha256=hashlib.sha256(raw).hexdigest(),
                  report_sha256=hashlib.sha256(body).hexdigest())
        return raw,pins

    def test_legacy_receipt_has_unknown_current_readback_and_omits_private_fields(self):
        raw,pins=self.zip()
        receipt,report=evidence.inspect_receipt(raw,pins)
        self.assertEqual(len(receipt['preserved_objects']),8)
        self.assertTrue(report['historical_receipt_verified'])
        self.assertFalse(report['selected_object_verified'])
        self.assertFalse(report['full_archive_coverage_verified'])
        self.assertFalse(report['history_complete'])
        self.assertEqual(report['missing_artifact_gaps'],11)
        self.assertNotIn('PRIVATE',json.dumps(report))
        self.assertNotIn('private-base',json.dumps(report))
        self.assertNotIn('encryption',report['historical_objects'][0])

    def test_tampered_bytes_false_success_and_incomplete_receipts_fail(self):
        raw,pins=self.zip()
        with self.assertRaises(ValueError):
            evidence.inspect_receipt(raw+b'changed',pins)
        for mutate in (
                lambda r:r.update(complete=False),
                lambda r:r.update(history_complete=True),
                lambda r:r['preserved_objects'].pop(),
                lambda r:r['preserved_objects'].append(r['preserved_objects'][0]),
                lambda r:r['preserved_objects'][0].update(sha256='0'*64),
                lambda r:r['verified_snapshots'][0]['outbox'].update(pending=1)):
            document=copy.deepcopy(self.receipt);mutate(document)
            with self.subTest(mutate=mutate),self.assertRaises(ValueError):
                raw,pins=self.zip(document);evidence.inspect_receipt(raw,pins)

    def test_current_readback_pins_original_version_and_is_read_only(self):
        target=11522307991
        row=next(r for r in self.receipt['preserved_objects'] if r['artifact_id']==target)
        data=b'retained exact bytes';row['sha256']=hashlib.sha256(data).hexdigest()
        s3=Mock()
        s3.get_bucket_versioning.return_value={'Status':'Enabled'}
        s3.get_object_lock_configuration.return_value={'ObjectLockConfiguration':{
            'ObjectLockEnabled':'Enabled','Rule':{'DefaultRetention':{'Mode':'COMPLIANCE','Days':35}}}}
        s3.head_object.return_value={'VersionId':row['version_id'],'ContentLength':len(data),
                                    'ServerSideEncryption':'aws:kms','SSEKMSKeyId':'PRIVATE-kms'}
        s3.get_object_retention.return_value={'Retention':{
            'Mode':'COMPLIANCE','RetainUntilDate':datetime.now(timezone.utc)+timedelta(days=35)}}
        s3.get_object.side_effect=lambda **kw:{'VersionId':kw['VersionId'],'Body':io.BytesIO(data)}
        proof=evidence.verify_selected(s3,'s3://private-bucket/private-base','PRIVATE-kms',
                                       self.receipt,self.pins,target)
        self.assertEqual(proof['sha256'],row['sha256'])
        self.assertEqual(proof['content_length'],len(data))
        self.assertNotIn('PRIVATE',json.dumps(proof))
        for call in s3.head_object.call_args_list:
            self.assertEqual(call.kwargs['VersionId'],row['version_id'])
        self.assertEqual(s3.get_object.call_args.kwargs['VersionId'],row['version_id'])
        self.assertEqual({call[0] for call in s3.mock_calls},
            {'get_bucket_versioning','get_object_lock_configuration','head_object',
             'get_object_retention','get_object'})
        row['retain_until']=(datetime.now(timezone.utc)+timedelta(days=40)).isoformat()
        with self.assertRaises(ValueError):
            evidence.verify_selected(s3,'s3://private-bucket/private-base','PRIVATE-kms',
                                     self.receipt,self.pins,target)
        row['retain_until']='2026-11-12T02:13:52+00:00'
        row['sha256']='0'*64
        with self.assertRaises(ValueError):
            evidence.verify_selected(s3,'s3://private-bucket/private-base','PRIVATE-kms',
                                     self.receipt,self.pins,target)

    def test_unknown_artifact_or_wrong_destination_is_rejected_before_s3(self):
        s3=Mock()
        for target,uri in ((999,'s3://bucket/private-base'),(11522307991,'s3://bucket/wrong-base')):
            with self.subTest(target=target),self.assertRaises(ValueError):
                evidence.verify_selected(s3,uri,'key',self.receipt,self.pins,target)
        self.assertFalse(s3.mock_calls)

    def test_source_metadata_requires_exact_run_attempt_and_revision(self):
        meta={'id':self.pins['artifact_id'],'expired':False,'size_in_bytes':123,
              'digest':'sha256:'+self.pins['zip_sha256'],
              'workflow_run':{'id':self.pins['run_id'],'head_sha':self.pins['source_revision']}}
        run={'head_sha':self.pins['source_revision'],'run_attempt':1,'head_branch':'master',
             'conclusion':'success','status':'completed','path':'.github/workflows/test262-cutover-evidence.yml'}
        fetch=Mock(side_effect=[run,meta,b'ZIP'])
        self.assertEqual(evidence.fetch_receipt(self.pins,fetch),b'ZIP')
        run['run_attempt']=2
        fetch=Mock(return_value=run)
        with self.assertRaises(ValueError):evidence.fetch_receipt(self.pins,fetch)
        self.assertEqual(fetch.call_count,1)

    def test_cli_failure_overwrites_stale_success_without_private_error_text(self):
        with tempfile.TemporaryDirectory() as directory:
            path=Path(directory)/'report.json';path.write_text('{"selected_object_verified":true}')
            with patch.object(evidence,'fetch_receipt',side_effect=ValueError('PRIVATE details')):
                self.assertEqual(evidence.main(['--output',str(path)]),1)
            report=json.loads(path.read_text())
            self.assertFalse(report['selected_object_verified'])
            self.assertEqual(report['failed_stage'],'receipt')
            self.assertNotIn('PRIVATE',json.dumps(report))

    def test_offline_receipt_mode_never_accesses_aws_or_github(self):
        raw,pins=self.zip()
        client=Mock()
        with tempfile.TemporaryDirectory() as directory:
            source=Path(directory)/'receipt.zip';source.write_bytes(raw)
            pinfile=Path(directory)/'pins.json';pinfile.write_text(json.dumps(pins))
            output=Path(directory)/'report.json'
            with patch.object(evidence,'PINS',pinfile), \
                 patch.object(evidence,'fetch_receipt',side_effect=AssertionError('Network access')), \
                 patch.dict('sys.modules',{'boto3':SimpleNamespace(client=client)}):
                self.assertEqual(evidence.main(['--receipt-zip',str(source),'--receipt-only',
                                                '--output',str(output)]),0)
            report=json.loads(output.read_text())
            self.assertTrue(report['historical_receipt_verified'])
            self.assertFalse(report['selected_object_verified'])
            self.assertFalse(report['source_metadata_verified'])
            client.assert_not_called()

    def test_workflow_preserves_master_guard_and_restricts_verification_session(self):
        root=Path(__file__).resolve().parents[3]
        workflow=(root/'.github/workflows/test262-cutover-evidence.yml').read_text()
        self.assertIn("if: github.ref == 'refs/heads/master'",workflow)
        self.assertIn("if: inputs.operation == 'preserve'",workflow)
        self.assertIn("if: inputs.operation == 'verify-retained'",workflow)
        self.assertIn('Unsupported evidence operation',workflow)
        import re
        policy=json.loads(re.search(r"'(\{\"Version\".*?\})' \|\|",workflow).group(1))
        actions=set(policy['Statement'][0]['Action'])
        self.assertEqual(actions,{'s3:GetBucketVersioning','s3:GetBucketObjectLockConfiguration',
                                 's3:GetObjectVersion','s3:GetObjectRetention','kms:Decrypt'})


if __name__=='__main__':
    unittest.main()
