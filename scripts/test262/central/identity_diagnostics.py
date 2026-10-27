"""Rebuild-only diagnosis of #2340; no fixture execution or catalogue mutations."""
import argparse
from datetime import datetime, timezone
import json
import os
from pathlib import Path, PurePosixPath
import re
import subprocess
import sys
from types import SimpleNamespace

# The controller is copied outside the original checkout before rebuilding it.
sys.path.insert(0, str(Path.cwd()))
from scripts.test262.central.client import bytea, identity, sha
from scripts.test262.central.diagnostics import report_failure
from scripts.test262.central.execution_identity import execution_identity
from scripts.test262.central.monitoring import github

REPOSITORY = 'tomacox74/js2il'
REPOSITORY_ID = 'faf01df8-aa65-4375-a708-6c1e555f957b'
SOURCE = '08b50be490e521bbd50c63ca3799f8bb33b58707'
PROVENANCE = 'b60fd015-e157-5f39-a8a6-b26b6ff1638c'
SNAPSHOT = 'eb93f530-6805-50a6-85ce-29bd798dde82'
VALIDATION = 'b2761fd5-fe24-559e-a912-bbecf5058a19'
AUTHORIZATION = 'identity-readonly-2340-20261027'
WORKFLOW = '.github/workflows/test262-identity-diagnostics.yml'
FILES = {'binaries', 'harness', 'tooling'}
FIELDS = {
    'runner', 'upstream', 'inventory', 'capabilities', 'environment_identity',
    'timeouts', 'compiler_options', 'owner', 'repo', 'cloneUrl', 'commit',
    'packageVersion', 'os', 'machine', 'node', 'major', 'os_release', 'id',
    'version_id', 'dotnet_runtime', 'name', 'requested', 'selected', 'runtime',
    'compile', 'cap_ms', 'schema_version', 'flags', 'async', 'onlyStrict',
    'noStrict', 'raw', 'module', 'includes', 'dependencies', 'sibling_files',
    'harness_files', 'negative', 'parse', 'resolution', 'isolation',
    'worker_process', 'timeout', 'agent_cleanup', 'compilation_coverage',
    'coverage_measurement',
}


def require(condition, message):
    if not condition:
        raise ValueError(message)


def write_report(args, report):
    path = Path(args.output)
    path.parent.mkdir(parents=True, exist_ok=True)
    pending = path.with_name(path.name + '.pending')
    with pending.open('w') as stream:
        json.dump(report, stream, indent=2)
        stream.write('\n')
        stream.flush()
        os.fsync(stream.fileno())
    pending.replace(path)


def safe_path(path):
    return (isinstance(path, str) and bool(re.fullmatch(r'[A-Za-z0-9_./+(),= -]+', path))
            and not PurePosixPath(path).is_absolute()
            and not any(part in ('', '.', '..') for part in path.split('/')))


def difference(path, expected, actual, expected_present=True, actual_present=True):
    return {'field': path, 'expected_present': expected_present, 'actual_present': actual_present,
            'expected_sha256': sha(expected) if expected_present else None,
            'actual_sha256': sha(actual) if actual_present else None}


def compare(expected, actual):
    require(isinstance(expected, dict) and isinstance(actual, dict), 'Identity documents must be objects')
    result = {'matches': sha(expected) == sha(actual), 'expected_sha256': sha(expected),
              'actual_sha256': sha(actual), 'components': [], 'file_differences': [],
              'field_differences': []}

    def fields(path, left, right, lp=True, rp=True):
        if lp and rp and sha(left) == sha(right):
            return
        if lp and rp and isinstance(left, dict) and isinstance(right, dict):
            for key in sorted(left.keys() | right.keys()):
                name = key if key in FIELDS else 'redacted-' + sha(key)
                fields(path + '.' + name, left.get(key), right.get(key), key in left, key in right)
        else:
            result['field_differences'].append(difference(path, left, right, lp, rp))

    for key in sorted(expected.keys() | actual.keys()):
        lp, rp = key in expected, key in actual
        left, right = expected.get(key), actual.get(key)
        name = key if key in FIELDS | FILES else 'redacted-' + sha(key)
        item = difference(name, left, right, lp, rp)
        item['matches'] = lp and rp and sha(left) == sha(right)
        result['components'].append(item)
        if key in FILES:
            require((not lp or isinstance(left, dict)) and (not rp or isinstance(right, dict)),
                    'File identity components must be maps')
            before, after = left if lp else {}, right if rp else {}
            for path in sorted(before.keys() | after.keys()):
                ep, ap = path in before, path in after
                old, new = before.get(path), after.get(path)
                require(all(not present or isinstance(value, str) and re.fullmatch('[a-f0-9]{64}', value)
                            for present, value in ((ep, old), (ap, new))), 'File digest must be SHA-256')
                if ep == ap and old == new:
                    continue
                result['file_differences'].append({
                    'component': key, 'path': path if safe_path(path) else '<redacted>',
                    'path_sha256': sha(path), 'expected_present': ep, 'actual_present': ap,
                    'expected_sha256': old, 'actual_sha256': new,
                    'status': 'changed' if ep and ap else 'missing' if ep else 'added'})
        else:
            fields(name, left, right, lp, rp)
    return result


