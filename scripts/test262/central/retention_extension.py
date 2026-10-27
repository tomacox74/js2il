"""Extend only the 42 owner-authorized pinned COMPLIANCE versions to a fixed date."""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import re

from botocore.exceptions import BotoCoreError, ClientError

from . import retention_audit as audit
from .cutover_evidence import REPOSITORY
from .retained_objects import destination

AUTHORIZATION = 'extend-42-retention-20270430'
RETAIN_UNTIL = datetime(2027, 4, 30, tzinfo=timezone.utc)


def require(condition):
    if not condition:
        raise ValueError('Invalid authorized retention extension input or response')


def initial_report(now=None):
    now = audit.timestamp(now or datetime.now(timezone.utc))
    return {
        'schema_version': 1, 'repository': REPOSITORY, 'operation': 'extend-retention',
        'started_at': now.isoformat(), 'checked_at': now.isoformat(),
        'target_retain_until': RETAIN_UNTIL.isoformat(),
        'complete': False, 'preflight_complete': False, 'protection_verified': False,
        'retention_writes_attempted': False, 'uploads': False, 'deletions': False,
        'imports': False, 'restores': False, 'production_changed': False,
        'permissions_changed': False, 'bucket_configuration_changes': False,
        'database_access': False, 'fixture_execution': False,
        'payload_reads': False, 'payload_digest_verified': False,
        'notifications_enabled': False, 'retention_policy_adopted': False,
        'collector_sha256': hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'audit_collector_sha256': hashlib.sha256(Path(audit.__file__).read_bytes()).hexdigest(),
        'objects': [], 'stage': 'validate-input',
        'limits': [
            'Only the accepted 41 archive versions and verified newer receipt version are authorized.',
            'Retention metadata PUT is authorized; object uploads/deletions and configuration/grant changes are not.',
            'COMPLIANCE extensions are irreversible; this fixed horizon is not an indefinite retention policy.',
            'Original payload hashes remain pinned expectations, not fresh rehash/decryption proofs.',
            'Writes stop at the first failure; acknowledged or ambiguous changes are never rolled back.',
            'The original private eight-object receipt self-version remains unrecorded and outside this target set.',
            'The local/Actions report is not uploaded to S3 or independently locked by this operation.']}


def summarize(report):
    rows = report['objects']
    deadlines = []
    for row in rows:
        if row['target_protection_verified']:
            observation = row.get('final_verification', row.get('verification'))
            if observation and 'retain_until' in observation:
                deadlines.append(audit.timestamp(observation['retain_until']))
    report['summary'] = {
        'target_count': len(rows),
        'put_attempt_count': sum(row['put_attempted'] for row in rows),
        'put_acknowledged_count': sum(row['put_acknowledged'] for row in rows),
        'target_verified_count': sum(row['target_protection_verified'] for row in rows),
        'unchanged_or_longer_count': sum(row['state'] == 'already_protected' for row in rows),
        'earliest_verified_retain_until': min(deadlines).isoformat() if deadlines else None}


