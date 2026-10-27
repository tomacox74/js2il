"""Offline contract tests for the authorized, bounded #2242 preservation."""
from contextlib import ExitStack, redirect_stderr, redirect_stdout
import copy
from datetime import datetime, timezone
import hashlib
import io
import json
import os
from pathlib import Path
import re
import subprocess
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch
import zipfile

from scripts.test262.central import archive_inventory as inventory
from scripts.test262.central import recovery_preservation as recovery
from scripts.test262.central import retained_discovery


FROZEN_SHA = 'ec75aea22258310fbb80f33b5c2f5c853e8be4575bf5efee565ca9ab90268bb7'
PRIVATE = 'PRIVATE-do-not-publish'


def sha(raw):
    return hashlib.sha256(raw).hexdigest()


def aws_error(code='AccessDenied'):
    error = RuntimeError(f'{PRIVATE} credentials=secret-token s3://{PRIVATE}/key')
    error.response = {'Error': {'Code': code, 'Message': str(error)}}
    return error


class RecoveryPreservationTests(unittest.TestCase):
    def setUp(self):
        self.stack = ExitStack()
        self.addCleanup(self.stack.close)
        self.directory = Path(self.stack.enter_context(tempfile.TemporaryDirectory()))
        self.original_raw = retained_discovery.INVENTORY.read_bytes()
        self.document = json.loads(self.original_raw)
        self.sources = [row for row in self.document['archives']
                        if row['required_for_scope'] and row['retained_object_artifact_id'] is None]
        self.assertEqual(len(self.sources), 33)
        self.zip_bytes = {}
        history = self.document['original_history']
        historical_sources = {row['artifact_id']: row for row in history['manifest']['artifacts']}
        for row in self.sources:
            imported = row['import_report']
            report = {
                'complete': imported['complete'], 'history_complete': False,
                'manifest_sha256': history['manifest_sha256'],
                'gaps': history['manifest']['gaps'],
                'selected_artifact_ids': imported['selected_artifact_ids'], 'snapshots': [],
            }
            for snapshot in imported['snapshots']:
                original = historical_sources[snapshot['artifact_id']]
                database = original['databases'][0]
                count = snapshot.get('verified_observation_mappings', 0)
                report['snapshots'].append(dict(
                    snapshot, sha256=database['sha256'], source_uri=database['source_uri'],
                    archive_sha256=original['archive_sha256'], result={'observations': count}))
            if 'error_type' in imported:
                report['error_type'] = imported['error_type']
            report_raw = inventory.encoded(report)
            payloads = {member['name']: (
                report_raw if member['name'] == 'import-report.json'
                else f"opaque evidence {row['artifact_id']} {member['name']}\x00\xff".encode())
                for member in row['members']}
            stream = io.BytesIO()
            with zipfile.ZipFile(stream, 'w', zipfile.ZIP_DEFLATED) as archive:
                for name, body in payloads.items():
                    archive.writestr(zipfile.ZipInfo(name, (2026, 10, 1, 0, 0, 0)), body)
            raw = stream.getvalue()
            self.zip_bytes[row['artifact_id']] = raw
            with zipfile.ZipFile(io.BytesIO(raw)) as archive:
                row['members'] = inventory.members(archive)
            row.update(zip_sha256=sha(raw), size_in_bytes=len(raw),
                       member_index_sha256=sha(inventory.encoded(row['members'])),
                       import_report=dict(inventory.project_import(
                           report, history['manifest'], history['manifest_sha256']),
                           sha256=sha(report_raw)))
        self.inventory_path = self.directory/'inventory.json'
        self.inventory_path.write_bytes(inventory.encoded(self.document))
        self.s3 = Mock()
        self.stored = {}
        self.uploads = []
        self.reads = []
        self.events = []
        self.readback_failure = None
        self.upload_failure = None
        self.s3.put_object.side_effect = self.put_object
        self.s3.get_bucket_versioning.side_effect = self.bucket_versioning
        self.s3.get_object_lock_configuration.return_value = {'ObjectLockConfiguration': {
            'ObjectLockEnabled': 'Enabled',
            'Rule': {'DefaultRetention': {'Mode': 'COMPLIANCE', 'Days': 35}}}}
        self.s3.head_object.side_effect = self.head_object
        self.s3.get_object_retention.side_effect = self.retention
        self.s3.get_object.side_effect = self.get_object
        self.fetch = Mock(side_effect=self.fetch_source)
        self.download = Mock(side_effect=self.download_source)
        self.client_config = Mock(return_value=SimpleNamespace(retries={'total_max_attempts': 1}))

    def synthetic_inventory(self):
        self.stack.enter_context(patch.object(recovery, 'INVENTORY', self.inventory_path))
        self.stack.enter_context(patch.object(
            recovery, 'INVENTORY_SHA256', sha(self.inventory_path.read_bytes())))
        # Only the frozen inventory projection is synthetic; the 33-row guard,
        # source validation, ZIP authentication and S3 verifier remain real.
        self.stack.enter_context(patch.object(recovery, 'validate', return_value=None))
        return self.new_report(self.inventory_path.read_bytes())

    def new_report(self, raw=None):
        report = recovery.initial_report(self.original_raw if raw is None else raw)
        report.update(run_id=123456, run_attempt=2, source_revision='a'*40)
        return report

    def metadata(self, row):
        return {
            'id': row['artifact_id'], 'expired': False,
            'digest': 'sha256:'+row['zip_sha256'], 'size_in_bytes': row['size_in_bytes'],
            'workflow_run': {'id': row['run_id'], 'head_sha': row['source_revision'],
                             'head_branch': 'master'},
        }

    def run_metadata(self, row):
        return {
            'id': row['run_id'], 'head_sha': row['source_revision'],
            'head_branch': 'master', 'status': 'completed', 'run_attempt': 1,
            'conclusion': 'failure' if not row['import_report']['complete'] else 'success',
            'path': '.github/workflows/test262-history-import.yml',
        }

    def fetch_source(self, endpoint, **kwargs):
        self.assertFalse(kwargs.get('binary', False), 'Metadata fetch must never fetch ZIPs')
        self.assertFalse(self.uploads, 'All metadata must be checked before any uploads')
        kind, identity = endpoint.rsplit('/', 2)[-2:]
        if kind == 'artifacts':
            row = next(row for row in self.sources if row['artifact_id'] == int(identity))
            return self.metadata(row)
        self.assertEqual(kind, 'runs')
        row = next(row for row in self.sources if row['run_id'] == int(identity))
        return self.run_metadata(row)

    def download_source(self, artifact_id, path):
        self.assertFalse(self.uploads, 'Every ZIP must be checked before any uploads')
        self.events.append(('download', artifact_id))
        Path(path).write_bytes(self.zip_bytes[artifact_id])

    def bucket_versioning(self, **kwargs):
        self.assertEqual(self.download.call_count, 33)
        self.events.append(('bucket', kwargs['Bucket']))
        return {'Status': 'Enabled'}

    def put_object(self, **kwargs):
        key = kwargs['Key']
        if self.upload_failure and self.upload_failure(key):
            raise aws_error()
        raw = kwargs['Body'].read()
        self.assertEqual(kwargs['Bucket'], PRIVATE)
        self.assertEqual(kwargs['ServerSideEncryption'], 'aws:kms')
        self.assertEqual(kwargs['SSEKMSKeyId'], PRIVATE+'-kms')
        self.assertEqual(set(kwargs),
                         {'Bucket', 'Key', 'Body', 'ServerSideEncryption', 'SSEKMSKeyId'})
        version = 'uploaded-version-'+str(len(self.uploads)+1)
        self.stored[(key, version)] = raw
        self.uploads.append((key, version, raw))
        self.events.append(('upload', key))
        return {'VersionId': version}

    def requested_bytes(self, kwargs):
        # "latest" is deliberately unusable. Every head/get must pin the
        # VersionId returned by put_object, even when another version is newer.
        self.assertIn('VersionId', kwargs)
        self.assertEqual(kwargs['Bucket'], PRIVATE)
        identity = (kwargs['Key'], kwargs['VersionId'])
        self.assertIn(identity, self.stored)
        return self.stored[identity]

    def head_object(self, **kwargs):
        raw = self.requested_bytes(kwargs)
        self.reads.append(kwargs)
        if self.readback_failure and self.readback_failure(kwargs['Key']):
            raise aws_error()
        return {'VersionId': kwargs['VersionId'], 'ContentLength': len(raw),
                'ServerSideEncryption': 'aws:kms', 'SSEKMSKeyId': PRIVATE+'-kms'}

    def retention(self, **kwargs):
        self.requested_bytes(kwargs)
        return {'Retention': {'Mode': 'COMPLIANCE',
                              'RetainUntilDate': datetime(2099, 1, 1, tzinfo=timezone.utc)}}

    def get_object(self, **kwargs):
        raw = self.requested_bytes(kwargs)
        return {'VersionId': kwargs['VersionId'], 'Body': io.BytesIO(raw)}

    def preserve(self, report):
        recovery.preserve(self.s3, f's3://{PRIVATE}/{PRIVATE}', PRIVATE+'-kms', report,
                          fetch=self.fetch, download=self.download)

    def assert_not_complete(self, report):
        self.assertFalse(report['complete'])
        self.assertFalse(report['receipt_verified'])
        for flag in ('history_complete', 'full_archive_coverage_verified', 'database_access',
                     'policy_changes', 'imports', 'restores', 'fixture_execution'):
            self.assertIs(report[flag], False, flag)

    def assert_public(self, report):
        encoded = json.dumps(report)
        for value in (PRIVATE, 's3://', 'secret-token', 'Traceback', 'credentials='):
            self.assertNotIn(value, encoded)

    def test_frozen_inventory_and_exact_scope_are_required_before_storage(self):
        report = self.new_report()
        original = json.loads(self.original_raw)
        rows = [row for row in original['archives']
                if row['required_for_scope'] and row['retained_object_artifact_id'] is None]
        self.assertEqual([row['artifact_id'] for row in report['archives']],
                         [row['artifact_id'] for row in rows])
        self.assertEqual(sum(not row['original_import_report_complete']
                             for row in report['archives']), 4)
        self.assertEqual(report['inventory_sha256'], FROZEN_SHA)
        self.assertEqual(report['mode'], 'preserve-recovery')
        self.assertEqual(report['objects_uploaded'], 0)
        self.assertTrue(all(row['state'] == 'not_assessed' for row in report['archives']))
        self.assert_not_complete(report)
        for public, source in zip(report['archives'], rows):
            for public_key, source_key in (
                    ('source_run_id', 'run_id'), ('source_revision', 'source_revision'),
                    ('expected_zip_sha256', 'zip_sha256'),
                    ('expected_size_in_bytes', 'size_in_bytes')):
                self.assertEqual(public[public_key], source[source_key])
        with self.assertRaises(ValueError):
            recovery.initial_report(self.original_raw+b'\n')
        for change in ('remove', 'duplicate', 'classification', 'mapping'):
            with self.subTest(change=change):
                document = copy.deepcopy(original)
                target = next(row for row in document['archives']
                              if row['artifact_id'] == rows[0]['artifact_id'])
                if change == 'remove':
                    document['archives'].remove(target)
                elif change == 'duplicate':
                    document['archives'].append(copy.deepcopy(target))
                elif change == 'classification':
                    target['classification'] = 'planning_only'
                else:
                    target['retained_object_artifact_id'] = target['artifact_id']
                raw = inventory.encoded(document)
                with patch.object(recovery, 'INVENTORY_SHA256', sha(raw)), \
                     patch.object(recovery, 'validate', return_value=None), \
                     self.assertRaises(ValueError):
                    recovery.initial_report(raw)
        self.assertEqual(self.s3.mock_calls, [])

    def test_source_metadata_rejects_each_provenance_mismatch_but_accepts_failed_run(self):
        source = self.sources[0]
        metadata, run = self.metadata(source), self.run_metadata(source)
        self.assertEqual(run['conclusion'], 'failure')
        recovery.source_metadata(source, metadata, run)
        changes = (
            ('artifact', 'id', source['artifact_id']+1),
            ('artifact', 'digest', 'sha256:'+'0'*64),
            ('artifact', 'size_in_bytes', source['size_in_bytes']+1),
            ('artifact', 'expired', True), ('artifact', 'expired', None),
            ('artifact', 'expired', 0),
            ('nested', 'id', source['run_id']+1),
            ('nested', 'head_sha', '0'*40), ('nested', 'head_branch', 'topic'),
            ('run', 'id', source['run_id']+1), ('run', 'head_sha', '0'*40),
            ('run', 'head_branch', 'topic'), ('run', 'status', 'in_progress'),
            ('run', 'run_attempt', 2), ('run', 'path', '.github/workflows/other.yml'),
        )
        for kind, field, value in changes:
            with self.subTest(kind=kind, field=field, value=value):
                meta, current = copy.deepcopy(metadata), copy.deepcopy(run)
                target = meta if kind == 'artifact' else meta['workflow_run'] if kind == 'nested' else current
                target[field] = value
                with self.assertRaises(ValueError):
                    recovery.source_metadata(source, meta, current)
        self.assertEqual(self.s3.mock_calls, [])

    def test_zip_authenticates_full_bytes_length_members_index_and_sidecars(self):
        source = self.sources[0]
        path = self.directory/'archive.zip'
        raw = self.zip_bytes[source['artifact_id']]
        path.write_bytes(raw)
        recovery.verify_zip(path, source)
        for partition in ('sha', 'length', 'members', 'index', 'sidecar',
                          'report_digest', 'report_complete'):
            with self.subTest(partition=partition):
                pin = copy.deepcopy(source)
                path.write_bytes(raw)
                if partition == 'sha':
                    pin['zip_sha256'] = '0'*64
                elif partition == 'length':
                    pin['size_in_bytes'] += 1
                elif partition == 'members':
                    pin['members'] = pin['members'][:-1]
                    pin['member_index_sha256'] = sha(inventory.encoded(pin['members']))
                elif partition == 'index':
                    pin['member_index_sha256'] = '0'*64
                elif partition == 'report_digest':
                    pin['import_report']['sha256'] = '0'*64
                elif partition == 'report_complete':
                    pin['import_report']['complete'] = not pin['import_report']['complete']
                elif partition == 'sidecar':
                    with zipfile.ZipFile(io.BytesIO(raw)) as archive:
                        contents = {name: archive.read(name) for name in archive.namelist()}
                    sidecar = next(name for name in contents if name.endswith('-wal'))
                    contents[sidecar] += b'tampered'
                    with zipfile.ZipFile(path, 'w') as archive:
                        for name, body in contents.items():
                            archive.writestr(name, body)
                    # Repin outer bytes to isolate member authentication.
                    pin.update(zip_sha256=sha(path.read_bytes()), size_in_bytes=path.stat().st_size)
                with self.assertRaises(ValueError):
                    recovery.verify_zip(path, pin)
        self.assertEqual(self.s3.mock_calls, [])

    def test_all_sources_are_preflighted_before_any_bucket_or_upload_call(self):
        report = self.synthetic_inventory()
        last = self.sources[-1]
        for partition in ('metadata', 'zip'):
            with self.subTest(partition=partition):
                current = copy.deepcopy(report)
                if partition == 'metadata':
                    def fetch(endpoint, **kwargs):
                        result = self.fetch_source(endpoint, **kwargs)
                        if endpoint.endswith('/artifacts/'+str(last['artifact_id'])):
                            result['digest'] = 'sha256:'+'0'*64
                        return result
                    self.fetch.side_effect = fetch
                else:
                    self.fetch.side_effect = self.fetch_source
                    self.zip_bytes[last['artifact_id']] += b'tamper'
                with self.assertRaises(ValueError):
                    self.preserve(current)
                self.assert_not_complete(current)
                self.assertEqual(current['objects_uploaded'], 0)
                self.assertEqual(self.s3.mock_calls, [])
        for partition in ('count_below', 'count_above', 'duplicate', 'pin',
                          'length', 'run', 'revision', 'complete', 'inventory', 'already_attempted'):
            with self.subTest(partition=partition):
                current = copy.deepcopy(report)
                if partition == 'count_below':
                    current['archives'].pop()
                elif partition == 'count_above':
                    current['archives'].append(copy.deepcopy(current['archives'][0]))
                elif partition == 'duplicate':
                    current['archives'][-1] = copy.deepcopy(current['archives'][0])
                elif partition == 'pin':
                    current['archives'][0]['expected_zip_sha256'] = '0'*64
                elif partition == 'length':
                    current['archives'][0]['expected_size_in_bytes'] += 1
                elif partition == 'run':
                    current['archives'][0]['source_run_id'] += 1
                elif partition == 'revision':
                    current['archives'][0]['source_revision'] = '0'*40
                elif partition == 'complete':
                    current['archives'][0]['original_import_report_complete'] = True
                elif partition == 'inventory':
                    current['inventory_sha256'] = '0'*64
                else:
                    current['archives'][0]['state'] = 'uploaded_pending_readback'
                with self.assertRaises(ValueError):
                    self.preserve(current)
                self.assertEqual(self.s3.mock_calls, [])

    def test_changed_frozen_inventory_is_rechecked_before_fetch_or_writes(self):
        report = self.synthetic_inventory()
        self.inventory_path.write_bytes(self.inventory_path.read_bytes()+b'\n')
        with self.assertRaises(ValueError):
            self.preserve(report)
        self.fetch.assert_not_called()
        self.download.assert_not_called()
        self.assertEqual(self.s3.mock_calls, [])
        self.assert_not_complete(report)

    def test_member_or_sidecar_tampering_is_rejected_by_preserve_before_writes(self):
        template = self.synthetic_inventory()
        source = self.sources[-1]
        original = self.zip_bytes[source['artifact_id']]
        for partition in ('same_length_sha', 'member', 'sidecar'):
            with self.subTest(partition=partition):
                report = copy.deepcopy(template)
                if partition == 'same_length_sha':
                    damaged = bytearray(original)
                    damaged[-1] ^= 1
                    self.zip_bytes[source['artifact_id']] = bytes(damaged)
                else:
                    stream = io.BytesIO()
                    with zipfile.ZipFile(io.BytesIO(original)) as archive, \
                         zipfile.ZipFile(stream, 'w') as changed:
                        for name in archive.namelist():
                            body = archive.read(name)
                            if partition == 'sidecar' and name.endswith('-wal'):
                                body += b'changed WAL'
                            if partition == 'member' and name == 'import-report.json':
                                continue
                            changed.writestr(name, body)
                    self.zip_bytes[source['artifact_id']] = stream.getvalue()
                with self.assertRaises(ValueError):
                    self.preserve(report)
                self.assert_not_complete(report)
                self.assertEqual(report['stage'], 'download')
                self.assertEqual(report['objects_uploaded'], 0)
                self.assertEqual(self.s3.mock_calls, [])

    def test_preserves_33_verbatim_archives_and_separate_exact_version_receipt(self):
        report = self.synthetic_inventory()
        self.preserve(report)
        self.assertTrue(report['complete'])
        self.assertTrue(report['receipt_verified'])
        self.assertTrue(report['archive_preservation_complete'])
        self.assertEqual(report['stage'], 'complete')
        self.assertEqual(report['objects_uploaded'], 33)
        self.assertEqual(len(self.uploads), 34)
        self.assertEqual(len(self.reads), 34)
        self.assertEqual(len({raw for _, _, raw in self.uploads[:-1]}), 33)
        self.assertEqual(self.download.call_count, 33)
        self.s3.get_bucket_versioning.assert_called_once_with(Bucket=PRIVATE)
        self.s3.get_object_lock_configuration.assert_called_once_with(Bucket=PRIVATE)
        for row, source, (key, version, raw) in zip(report['archives'], self.sources, self.uploads):
            self.assertEqual(key, f"{PRIVATE}/cutover-evidence/123456-2/{source['artifact_id']}.zip")
            self.assertEqual(raw, self.zip_bytes[source['artifact_id']])
            self.assertEqual(row['state'], 'retained_verified')
            self.assertEqual(row['uploaded_version_id'], version)
            self.assertIs(row['original_import_report_complete'], source['import_report']['complete'])
            self.assertIs(row['upload_attempted'], True)
            self.assertIs(row['upload_completed'], True)
            self.assertTrue(datetime.fromisoformat(row['checked_at']).tzinfo)
            proof = row['object']
            self.assertEqual(proof, {
                'version_id': version, 'sha256': sha(raw), 'content_length': len(raw),
                'encryption': 'aws:kms', 'lock_mode': 'COMPLIANCE',
                'retain_until': '2099-01-01T00:00:00+00:00', 'retention_verified': True})
            with zipfile.ZipFile(io.BytesIO(raw)) as archive:
                self.assertEqual(inventory.members(archive), source['members'])
        self.assertEqual(sum(not row['original_import_report_complete']
                             for row in report['archives']), 4)
        self.assertTrue(any(member['name'].endswith('-wal')
                            for source in self.sources for member in source['members']))
        self.assertTrue(any(member['name'].endswith('-shm')
                            for source in self.sources for member in source['members']))
        key, version, receipt_bytes = self.uploads[-1]
        self.assertEqual(key, PRIVATE+'/cutover-evidence/123456-2/recovery-preservation-report.json')
        baseline = json.loads(receipt_bytes)
        self.assertEqual(inventory.encoded(report['retained_report']), receipt_bytes)
        self.assertFalse(baseline['complete'])
        self.assertFalse(baseline['receipt_verified'])
        self.assertNotIn('receipt', baseline)
        self.assertEqual(baseline['archives'], report['archives'])
        self.assertTrue(baseline['archive_preservation_complete'])
        self.assertEqual(report['receipt_upload']['uploaded_version_id'], version)
        self.assertIs(report['receipt_upload']['upload_attempted'], True)
        self.assertIs(report['receipt_upload']['upload_completed'], True)
        self.assertEqual(report['receipt']['version_id'], version)
        self.assertEqual(report['receipt']['sha256'], sha(receipt_bytes))
        self.assertEqual(report['receipt']['content_length'], len(receipt_bytes))
        self.assertTrue(report['receipt']['retention_verified'])
        self.assertEqual(report['receipt']['encryption'], 'aws:kms')
        self.assertEqual(report['receipt']['lock_mode'], 'COMPLIANCE')
        self.assertEqual(report['receipt']['retain_until'], '2099-01-01T00:00:00+00:00')
        self.assertIs(report['object_store_writes_attempted'], True)
        self.assert_public(report)
        self.assert_public(baseline)
        self.assertEqual({call[0] for call in self.s3.mock_calls}, {
            'get_bucket_versioning', 'get_object_lock_configuration', 'put_object',
            'head_object', 'get_object_retention', 'get_object'})
        for flag in ('history_complete', 'full_archive_coverage_verified', 'database_access',
                     'policy_changes', 'imports', 'restores', 'fixture_execution'):
            self.assertIs(report[flag], False)

    def test_uploaded_version_is_pinned_and_recorded_before_readback_failure(self):
        path = self.directory/'payload.zip'
        path.write_bytes(b'original exact ZIP bytes')
        record = {}
        self.readback_failure = lambda key: True
        with self.assertRaises(RuntimeError):
            recovery.upload_exact(self.s3, PRIVATE, 'key', PRIVATE+'-kms', path,
                                  sha(path.read_bytes()), path.stat().st_size, record)
        self.assertEqual(record['uploaded_version_id'], 'uploaded-version-1')
        self.assertIs(record['upload_attempted'], True)
        self.assertIs(record['upload_completed'], True)
        self.assertEqual(record['state'], 'uploaded_pending_readback')
        self.assertEqual(self.reads[0]['VersionId'], 'uploaded-version-1')
        self.assertNotIn('object', record)
        self.assert_public(record)

    def test_upload_rejects_missing_immutable_version_without_latest_readback(self):
        path = self.directory/'payload.zip'
        path.write_bytes(b'bounded ZIP evidence')
        for version in (None, '', 'null'):
            with self.subTest(version=version):
                self.s3.reset_mock()
                self.s3.put_object.side_effect = None
                self.s3.put_object.return_value = {'VersionId': version}
                record = {}
                with self.assertRaises(ValueError):
                    recovery.upload_exact(self.s3, PRIVATE, 'key', PRIVATE+'-kms', path,
                                          sha(path.read_bytes()), path.stat().st_size, record)
                self.assertIs(record['upload_attempted'], True)
                self.assertIs(record['upload_completed'], True)
                self.assertEqual(record['state'], 'uploaded_pending_readback')
                self.assertNotIn('uploaded_version_id', record)
                self.assertEqual([call[0] for call in self.s3.mock_calls], ['put_object'])

    def test_exact_readback_rejects_wrong_version_bytes_length_kms_and_compliance(self):
        path = self.directory/'payload.zip'
        raw = b'original bounded ZIP bytes'
        path.write_bytes(raw)
        for partition in ('head_version', 'get_version', 'hash', 'length', 'kms',
                          'encryption', 'lock', 'expiry'):
            with self.subTest(partition=partition):
                self.s3.head_object.side_effect = self.head_object
                self.s3.get_object.side_effect = self.get_object
                self.s3.get_object_retention.side_effect = self.retention
                if partition in ('head_version', 'length', 'kms', 'encryption'):
                    def head(**kwargs):
                        value = self.head_object(**kwargs)
                        field, replacement = {
                            'head_version': ('VersionId', 'newer-latest-version'),
                            'length': ('ContentLength', len(raw)+1),
                            'kms': ('SSEKMSKeyId', 'different-kms'),
                            'encryption': ('ServerSideEncryption', 'AES256'),
                        }[partition]
                        value[field] = replacement
                        return value
                    self.s3.head_object.side_effect = head
                elif partition in ('get_version', 'hash'):
                    def get(**kwargs):
                        self.requested_bytes(kwargs)
                        return {
                            'VersionId': 'newer-latest-version' if partition == 'get_version'
                            else kwargs['VersionId'],
                            'Body': io.BytesIO(b'x'*len(raw) if partition == 'hash' else raw)}
                    self.s3.get_object.side_effect = get
                else:
                    def retention(**kwargs):
                        value = self.retention(**kwargs)
                        if partition == 'lock':
                            value['Retention']['Mode'] = 'GOVERNANCE'
                        else:
                            value['Retention']['RetainUntilDate'] = datetime(
                                2000, 1, 1, tzinfo=timezone.utc)
                        return value
                    self.s3.get_object_retention.side_effect = retention
                record = {}
                with self.assertRaises(ValueError):
                    recovery.upload_exact(self.s3, PRIVATE, 'key', PRIVATE+'-kms', path,
                                          sha(raw), len(raw), record)
                self.assertEqual(record['uploaded_version_id'], self.uploads[-1][1])
                self.assertEqual(record['state'], 'uploaded_pending_readback')
                self.assertNotIn('object', record)

    def test_bucket_policy_failure_prevents_uploads_without_changing_policy(self):
        template = self.synthetic_inventory()
        for partition in ('versioning', 'lock_disabled', 'mode', 'days'):
            with self.subTest(partition=partition):
                report = copy.deepcopy(template)
                self.download.reset_mock()
                self.s3.reset_mock()
                self.s3.get_bucket_versioning.side_effect = (
                    lambda **kwargs: {'Status': 'Suspended' if partition == 'versioning' else 'Enabled'})
                lock = {'ObjectLockEnabled': 'Enabled',
                        'Rule': {'DefaultRetention': {'Mode': 'COMPLIANCE', 'Days': 35}}}
                if partition == 'lock_disabled':
                    lock['ObjectLockEnabled'] = 'Disabled'
                elif partition == 'mode':
                    lock['Rule']['DefaultRetention']['Mode'] = 'GOVERNANCE'
                elif partition == 'days':
                    lock['Rule']['DefaultRetention']['Days'] = 34
                self.s3.get_object_lock_configuration.return_value = {'ObjectLockConfiguration': lock}
                with self.assertRaises(ValueError):
                    self.preserve(report)
                self.assert_not_complete(report)
                self.assertEqual(report['stage'], 'bucket')
                self.assertFalse(report['object_store_writes_attempted'])
                self.assertEqual(report['objects_uploaded'], 0)
                self.assertEqual({call[0] for call in self.s3.mock_calls},
                                 {'get_bucket_versioning', 'get_object_lock_configuration'})

    def test_archive_readback_failure_preserves_progress_without_claiming_completion(self):
        report = self.synthetic_inventory()
        second_id = self.sources[1]['artifact_id']
        self.readback_failure = lambda key: key.endswith(f'/{second_id}.zip')
        with self.assertRaises(RuntimeError):
            self.preserve(report)
        self.assert_not_complete(report)
        self.assertFalse(report['archive_preservation_complete'])
        self.assertEqual(report['stage'], 'upload')
        self.assertEqual(report['objects_uploaded'], 2)
        self.assertEqual(len(self.uploads), 2)
        first, second = report['archives'][:2]
        self.assertEqual(first['state'], 'retained_verified')
        self.assertEqual(first['object']['version_id'], 'uploaded-version-1')
        self.assertEqual(second['state'], 'uploaded_pending_readback')
        self.assertEqual(second['uploaded_version_id'], 'uploaded-version-2')
        self.assertNotIn('object', second)
        self.assertTrue(all(row['state'] == 'source_bytes_verified'
                            for row in report['archives'][2:]))
        self.assertNotIn('receipt', report)
        self.assert_public(report)

    def test_archive_upload_failure_records_attempt_but_not_completed_upload(self):
        report = self.synthetic_inventory()
        self.upload_failure = lambda key: True
        with self.assertRaises(RuntimeError):
            self.preserve(report)
        self.assert_not_complete(report)
        self.assertEqual(report['stage'], 'upload')
        self.assertIs(report['object_store_writes_attempted'], True)
        self.assertEqual(report['objects_uploaded'], 0)
        first = report['archives'][0]
        self.assertIs(first['upload_attempted'], True)
        self.assertNotIn('upload_completed', first)
        self.assertNotIn('uploaded_version_id', first)
        self.assertNotIn('object', first)
        self.assertFalse(report['archive_preservation_complete'])
        self.s3.head_object.assert_not_called()
        self.assert_public(report)

    def test_receipt_upload_or_readback_failure_retains_all_archive_proofs(self):
        template = self.synthetic_inventory()
        for partition in ('upload', 'readback'):
            with self.subTest(partition=partition):
                self.uploads.clear()
                self.stored.clear()
                self.reads.clear()
                self.download.reset_mock()
                report = copy.deepcopy(template)
                is_receipt = lambda key: key.endswith('/recovery-preservation-report.json')
                self.upload_failure = is_receipt if partition == 'upload' else None
                self.readback_failure = is_receipt if partition == 'readback' else None
                with self.assertRaises(RuntimeError):
                    self.preserve(report)
                self.assert_not_complete(report)
                self.assertTrue(report['archive_preservation_complete'])
                self.assertEqual(report['objects_uploaded'], 33)
                self.assertEqual(report['stage'], 'receipt')
                self.assertTrue(all(row['state'] == 'retained_verified' and
                                    row['object']['retention_verified'] for row in report['archives']))
                self.assertNotIn('receipt', report)
                if partition == 'readback':
                    self.assertEqual(report['receipt_upload']['uploaded_version_id'],
                                     'uploaded-version-34')
                    self.assertEqual(len(self.uploads), 34)
                else:
                    self.assertEqual(len(self.uploads), 33)
                    self.assertNotIn('uploaded_version_id', report.get('receipt_upload', {}))
                self.assert_public(report)

    def cli_environment(self):
        return {
            'GITHUB_REF': 'refs/heads/master', 'GITHUB_SHA': 'a'*40,
            'GITHUB_RUN_ID': '123456', 'GITHUB_RUN_ATTEMPT': '2',
            'TEST262_BACKUP_S3_URI': f's3://{PRIVATE}/{PRIVATE}',
            'TEST262_BACKUP_KMS_KEY': PRIVATE+'-kms',
        }

    def cli_modules(self, client):
        config = SimpleNamespace(Config=self.client_config)
        return {'boto3': SimpleNamespace(client=client),
                'botocore': SimpleNamespace(config=config), 'botocore.config': config}

    def test_cli_guards_reject_authorization_ref_sha_run_and_overwrite_before_io(self):
        cases = [
            ('authorization', {}, 'wrong'),
            ('authorization_blank', {}, ''),
            ('branch', {'GITHUB_REF': 'refs/heads/topic'}, recovery.AUTHORIZATION),
            ('sha', {'GITHUB_SHA': PRIVATE}, recovery.AUTHORIZATION),
            ('sha_short', {'GITHUB_SHA': 'a'*39}, recovery.AUTHORIZATION),
            ('sha_long', {'GITHUB_SHA': 'a'*41}, recovery.AUTHORIZATION),
            ('run_zero', {'GITHUB_RUN_ID': '0'}, recovery.AUTHORIZATION),
            ('run_negative', {'GITHUB_RUN_ID': '-1'}, recovery.AUTHORIZATION),
            ('run_missing', {'GITHUB_RUN_ID': ''}, recovery.AUTHORIZATION),
            ('attempt_zero', {'GITHUB_RUN_ATTEMPT': '0'}, recovery.AUTHORIZATION),
            ('attempt_negative', {'GITHUB_RUN_ATTEMPT': '-1'}, recovery.AUTHORIZATION),
            ('attempt_invalid', {'GITHUB_RUN_ATTEMPT': '1x'}, recovery.AUTHORIZATION),
            ('overwrite', {}, recovery.AUTHORIZATION),
        ]
        for name, change, authorization in cases:
            with self.subTest(case=name):
                path = self.directory/(name+'.json')
                if name == 'overwrite':
                    path.write_bytes(b'existing evidence must remain verbatim')
                client, preserve = Mock(), Mock()
                output = io.StringIO()
                with patch.dict(os.environ, dict(self.cli_environment(), **change), clear=True), \
                     patch.dict('sys.modules', self.cli_modules(client)), \
                     patch.object(recovery, 'preserve', preserve), \
                     redirect_stdout(output), redirect_stderr(output):
                    try:
                        status = recovery.main(['--authorization', authorization, '--output', str(path)])
                    except (ValueError, SystemExit) as error:
                        status = error.code if isinstance(error, SystemExit) else 1
                    self.assertNotEqual(status, 0)
                client.assert_not_called()
                self.client_config.assert_not_called()
                preserve.assert_not_called()
                if name == 'overwrite':
                    self.assertEqual(path.read_bytes(), b'existing evidence must remain verbatim')
                else:
                    report = json.loads(path.read_bytes())
                    self.assertFalse(report['complete'])
                    self.assertFalse(report['archive_preservation_complete'])
                    self.assertFalse(report['receipt_verified'])
                    self.assertEqual(report['failed_stage'], 'scope')
                    self.assert_public(report)
                    self.assertNotIn(PRIVATE, output.getvalue())

    def test_cli_success_persists_exact_receipt_self_proof_with_no_network(self):
        self.synthetic_inventory()
        path = self.directory/'cli-success.json'
        implementation = recovery.preserve
        def preserve(s3, uri, kms, report):
            return implementation(s3, uri, kms, report, fetch=self.fetch, download=self.download)
        output = io.StringIO()
        client = Mock(return_value=self.s3)
        with patch.dict(os.environ, self.cli_environment(), clear=True), \
             patch.dict('sys.modules', self.cli_modules(client)), \
             patch.object(recovery, 'preserve', side_effect=preserve), \
             redirect_stdout(output), redirect_stderr(output):
            self.assertEqual(recovery.main([
                '--authorization', recovery.AUTHORIZATION, '--output', str(path)]), 0)
        report = json.loads(path.read_bytes())
        self.client_config.assert_called_once_with(retries={'total_max_attempts': 1})
        client.assert_called_once_with('s3', config=self.client_config.return_value)
        self.assertEqual(report['stage'], 'complete')
        self.assertTrue(report['complete'])
        self.assertTrue(report['receipt_verified'])
        self.assertEqual(report['objects_uploaded'], 33)
        self.assertEqual(report['run_id'], 123456)
        self.assertEqual(report['run_attempt'], 2)
        self.assertEqual(report['source_revision'], 'a'*40)
        self.assertEqual(report['receipt']['sha256'], sha(inventory.encoded(report['retained_report'])))
        self.assertEqual(report['receipt']['content_length'], len(inventory.encoded(report['retained_report'])))
        self.assert_public(report)
        self.assertNotIn(PRIVATE, output.getvalue())

    def test_download_helper_uses_scoped_subprocess_and_local_binary_stream_only(self):
        path = self.directory/'download.zip'
        def run(command, **kwargs):
            self.assertEqual(command, [
                'gh', 'api', 'repos/tomacox74/js2il/actions/artifacts/123/zip'])
            self.assertEqual(kwargs['timeout'], 180)
            self.assertIs(kwargs['check'], True)
            self.assertEqual(kwargs['stderr'], subprocess.DEVNULL)
            kwargs['stdout'].write(b'opaque ZIP bytes\x00\xff')
        with patch.object(recovery.subprocess, 'run', side_effect=run) as command:
            recovery.download_zip(123, path)
        command.assert_called_once()
        self.assertEqual(path.read_bytes(), b'opaque ZIP bytes\x00\xff')

    def test_cli_sanitizes_failure_and_persists_truthful_stage_and_aws_code(self):
        for code in ('AccessDenied', PRIVATE+'/invalid-code'):
            with self.subTest(code=code):
                path = self.directory/('failure-'+str(len(list(self.directory.glob('failure-*'))))+'.json')
                output = io.StringIO()
                client = Mock(return_value=self.s3)
                self.client_config.reset_mock()
                def fail(s3, uri, kms, report):
                    report['stage'] = 'bucket'
                    raise aws_error(code)
                with patch.dict(os.environ, self.cli_environment(), clear=True), \
                     patch.dict('sys.modules', self.cli_modules(client)), \
                     patch.object(recovery, 'preserve', side_effect=fail), \
                     redirect_stdout(output), redirect_stderr(output):
                    self.assertEqual(recovery.main([
                        '--authorization', recovery.AUTHORIZATION, '--output', str(path)]), 1)
                self.client_config.assert_called_once_with(retries={'total_max_attempts': 1})
                client.assert_called_once_with('s3', config=self.client_config.return_value)
                report = json.loads(path.read_bytes())
                self.assert_not_complete(report)
                self.assertFalse(report['archive_preservation_complete'])
                self.assertEqual(report['failed_stage'], 'bucket')
                self.assertEqual(report['error_class'], 'RuntimeError')
                self.assertEqual(report['objects_uploaded'], 0)
                if code == 'AccessDenied':
                    self.assertEqual(report['aws_error_code'], 'AccessDenied')
                else:
                    self.assertNotIn('aws_error_code', report)
                self.assertNotIn('error_message', report)
                self.assert_public(report)
                self.assertNotIn(PRIVATE, output.getvalue())

    def test_workflow_guards_authorization_before_role_and_keeps_minimal_policy(self):
        root = Path(__file__).resolve().parents[3]
        workflow = (root/'.github/workflows/test262-cutover-evidence.yml').read_text()
        self.assertIn("if: github.ref == 'refs/heads/master'", workflow)
        self.assertIn('environment: test262-catalogue-backup', workflow)
        self.assertIn('group: test262-cutover-evidence-preservation', workflow)
        self.assertIn('cancel-in-progress: false', workflow)
        for operation in ('preserve', 'verify-retained', 'discover-retained', 'preserve-recovery'):
            self.assertIn(f"if: inputs.operation == '{operation}'", workflow)
        self.assertRegex(workflow, r'recovery_authorization:\n(?:[^\n]*\n)*?\s+default: [\'"]{2}')
        validation = workflow.split('- name: Validate reviewed operation', 1)[1].split(
            '- uses: aws-actions/configure-aws-credentials', 1)[0]
        self.assertIn('RECOVERY_AUTHORIZATION: ${{ inputs.recovery_authorization }}', validation)
        self.assertIn(recovery.AUTHORIZATION, validation)
        # Execute only the extracted local validation shell (no AWS/GitHub calls).
        shell = validation.split('run: |', 1)[1]
        shell = '\n'.join(line[10:] if line.startswith(' '*10) else line
                          for line in shell.splitlines())
        for operation, token, succeeds in (
                ('preserve-recovery', '', False),
                ('preserve-recovery', 'wrong', False),
                ('preserve-recovery', recovery.AUTHORIZATION, True),
                ('preserve', '', True), ('verify-retained', '', True),
                ('discover-retained', '', True), ('unknown', recovery.AUTHORIZATION, False)):
            with self.subTest(operation=operation, token=token):
                result = subprocess.run(
                    ['bash', '-c', shell],
                    env=dict(os.environ, OPERATION=operation, RECOVERY_AUTHORIZATION=token),
                    capture_output=True, text=True, timeout=5, check=False)
                self.assertEqual(result.returncode == 0, succeeds, result.stdout+result.stderr)
        block = next(block for block in workflow.split('      - ')
                     if 'python -m scripts.test262.central.recovery_preservation' in block)
        self.assertIn("if: inputs.operation == 'preserve-recovery'", block)
        self.assertIn('RECOVERY_AUTHORIZATION: ${{ inputs.recovery_authorization }}', block)
        self.assertIn('--authorization "$RECOVERY_AUTHORIZATION"', block)
        self.assertIn('--output /tmp/test262-cutover-report/recovery-preservation-report.json', block)
        policies = [json.loads(raw) for raw in re.findall(
            r"'(\{\"Version\".*?\})' \|\|", workflow)]
        self.assertEqual(len(policies), 5)
        recovery_policy = re.search(
            r"inputs.operation == 'preserve-recovery' &&\s*'(\{\"Version\".*?\})' \|\|",
            workflow)
        self.assertIsNotNone(recovery_policy)
        self.assertEqual(json.loads(recovery_policy[1]), policies[1])
        def actions(policy):
            return {action for statement in policy['Statement'] for action in statement['Action']}
        self.assertEqual(actions(policies[0]), {
            's3:ListBucketVersions', 's3:GetObjectVersion', 's3:GetObjectRetention'})
        self.assertEqual(actions(policies[-1]), {
            's3:GetBucketVersioning', 's3:GetBucketObjectLockConfiguration',
            's3:GetObjectVersion', 's3:GetObjectRetention', 'kms:Decrypt'})
        required = {
            's3:PutObject', 's3:GetBucketVersioning', 's3:GetBucketObjectLockConfiguration',
            's3:GetObjectVersion', 's3:GetObjectRetention', 'kms:GenerateDataKey', 'kms:Decrypt'}
        self.assertTrue(required <= actions(policies[1]))
        self.assertTrue(actions(policies[1]) <= required | {'kms:Encrypt'})
        self.assertTrue(all(statement['Effect'] == 'Allow'
                            for statement in policies[1]['Statement']))


if __name__ == '__main__':
    unittest.main()
