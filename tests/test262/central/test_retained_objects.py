from datetime import datetime, timedelta, timezone
import hashlib
import io
import json
import os
from pathlib import Path
from types import SimpleNamespace
import tempfile
import unittest
from unittest.mock import Mock, patch

from scripts.test262.central import backup, retained_objects, retained_restore


class RetainedObjectTests(unittest.TestCase):
    def setUp(self):
        self.s3=Mock()
        self.data=b'retained snapshot'
        self.s3.head_object.return_value={'VersionId':'immutable-v1','ContentLength':len(self.data),
            'ServerSideEncryption':'aws:kms','SSEKMSKeyId':'kms-key'}
        self.s3.get_object_retention.return_value={'Retention':{'Mode':'COMPLIANCE',
            'RetainUntilDate':datetime.now(timezone.utc)+timedelta(days=35)}}
        self.s3.get_object.side_effect=lambda **k:{'VersionId':k['VersionId'],'Body':io.BytesIO(self.data)}
        self.s3.get_bucket_versioning.return_value={'Status':'Enabled'}
        self.s3.get_object_lock_configuration.return_value={'ObjectLockConfiguration':{
            'ObjectLockEnabled':'Enabled','Rule':{'DefaultRetention':{'Mode':'COMPLIANCE','Days':35}}}}

    def verify(self,**kwargs):
        return retained_objects.verify_object(self.s3,'bucket','prefix/dump','kms-key',**kwargs)

    def test_reads_pinned_version_even_when_latest_key_changes(self):
        pin={'version_id':'immutable-v1','content_length':len(self.data),'sha256':hashlib.sha256(self.data).hexdigest()}
        result=self.verify(pinned=pin)
        self.assertEqual(result['sha256'],pin['sha256'])
        self.s3.head_object.assert_called_once_with(Bucket='bucket',Key='prefix/dump',VersionId='immutable-v1')
        self.s3.get_object.assert_called_once_with(Bucket='bucket',Key='prefix/dump',VersionId='immutable-v1')

    def test_rejects_missing_version_wrong_encryption_or_expired_lock_before_download(self):
        for change in ({'VersionId':'null'},{'SSEKMSKeyId':'other'},{'ServerSideEncryption':'AES256'}):
            with self.subTest(change=change),patch.dict(self.s3.head_object.return_value,change):
                with self.assertRaises(ValueError):self.verify()
        self.s3.get_object_retention.return_value['Retention']['RetainUntilDate']=datetime.now(timezone.utc)-timedelta(seconds=1)
        with self.assertRaises(ValueError):self.verify()
        self.s3.get_object.assert_not_called()

    def test_corrupt_bytes_wrong_length_or_version_do_not_verify(self):
        pin={'version_id':'immutable-v1','content_length':len(self.data),'sha256':'0'*64}
        with self.assertRaises(ValueError):self.verify(pinned=pin)
        self.s3.head_object.return_value['ContentLength']+=1
        with self.assertRaises(ValueError):self.verify()
        self.s3.get_object.side_effect=lambda **k:{'VersionId':'other','Body':io.BytesIO(self.data)}
        with self.assertRaises(ValueError):self.verify()

    def test_governance_or_short_default_retention_is_not_accepted(self):
        retained_objects.verify_bucket(self.s3,'bucket')
        policy=self.s3.get_object_lock_configuration.return_value['ObjectLockConfiguration']['Rule']['DefaultRetention']
        for change in ({'Mode':'GOVERNANCE'},{'Days':34}):
            with patch.dict(policy,change):
                with self.assertRaises(ValueError):retained_objects.verify_bucket(self.s3,'bucket')

    def receipt(self):
        prefix='backups/2026/10/08/123-1'
        return {'schema_version':1,'backup_verified':True,'bucket':'bucket','prefix':prefix,
                'checked_at':datetime.now(timezone.utc).isoformat(),
                'objects':{'catalogue.dump':{},'manifest.json':{}}}

    def test_selects_paginated_latest_completed_receipt_and_records_exact_version(self):
        now=datetime.now(timezone.utc);receipt=self.receipt()
        self.s3.get_paginator.return_value.paginate.return_value=[{'Contents':[
            {'Key':'backups/2026/10/07/122-1/backup-report.json','LastModified':now-timedelta(days=1)},
            {'Key':'backups/2026/10/08/123-1/catalogue.dump','LastModified':now}]},
            {'Contents':[{'Key':receipt['prefix']+'/backup-report.json','LastModified':now}]}]
        self.data=json.dumps(receipt).encode();self.s3.head_object.return_value['ContentLength']=len(self.data)
        with tempfile.TemporaryDirectory() as output:
            prefix,found,proof=retained_objects.latest_receipt(self.s3,'bucket','backups','kms-key',output)
        self.assertEqual(prefix,receipt['prefix']);self.assertEqual(found,receipt)
        self.assertEqual(proof['version_id'],'immutable-v1')

    def test_broken_latest_receipt_never_falls_back_to_old_success(self):
        now=datetime.now(timezone.utc)
        self.s3.get_paginator.return_value.paginate.return_value=[{'Contents':[
            {'Key':'backups/2026/10/08/123-1/backup-report.json','LastModified':now}]}]
        receipt=self.receipt();receipt['backup_verified']=False
        self.data=json.dumps(receipt).encode();self.s3.head_object.return_value['ContentLength']=len(self.data)
        with tempfile.TemporaryDirectory() as output:
            with self.assertRaises(ValueError):retained_objects.latest_receipt(self.s3,'bucket','backups','kms-key',output)
        self.s3.get_object.assert_called_once()

    def test_stale_backup_receipt_is_rejected_before_fetch(self):
        self.s3.get_paginator.return_value.paginate.return_value=[{'Contents':[
            {'Key':'backups/2026/10/08/123-1/backup-report.json','LastModified':datetime.now(timezone.utc)-timedelta(hours=49)}]}]
        with tempfile.TemporaryDirectory() as output:
            with self.assertRaises(ValueError):retained_objects.latest_receipt(self.s3,'bucket','backups','kms-key',output)
        self.s3.get_object.assert_not_called()

    def test_failed_backup_readback_never_publishes_success_receipt(self):
        with tempfile.TemporaryDirectory() as output:
            archive=Path(output)/'catalogue.dump';archive.write_bytes(self.data)
            manifest=Path(output)/'manifest.json';manifest.write_text('{}')
            args=SimpleNamespace(output=output,destination='s3://bucket/backups/2026/10/08/123-1',kms_key='kms-key')
            with patch.dict('sys.modules',{'boto3':SimpleNamespace(client=lambda *a:self.s3)}), \
                 patch.object(backup,'create_snapshot',return_value=(archive,manifest)), \
                 patch.object(retained_objects,'verify_object',side_effect=ValueError('PRIVATE server diagnostic')):
                with self.assertRaises(ValueError):backup.backup(args)
            report=json.loads((Path(output)/'backup-report.json').read_text())
        self.assertFalse(report['backup_verified']);self.assertNotIn('PRIVATE',json.dumps(report))
        self.assertEqual([call.args[2] for call in self.s3.upload_file.call_args_list],['backups/2026/10/08/123-1/catalogue.dump'])

    def test_successful_backup_publishes_receipt_after_both_verified_objects(self):
        with tempfile.TemporaryDirectory() as output:
            archive=Path(output)/'catalogue.dump';archive.write_bytes(self.data)
            manifest=Path(output)/'manifest.json';manifest.write_text('{}')
            args=SimpleNamespace(output=output,destination='s3://bucket/backups/2026/10/08/123-1',kms_key='kms-key')
            def proof(s3,bucket,key,kms,**kwargs):
                file=Path(output)/key.split('/')[-1]
                return {'version_id':'version-'+file.name,'sha256':hashlib.sha256(file.read_bytes()).hexdigest(),
                        'content_length':file.stat().st_size}
            with patch.dict('sys.modules',{'boto3':SimpleNamespace(client=lambda *a:self.s3)}), \
                 patch.object(backup,'create_snapshot',return_value=(archive,manifest)), \
                 patch.object(retained_objects,'verify_object',side_effect=proof):
                backup.backup(args)
            report=json.loads((Path(output)/'backup-report.json').read_text())
        self.assertTrue(report['backup_verified'])
        self.assertEqual(report['objects']['catalogue.dump']['version_id'],'version-catalogue.dump')
        self.assertEqual([call.args[2].split('/')[-1] for call in self.s3.upload_file.call_args_list],
                         ['catalogue.dump','manifest.json','backup-report.json'])

    def test_restore_consumes_receipt_versions_and_checks_fencing_without_production_dsn(self):
        manifest={'sha256':hashlib.sha256(self.data).hexdigest(),
                  'contract':{'minimum_writer_epoch':2},'row_counts':{'observations':3},'view_counts':{'coverage':1}}
        receipt=self.receipt();receipt['objects']={'catalogue.dump':{'version_id':'dump-v1'},
                                                   'manifest.json':{'version_id':'manifest-v2'}}
        before=Mock();before.execute.return_value.fetchone.side_effect=[('170000',),(False,)]
        after=Mock();after.execute.return_value.fetchone.side_effect=[('shadow',3),(None,)]
        def object_proof(s3,bucket,key,kms,*,pinned,output):
            self.assertEqual(pinned,receipt['objects'][Path(output).name])
            data=self.data if Path(output).name=='catalogue.dump' else json.dumps(manifest).encode()
            Path(output).write_bytes(data)
            return {'version_id':pinned['version_id'],'sha256':hashlib.sha256(data).hexdigest()}
        with tempfile.TemporaryDirectory() as output, \
             patch.dict('sys.modules',{'boto3':SimpleNamespace(client=lambda *a:self.s3)}), \
             patch.dict(os.environ,{'TEST262_RESTORE_DATABASE_URL':'host=127.0.0.1 dbname=catalogue_retained_restore',
                                  'TEST262_BACKUP_S3_URI':'s3://bucket/backups','TEST262_BACKUP_KMS_KEY':'kms-key'}), \
             patch('psycopg.connect') as connect, \
             patch.object(retained_objects,'latest_receipt',return_value=(receipt['prefix'],receipt,{'version_id':'receipt-v3'})), \
             patch.object(retained_objects,'verify_object',side_effect=object_proof), \
             patch.object(retained_restore.subprocess,'run',return_value=SimpleNamespace(stdout='{"restore_verified":true}\n')) as restore:
            connect.return_value.__enter__.side_effect=[before,after]
            self.assertEqual(retained_restore.verify(SimpleNamespace(output=output,backup_suffix='')),0)
            report=json.loads((Path(output)/'restore-report.json').read_text())
            self.assertTrue(report['restore_verified']);self.assertEqual(report['restored_epoch'],3)
            self.assertEqual(report['receipt']['version_id'],'receipt-v3')
            self.assertFalse((Path(output)/'catalogue.dump').exists())
            self.assertTrue(restore.call_args.kwargs['capture_output'])
            self.assertFalse(report['production_cutover'])

    def test_restore_rejects_production_and_hostaddr_override_before_connect(self):
        for dsn in ('host=production.example dbname=catalogue_retained_restore',
                    'host=127.0.0.1 hostaddr=10.0.0.1 dbname=catalogue_retained_restore',
                    'host=127.0.0.1 dbname=postgres'):
            with self.subTest(dsn=dsn),tempfile.TemporaryDirectory() as output, \
                 patch.dict('sys.modules',{'boto3':SimpleNamespace(client=lambda *a:self.s3)}), \
                 patch.dict(os.environ,{'TEST262_RESTORE_DATABASE_URL':dsn}),patch('psycopg.connect') as connect:
                self.assertEqual(retained_restore.verify(SimpleNamespace(output=output,backup_suffix='')),1)
                connect.assert_not_called()
                self.s3.get_object.assert_not_called()