def git(*args):
    return subprocess.check_output(['git', *args], text=True).strip()


def check_invocation(args):
    require(args.authorization == AUTHORIZATION and args.profile in ('refresh', 'drill'),
            'Explicit read-only diagnostic authorization/profile required')
    require(os.getenv('GITHUB_REPOSITORY') == REPOSITORY
            and os.getenv('GITHUB_EVENT_NAME') == 'workflow_dispatch'
            and os.getenv('GITHUB_REF') == 'refs/heads/master'
            and os.getenv('GITHUB_RUN_ATTEMPT') == '1', 'Protected manual master first attempt required')
    controller = os.getenv('GITHUB_SHA', '')
    run_id = os.getenv('GITHUB_RUN_ID', '')
    require(re.fullmatch('[a-f0-9]{40}', controller) and re.fullmatch('[1-9][0-9]*', run_id),
            'Authenticated controller source/run required')
    run = github(f'repos/{REPOSITORY}/actions/runs/{run_id}/attempts/1')
    require(run['id'] == int(run_id) and run['run_attempt'] == 1
            and run['repository']['full_name'] == run['head_repository']['full_name'] == REPOSITORY
            and run['head_sha'] == controller and run['head_branch'] == 'master'
            and run['path'] == WORKFLOW and run['event'] == 'workflow_dispatch'
            and run['status'] == 'in_progress', 'Foreign diagnostic controller')
    require(git('rev-parse', 'HEAD') == SOURCE and not git('status', '--porcelain', '--untracked-files=no')
            and git('rev-parse', '--is-shallow-repository') == ('true' if args.profile == 'drill' else 'false'),
            'Original source/build profile must be exact and clean')
    return {key: run[key] for key in ('id', 'run_attempt', 'head_sha', 'event', 'path', 'html_url')}


def open_database():
    import psycopg
    return psycopg.connect(
        os.environ['TEST262_DATABASE_URL'], autocommit=True,
        options='-c default_transaction_read_only=on -c statement_timeout=120000 -c lock_timeout=15000')


def read_original(args):
    from psycopg.types.json import Jsonb
    with open_database() as db:
        require(db.execute('SHOW default_transaction_read_only').fetchone()[0] == 'on',
                'Server default must be read only')
        with db.transaction():
            db.execute('SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY')
            require(db.execute('SHOW transaction_read_only').fetchone()[0] == 'on',
                    'Server transaction must be read only')
            contract = db.execute('SELECT test262.api_contract()').fetchone()[0]
            require(contract['api_contract_version'] == 1
                    and contract['repository_id'] == args.repository == REPOSITORY_ID
                    and contract['producer_id'] == args.producer
                    and contract['trust_class'] == 'trusted' and contract['permission'] == 'coordinator'
                    and contract['deployment_state'] == 'active'
                    and contract['minimum_writer_epoch'] == args.epoch, 'Foreign catalogue read scope')

            def one(table, key, value):
                allowed = {'provenances': 'provenance_id', 'corpora': 'corpus_id',
                           'registration_snapshots': 'snapshot_id', 'validation_events': 'validation_id'}
                require(allowed.get(table) == key, 'Only fixed original identity reads are permitted')
                result = db.execute('SELECT test262.api_read(%s,%s,%s,%s,%s)',
                                    (args.epoch, table, Jsonb({key: value}), None, 2)).fetchone()[0]
                require(len(result['rows']) == 1 and not result['next'], 'Exact immutable baseline row required')
                return result['rows'][0]

            provenance = one('provenances', 'provenance_id', PROVENANCE)
            corpus = one('corpora', 'corpus_id', provenance['corpus_id'])
            snapshot = one('registration_snapshots', 'snapshot_id', SNAPSHOT)
            validation = one('validation_events', 'validation_id', VALIDATION)
            doc = provenance['identity_document']
            require(provenance['provenance_id'] == PROVENANCE
                    and provenance['repository_id'] == REPOSITORY_ID
                    and provenance['evidence_kind'] == 'native' and provenance['identity_version'] == 1
                    and provenance['identity_sha256'] == bytea(sha(doc))
                    and identity(REPOSITORY_ID, 'native', sha(doc)) == PROVENANCE
                    and corpus['corpus_id'] == provenance['corpus_id'] == doc['inventory']
                    and corpus['inventory_state'] == 'sealed'
                    and corpus['revision'] == doc['upstream']['commit']
                    and corpus['upstream_url'] == doc['upstream']['cloneUrl'],
                    'Original provenance/corpus identity is not intact')
            require(snapshot['snapshot_id'] == SNAPSHOT and snapshot['repository_id'] == REPOSITORY_ID
                    and snapshot['state'] == 'sealed' and snapshot['source_revision'] == SOURCE
                    and validation['validation_id'] == VALIDATION
                    and validation['repository_id'] == REPOSITORY_ID
                    and validation['target_revision'] == SOURCE and validation['conclusion'] == 'success'
                    and validation['external_run_key'] == '40565545982',
                    'Original source registration/validation binding differs')
            return {'document': doc, 'corpus': corpus,
                    'authority': {key: contract[key] for key in
                                  ('repository_id', 'producer_id', 'minimum_writer_epoch', 'deployment_state')},
                    'database_read_only': True}


