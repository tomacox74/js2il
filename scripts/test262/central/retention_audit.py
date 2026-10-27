"""Audit retention configuration and pinned object metadata without payloads or writes."""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess

from botocore.exceptions import BotoCoreError, ClientError

from . import recovery_coverage
from .cutover_evidence import REPOSITORY
from .monitoring import github
from .retained_objects import destination

PRESERVATION = recovery_coverage.LEDGER.with_name('retained-recovery-preservation-40393597860.json')
PRESERVATION_SHA256 = '4e5b1c4590ec3325c6bebd39d9a5001d2ebf9a72a858aef8e6d1e94762901530'
AWS_CODES = {
    'AccessDenied', 'AccessDeniedException', 'NoSuchBucket', 'NoSuchKey', 'NoSuchVersion',
    'NoSuchLifecycleConfiguration', 'ObjectLockConfigurationNotFoundError',
    'ServerSideEncryptionConfigurationNotFoundError', 'InvalidRequest',
    'MethodNotAllowed', 'PermanentRedirect', 'SlowDown', 'ServiceUnavailable',
}
ABSENT_CODES = {
    'object_lock': 'ObjectLockConfigurationNotFoundError',
    'lifecycle': 'NoSuchLifecycleConfiguration',
    'encryption': 'ServerSideEncryptionConfigurationNotFoundError',
}


def require(condition):
    if not condition:
        raise ValueError('Invalid retention audit input or response')


def timestamp(value):
    point = value if isinstance(value, datetime) else datetime.fromisoformat(value)
    require(point.tzinfo is not None)
    return point


def failure(error, state):
    result = {'state': state, 'error_class': type(error).__name__}
    if isinstance(error, ClientError):
        code = error.response.get('Error', {}).get('Code')
        result['aws_error_code'] = code if code in AWS_CODES else 'UnrecognizedAwsError'
    return result


def initial_report(now=None):
    now = timestamp(now or datetime.now(timezone.utc))
    return {
        'schema_version': 1, 'repository': REPOSITORY, 'operation': 'audit-retention',
        'started_at': now.isoformat(), 'checked_at': now.isoformat(),
        'audit_complete': False, 'object_metadata_verified': False,
        'configuration_reads_complete': False, 'database_access': False, 'object_store_access': False,
        'object_store_writes': False, 'payload_reads': False, 'payload_digest_verified': False,
        'notifications_enabled': False, 'policy_changes': False, 'imports': False,
        'restores': False, 'fixture_execution': False, 'retention_policy_adopted': False,
        'collector_sha256': hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'limits': [
            'This is a bounded configuration/metadata observation, not a payload digest or decrypt test.',
            'Configuration read denial is unknown, not absence or adequate retention.',
            'Lifecycle prefix overlap is conservative; object tags and size selectors are not evaluated.',
            'No lifecycle rule does not guarantee indefinite protection or an adopted retention policy.',
            'Lock expiry is not evidence that an object was deleted.',
            'The original private eight-object receipt has no recorded self-version; it is not assessed.',
            'IAM/KMS policies, replica destinations and all-history/all-log coverage are not audited.',
            'Workflow receipts are authenticated historical assertions, not new backups or restore drills.']}


def project_lifecycle_rule(rule, base, index):
    require(isinstance(rule, dict) and rule.get('Status') in ('Enabled', 'Disabled'))
    selector = rule.get('Filter', {})
    require(isinstance(selector, dict))
    combined = selector.get('And', {})
    require(isinstance(combined, dict))
    prefix = combined.get('Prefix', selector.get('Prefix', rule.get('Prefix', '')))
    require(isinstance(prefix, str))
    scope = base.rstrip('/')+'/' if base else ''
    overlaps = prefix.startswith(scope) or scope.startswith(prefix)
    result = {
        'index': index, 'enabled': rule['Status'] == 'Enabled',
        'destination_relation': 'overlaps' if overlaps else 'outside',
        'conditions_unassessed': bool(set(selector)-{'Prefix', 'And'} or set(combined)-{'Prefix'}),
    }
    expiration = rule.get('Expiration', {})
    noncurrent = rule.get('NoncurrentVersionExpiration', {})
    require(isinstance(expiration, dict) and isinstance(noncurrent, dict))
    for source, field, target in (
            (expiration, 'Days', 'expiration_days'),
            (noncurrent, 'NoncurrentDays', 'noncurrent_expiration_days'),
            (noncurrent, 'NewerNoncurrentVersions', 'newer_noncurrent_versions')):
        if field in source:
            require(type(source[field]) is int and source[field] > 0)
            result[target] = source[field]
    if 'Date' in expiration:
        result['expiration_date'] = timestamp(expiration['Date']).isoformat()
    if 'ExpiredObjectDeleteMarker' in expiration:
        require(type(expiration['ExpiredObjectDeleteMarker']) is bool)
        result['expired_object_delete_marker'] = expiration['ExpiredObjectDeleteMarker']
    for field, target in (('Transitions', 'transition_count'),
                          ('NoncurrentVersionTransitions', 'noncurrent_transition_count')):
        transitions = rule.get(field, [])
        require(isinstance(transitions, list))
        result[target] = len(transitions)
    return result