def extend(s3, uri, kms_key, report, authorization, now=None, checkpoint=None):
    now = audit.timestamp(now or datetime.now(timezone.utc))
    require(authorization == AUTHORIZATION and now < RETAIN_UNTIL)
    require(isinstance(kms_key, str) and bool(kms_key.strip()))
    bucket, base = destination(uri)
    ledger, preservation = audit.load_evidence()
    targets = audit.retention_targets(ledger, preservation)
    require(len(targets) == 42
            and sum(identity['kind'] == 'archive' for identity, *_ in targets) == 41
            and sum(identity['kind'] == 'preservation_receipt' for identity, *_ in targets) == 1
            and len({(origin, name, pin['version_id']) for _, origin, name, pin, _ in targets}) == 42)
    report.update(ledger_sha256=audit.recovery_coverage.LEDGER_SHA256,
                  preservation_sha256=audit.PRESERVATION_SHA256, stage='preflight-configuration')
    report['configuration'] = audit.audit_configuration(s3, bucket, base, kms_key)
    config = report['configuration']
    lock = config['object_lock']
    configuration_verified = (
        all(row['state'] in ('observed', 'not_configured') for row in config.values())
        and config['versioning'].get('enabled') is True
        and lock.get('enabled') is True and lock.get('default_mode') == 'COMPLIANCE'
        and (lock.get('default_days', 0) >= 35 or lock.get('default_years', 0) >= 1)
        and config['encryption'].get('configured_kms_identity_matches_exactly') is True
        and config['encryption'].get('algorithms') == ['aws:kms'])
    report['stage'] = 'preflight-metadata'
    before = audit.audit_objects(s3, bucket, base, kms_key, ledger, preservation, now)
    require(len(before) == 42)
    for (identity, _, _, pin, _), observation in zip(targets, before):
        require(observation['version_id'] == pin['version_id']
                and observation.get('artifact_id') == identity.get('artifact_id')
                and observation['kind'] == identity['kind'])
        report['objects'].append(dict(
            identity, version_id=pin['version_id'], expected_sha256=pin['sha256'],
            preflight=observation, state='not_attempted', put_attempted=False,
            put_acknowledged=False, target_protection_verified=False))
    report['preflight_complete'] = configuration_verified and all(
        row['preflight'].get('metadata_verified') is True for row in report['objects'])

    journal_failed = False

    def flush():
        nonlocal journal_failed
        summarize(report)
        if checkpoint is not None and not journal_failed:
            try:
                checkpoint(report)
            except (OSError, ValueError, TypeError) as error:
                journal_failed = True
                report['journal_error'] = audit.failure(error, 'incomplete')
                report.setdefault('failed_stage', 'local-checkpoint')
                report.setdefault('failure_code', 'local_checkpoint_failed')
                report['complete'] = False
                if report['stage'] == 'complete':
                    report['stage'] = 'incomplete'
        return not journal_failed

    if not report['preflight_complete']:
        report.update(stage='incomplete', failed_stage='preflight',
                      failure_code='preflight_not_verified')
        flush()
        return
    failed = not flush()
    report['stage'] = 'extending'
    for target, row in zip(targets, report['objects']):
        if failed:
            break
        identity, origin, name, pin, expiration = target
        prior = audit.timestamp(row['preflight']['retain_until'])
        if prior >= RETAIN_UNTIL:
            row['state'] = 'already_protected'
            continue
        key = '/'.join(filter(None, (base, 'cutover-evidence', origin, name)))
        row.update(put_attempted=True, state='put_attempted')
        report['retention_writes_attempted'] = True
        if not flush():
            row.update(put_attempted=False, state='journal_failed_before_write')
            report['retention_writes_attempted'] = any(
                item['put_attempted'] for item in report['objects'])
            failed = True
            break
        try:
            response = s3.put_object_retention(
                Bucket=bucket, Key=key, VersionId=pin['version_id'],
                Retention={'Mode': 'COMPLIANCE', 'RetainUntilDate': RETAIN_UNTIL})
            require(isinstance(response, dict)
                    and isinstance(response.get('ResponseMetadata'), dict)
                    and response['ResponseMetadata'].get('HTTPStatusCode') == 200)
        except (ClientError, BotoCoreError, OSError, ValueError, TypeError, KeyError) as error:
            row.update(state='write_failed', error=audit.failure(error, 'unconfirmed'))
            report.update(failed_stage='put-object-retention', failure_code='retention_write_failed')
            failed = True
            flush()
            break
        row.update(put_acknowledged=True, state='put_acknowledged')
        if not flush():
            failed = True
            break
        required = dict(pin, retain_until=max(prior, RETAIN_UNTIL).isoformat())
        row['verification'] = audit.audit_object(
            s3, bucket, key, kms_key, required, identity, expiration, now)
        if row['verification'].get('metadata_verified') is not True:
            row['state'] = 'verification_failed'
            report.update(failed_stage='post-write-verification',
                          failure_code='post_write_verification_failed')
            failed = True
            flush()
            break
        row.update(state='extended_verified', target_protection_verified=True)
        if not flush():
            failed = True
            break

    report['stage'] = 'final-verification'
    for target, row in zip(targets, report['objects']):
        identity, origin, name, pin, expiration = target
        key = '/'.join(filter(None, (base, 'cutover-evidence', origin, name)))
        required = dict(pin, retain_until=row['preflight']['retain_until'])
        observation = audit.audit_object(
            s3, bucket, key, kms_key, required, identity, expiration, now)
        row['final_verification'] = observation
        row['target_protection_verified'] = (
            observation.get('metadata_verified') is True
            and audit.timestamp(observation['retain_until']) >= RETAIN_UNTIL)
    report['protection_verified'] = all(
        row['target_protection_verified'] for row in report['objects'])
    report['complete'] = report['protection_verified'] and not failed
    report['stage'] = 'complete' if report['complete'] else 'incomplete'
    if not report['complete'] and 'failed_stage' not in report:
        report.update(failed_stage='final-verification', failure_code='final_verification_failed')
    flush()


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--authorization', required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args(argv)
    temporary = args.output.with_name('.'+args.output.name+'.tmp')
    try:
        if args.output.exists() or temporary.exists():
            raise FileExistsError('Refusing to overwrite a local report or checkpoint')
        report = initial_report()
        args.output.parent.mkdir(parents=True, exist_ok=True)
        with args.output.open('x') as stream:
            stream.write(json.dumps(report, indent=2)+'\n')
    except (OSError, ValueError, TypeError) as error:
        print(json.dumps({'complete': False, 'error_class': type(error).__name__}))
        return 1

    def checkpoint(document):
        document['checked_at'] = datetime.now(timezone.utc).isoformat()
        summarize(document)
        with temporary.open('x') as stream:
            stream.write(json.dumps(document, indent=2)+'\n')
        temporary.replace(args.output)

    status = 1
    try:
        require(args.authorization == AUTHORIZATION
                and os.environ.get('GITHUB_REF') == 'refs/heads/master'
                and os.environ.get('GITHUB_REPOSITORY') == REPOSITORY
                and audit.timestamp(report['started_at']) < RETAIN_UNTIL)
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
        extend(s3, uri, kms_key, report, args.authorization, checkpoint=checkpoint)
        status = 0 if report['complete'] and report['protection_verified'] else 1
    except (ClientError, BotoCoreError, OSError, ValueError, TypeError, KeyError) as error:
        report.update(error=audit.failure(error, 'incomplete'),
                      failed_stage=report.get('failed_stage', report['stage']), complete=False)
    finally:
        try:
            checkpoint(report)
        except (OSError, ValueError, TypeError) as error:
            report.update(error=audit.failure(error, 'incomplete'),
                          failed_stage=report.get('failed_stage', 'local-checkpoint'), complete=False)
            status = 1
    print(json.dumps({key: report[key] for key in
                     ('complete', 'preflight_complete', 'protection_verified',
                      'summary', 'error', 'journal_error') if key in report}))
    return status


if __name__ == '__main__':
    raise SystemExit(main())