def run(args, report):
    report.update(schema_version=1, complete=False, stage='authorization',
                  source_revision=SOURCE, original_provenance=PROVENANCE, profile=args.profile,
                  fixture_executions=0, catalogue_writes=False, publication=False,
                  permission_or_authority_changes=False, imports_or_restores=False, s3_access=False)
    report['actions'] = check_invocation(args)
    write_report(args, report)
    report['stage'] = 'read-original-identity'
    original = read_original(args)
    report['database_read_only'] = original['database_read_only']
    report['authority'] = original['authority']
    report['stage'] = 'compute-rebuilt-identity'
    require(git('-C', args.root, 'rev-parse', 'HEAD') == original['document']['upstream']['commit']
            and not git('-C', args.root, 'status', '--porcelain', '--untracked-files=no'),
            'Original pinned upstream must be clean')
    build = SimpleNamespace(root=args.root, kind='native',
                            host='scripts/test262/NativeScreeningHost/bin/Release/net10.0/NativeScreeningHost.dll',
                            runtime_timeout=30, compile_timeout=60, cap_ms=120000,
                            compilation_coverage=False)
    actual = execution_identity(build, original['corpus']['corpus_id'])
    report['stage'] = 'compare'
    report['comparison'] = compare(original['document'], actual)
    report['sealed_upstream_matches'] = (
        original['corpus']['revision'] == actual['upstream']['commit']
        and original['corpus']['upstream_url'] == actual['upstream']['cloneUrl'])
    require(git('rev-parse', 'HEAD') == SOURCE and not git('status', '--porcelain', '--untracked-files=no'),
            'Original source changed during diagnostics')
    report.update(complete=True, stage='compared', checked_at=datetime.now(timezone.utc).isoformat(),
                  historical_failed_build_document_retained=False)
    write_report(args, report)
    return report


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--authorization', required=True)
    parser.add_argument('--profile', choices=('refresh', 'drill'), required=True)
    parser.add_argument('--root', required=True)
    parser.add_argument('--output', required=True)
    parser.add_argument('--repository', default=os.getenv('TEST262_REPOSITORY_ID'))
    parser.add_argument('--producer', default=os.getenv('TEST262_PRODUCER_ID'))
    parser.add_argument('--epoch', type=int, default=os.getenv('TEST262_AUTHORITY_EPOCH'))
    args = parser.parse_args(argv)
    report = {'complete': False, 'stage': 'authorization'}
    try:
        require(args.repository == REPOSITORY_ID and args.producer and args.epoch is not None,
                'Explicit original catalogue scope required')
        run(args, report)
        return 0
    except Exception as error:
        report.update(complete=False, error_class=type(error).__name__)
        error._test262_stage = 'identity-diagnostic.' + report['stage']
        try:
            write_report(args, report)
        except OSError as journal_error:
            journal_error._test262_stage = 'identity-diagnostic.failure-journal'
            report_failure(journal_error)
        report_failure(error)
        return 1


if __name__ == '__main__':
    raise SystemExit(main())