def project_configuration(kind, raw, base, kms_key):
    require(isinstance(raw, dict))
    if kind == 'versioning':
        require(raw.get('Status') in (None, 'Enabled', 'Suspended')
                and raw.get('MFADelete') in (None, 'Enabled', 'Disabled'))
        return {'state': 'observed', 'enabled': raw.get('Status') == 'Enabled',
                'mfa_delete': raw.get('MFADelete', 'not_reported')}
    if kind == 'object_lock':
        config = raw['ObjectLockConfiguration']
        require(isinstance(config, dict) and config.get('ObjectLockEnabled') == 'Enabled')
        rule = config.get('Rule', {})
        require(isinstance(rule, dict))
        default = rule.get('DefaultRetention', {})
        require(isinstance(default, dict))
        result = {'state': 'observed', 'enabled': True}
        if default:
            require(default.get('Mode') in ('COMPLIANCE', 'GOVERNANCE'))
            result['default_mode'] = default['Mode']
            require(('Days' in default) != ('Years' in default))
            for key in ('Days', 'Years'):
                if key in default:
                    require(type(default[key]) is int and default[key] > 0)
                    result['default_'+key.lower()] = default[key]
        else:
            result['default_mode'] = 'not_configured'
        return result
    if kind == 'lifecycle':
        rules = raw['Rules']
        require(isinstance(rules, list) and 0 < len(rules) <= 1000)
        return {'state': 'observed', 'rules': [
            project_lifecycle_rule(rule, base, index) for index, rule in enumerate(rules)]}
    config = raw['ServerSideEncryptionConfiguration']
    require(isinstance(config, dict))
    rules = config['Rules']
    require(isinstance(rules, list) and len(rules) == 1)
    require(isinstance(rules[0], dict))
    default = rules[0]['ApplyServerSideEncryptionByDefault']
    require(isinstance(default, dict))
    require(default.get('SSEAlgorithm') in ('AES256', 'aws:kms', 'aws:kms:dsse'))
    return {'state': 'observed', 'algorithms': [default['SSEAlgorithm']],
            'configured_kms_identity_matches_exactly': default.get('KMSMasterKeyID') == kms_key}


def audit_configuration(s3, bucket, base, kms_key):
    observations = {}
    for kind, method in (
            ('versioning', 'get_bucket_versioning'),
            ('object_lock', 'get_object_lock_configuration'),
            ('lifecycle', 'get_bucket_lifecycle_configuration'),
            ('encryption', 'get_bucket_encryption')):
        try:
            raw = getattr(s3, method)(Bucket=bucket)
            observations[kind] = project_configuration(kind, raw, base, kms_key)
        except ClientError as error:
            code = error.response.get('Error', {}).get('Code')
            state = 'not_configured' if code == ABSENT_CODES.get(kind) else 'unavailable'
            observations[kind] = failure(error, state)
        except BotoCoreError as error:
            observations[kind] = failure(error, 'unavailable')
        except (ValueError, TypeError, KeyError) as error:
            observations[kind] = failure(error, 'invalid')
    return observations


def load_evidence():
    ledger = recovery_coverage.load_ledger()
    raw = PRESERVATION.read_bytes()
    require(hashlib.sha256(raw).hexdigest() == PRESERVATION_SHA256)
    return ledger, json.loads(raw)


def retention_targets(ledger, preservation):
    targets = []
    for row in ledger['archives']:
        origin = 40393597860 if row['verification']['run_id'] == 40393597860 else 37715977350
        targets.append(({'kind': 'archive', 'artifact_id': row['artifact_id']},
                        f'{origin}-1', str(row['artifact_id'])+'.zip', row['object'],
                        row['actions_expires_at_from_frozen_inventory']))
    targets.append(({'kind': 'preservation_receipt'}, '40393597860-1',
                    'recovery-preservation-report.json', preservation['report']['receipt'],
                    preservation['actions_artifact']['expires_at']))
    return targets


def audit_object(s3, bucket, key, kms_key, pin, identity, expiration, now):
    now = timestamp(now)
    row = dict(identity, version_id=pin['version_id'], expected_sha256=pin['sha256'],
               metadata_verified=False, payload_digest_verified=False)
    try:
        head = s3.head_object(Bucket=bucket, Key=key, VersionId=pin['version_id'])
        retained = s3.get_object_retention(Bucket=bucket, Key=key, VersionId=pin['version_id'])
        require(isinstance(head, dict) and isinstance(retained, dict))
        retention = retained['Retention']
        require(isinstance(retention, dict))
        require(isinstance(head.get('VersionId'), str) and head['VersionId'] not in ('', 'null'))
        require(type(head.get('ContentLength')) is int and head['ContentLength'] >= 0)
        require(head.get('ServerSideEncryption') in ('AES256', 'aws:kms', 'aws:kms:dsse'))
        require(retention.get('Mode') in ('COMPLIANCE', 'GOVERNANCE'))
        until = timestamp(retention['RetainUntilDate'])
        previous = timestamp(pin['retain_until'])
        covers = until > timestamp(expiration.replace('Z', '+00:00')) if expiration else None
        row.update(
            state='observed', content_length=head['ContentLength'],
            encryption=head['ServerSideEncryption'], retain_until=until.isoformat(),
            lock_mode=retention['Mode'], version_matches=head['VersionId'] == pin['version_id'],
            length_matches=head['ContentLength'] == pin['content_length'],
            kms_identity_matches=head.get('SSEKMSKeyId') == kms_key,
            prior_lock_not_shortened=until >= previous,
            compliance_lock_current=retention['Mode'] == 'COMPLIANCE' and until > now,
            covers_frozen_actions_expiry=covers)
        row['metadata_verified'] = all(row[field] for field in (
            'version_matches', 'length_matches', 'kms_identity_matches',
            'prior_lock_not_shortened', 'compliance_lock_current')) and row['encryption'] == 'aws:kms'
    except (ClientError, BotoCoreError) as error:
        row.update(failure(error, 'unavailable'))
    except (ValueError, TypeError, KeyError) as error:
        row.update(failure(error, 'invalid'))
    return row


def audit_objects(s3, bucket, base, kms_key, ledger, preservation, now):
    now = timestamp(now)
    observations = []
    for identity, origin, name, pin, expiration in retention_targets(ledger, preservation):
        key = '/'.join(filter(None, (base, 'cutover-evidence', origin, name)))
        observations.append(audit_object(s3, bucket, key, kms_key, pin, identity, expiration, now))
    return observations


def audit(s3, uri, kms_key, report, now=None, fetch=github):
    now = timestamp(now or datetime.now(timezone.utc))
    require(isinstance(kms_key, str) and bool(kms_key.strip()))
    ledger, preservation = load_evidence()
    bucket, base = destination(uri)
    report.update(ledger_sha256=recovery_coverage.LEDGER_SHA256,
                  preservation_sha256=PRESERVATION_SHA256,
                  original_private_receipt_self_version='unrecorded_not_assessed',
                  stage='configuration', object_store_access=True,
                  metadata_reference_at=now.isoformat())
    report['configuration'] = audit_configuration(s3, bucket, base, kms_key)
    report['configuration_reads_complete'] = all(
        row['state'] in ('observed', 'not_configured') for row in report['configuration'].values())
    report['stage'] = 'exact-version-metadata'
    report['objects'] = audit_objects(s3, bucket, base, kms_key, ledger, preservation, now)
    report['object_metadata_verified'] = all(row['metadata_verified'] for row in report['objects'])
    findings = []
    for kind, row in report['configuration'].items():
        if row['state'] not in ('observed', 'not_configured'):
            findings.append({'kind': kind, 'code': 'configuration_read_'+row['state'],
                             'state': 'unknown', 'owner_issue': 2343})
        elif kind == 'versioning' and row.get('enabled') is not True:
            findings.append({'kind': kind, 'code': 'bucket_versioning_not_enabled',
                             'state': 'attention', 'owner_issue': 2343})
        elif kind == 'object_lock' and (row.get('enabled') is not True
                                       or row.get('default_mode') != 'COMPLIANCE'
                                       or not (row.get('default_days', 0) >= 35
                                               or row.get('default_years', 0) >= 1)):
            findings.append({'kind': kind, 'code': 'default_compliance_floor_not_met',
                             'state': 'attention', 'owner_issue': 2343})
    for row in report['objects']:
        identity = {key: row[key] for key in ('kind', 'artifact_id') if key in row}
        if not row['metadata_verified']:
            findings.append(dict(identity, code='exact_version_metadata_not_verified',
                                 state='unknown' if row['state'] != 'observed' else 'attention',
                                 owner_issue=2343))
        if row.get('covers_frozen_actions_expiry') is not True:
            findings.append(dict(identity, code='protection_beyond_actions_expiry_unproven',
                                 state='attention', owner_issue=2343))
    report['stage'] = 'workflow-receipts'
    report['coverage_collection_complete'] = False
    try:
        report['coverage'] = recovery_coverage.collect(fetch, now)
        report['coverage_collection_complete'] = True
        # Live probes supersede dated retention proofs, not independent workflow failures.
        findings.extend(row for row in report['coverage']['findings']
                        if row.get('kind') in ('backup', 'restore'))
    except (OSError, ValueError, TypeError, KeyError, subprocess.SubprocessError) as error:
        report['coverage'] = dict(failure(error, 'unavailable'), fetch_state='unavailable')
        findings.append({'kind': 'workflow-receipts', 'code': 'workflow_coverage_unavailable',
                         'state': 'unknown', 'owner_issue': 2343})
    report['findings'] = findings
    report['audit_complete'] = (report['configuration_reads_complete']
                                and report['coverage_collection_complete'] and all(
                                    row['state'] == 'observed' for row in report['objects']))
    report['stage'] = 'complete' if report['audit_complete'] else 'incomplete'
    report['summary'] = {
        'archive_count': sum(row['kind'] == 'archive' for row in report['objects']),
        'pinned_object_count': len(report['objects']),
        'metadata_verified_count': sum(row['metadata_verified'] for row in report['objects']),
        'earliest_observed_lock_deadline': min(
            (row['retain_until'] for row in report['objects'] if 'retain_until' in row), default=None)}


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args(argv)
    if args.output.exists():
        print(json.dumps({'audit_complete': False, 'error_class': 'FileExistsError'}))
        return 1
    report = initial_report()
    status = 1
    try:
        require(os.environ.get('GITHUB_REF') == 'refs/heads/master'
                and os.environ.get('GITHUB_REPOSITORY') == REPOSITORY)
        revision, run, attempt = (os.environ[key] for key in
                                  ('GITHUB_SHA', 'GITHUB_RUN_ID', 'GITHUB_RUN_ATTEMPT'))
        require(re.fullmatch(r'[0-9a-f]{40}', revision)
                and re.fullmatch(r'[1-9]\d*', run) and re.fullmatch(r'[1-9]\d*', attempt))
        uri, kms_key = os.environ['TEST262_BACKUP_S3_URI'], os.environ['TEST262_BACKUP_KMS_KEY']
        destination(uri)
        require(bool(kms_key.strip()))
        report.update(source_revision=revision, run_id=int(run), run_attempt=int(attempt))
        import boto3
        from botocore.config import Config
        s3 = boto3.client('s3', config=Config(retries={'total_max_attempts': 1}))
        audit(s3, uri, kms_key, report)
        status = 0 if report['audit_complete'] and report['object_metadata_verified'] else 1
    except (ClientError, BotoCoreError, OSError, ValueError, TypeError, KeyError,
            subprocess.SubprocessError) as error:
        report.update(failure(error, 'incomplete'), failed_stage=report.get('stage', 'validate-input'))
    finally:
        report['checked_at'] = datetime.now(timezone.utc).isoformat()
        args.output.parent.mkdir(parents=True, exist_ok=True)
        with args.output.open('x') as stream:
            stream.write(json.dumps(report, indent=2)+'\n')
    print(json.dumps({key: report[key] for key in
                     ('audit_complete', 'configuration_reads_complete', 'object_metadata_verified')}))
    return status


if __name__ == '__main__':
    raise SystemExit(main())
